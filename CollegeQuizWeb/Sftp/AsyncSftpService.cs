using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CollegeQuizWeb.Config;
using CollegeQuizWeb.Utils;
using FluentFTP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

namespace CollegeQuizWeb.Sftp;

public class AsyncSftpService : IAsyncSftpService
{
    private readonly IAsyncSftpConnector _asyncSftpConnector;
    
    private static readonly string ROOT_PATH = Directory.GetCurrentDirectory();
    public static readonly string STATIC_UPLOADS_DIR = $"{ROOT_PATH}/_Uploads/QuizImages";
    public static readonly string REMOTE_UPLOADS_DIR =
        ConfigLoader.ExternalContentServerBaseDir + ConfigLoader.ExternalContentServerUploadDir;
    
    public AsyncSftpService(IAsyncSftpConnector asyncSftpConnector)
    {
        _asyncSftpConnector = asyncSftpConnector;
    }

    public async Task PrepareDirectory(int imagesCount, long quizId)
    {
        if (ConfigLoader.ExternalContentServerActive)
        {
            AsyncFtpClient client = await _asyncSftpConnector.Connect();
            string dirName = $"{REMOTE_UPLOADS_DIR}/{quizId}";
            if (!await client.DirectoryExists(dirName))
            {
                await client.CreateDirectory(dirName);
            }
            else
            {
                if (imagesCount == 0) await client.DeleteDirectory(dirName);
                else await client.EmptyDirectory(dirName, FtpListOption.Recursive);
            }
            await _asyncSftpConnector.Disconnect();
            return;
        }
        string dir = $"{STATIC_UPLOADS_DIR}/{quizId}";
        if (imagesCount == 0 && Directory.Exists(dir))
        {
            Directory.Delete(dir, true);
            return;
        }
        DirectoryInfo directoryInfo = new DirectoryInfo(dir);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        foreach (var file in directoryInfo.GetFiles())
        {
            file.Delete();
        }
    }

    public async Task<string> UpdateQuizQuestionImage(IFormFile formFile, long quizId, string qstId, DateTime qDt, Controller controller)
    {
        string preHash = qDt.ToString("yyyyMMddHHmmss");
        
        MemoryStream inputMemoryStream = new MemoryStream();
        MemoryStream outputMemoryStream = new MemoryStream();

        await formFile.CopyToAsync(inputMemoryStream);
        
        SKBitmap skBitmap = SKBitmap.Decode(inputMemoryStream.ToArray());
        SKBitmap resizedBitmap = new SKBitmap(300, 300);
        skBitmap.ScalePixels(resizedBitmap, SKFilterQuality.High);
        
        SKSurface skSurface = SKSurface.Create(new SKImageInfo(300, 300));
        SKCanvas skCanvas = skSurface.Canvas;
        skCanvas.DrawBitmap(resizedBitmap, 0, 0);
        SKImage skImage = skSurface.Snapshot();

        var encodedData = skImage.Encode(SKEncodedImageFormat.Jpeg, 100);
        
        skCanvas.Dispose();
        skSurface.Dispose();
        skBitmap.Dispose();
        inputMemoryStream.Close();

        string fileName = $"question{qstId}_{preHash}.jpg";
        string imageCndEndpoint = 
            $"{ConfigLoader.SftpHref}{ConfigLoader.ExternalContentServerUploadDir}/{quizId}/{fileName}";
        
        if (!ConfigLoader.ExternalContentServerActive)
        {
            string url = $"{Utilities.GetBaseUrl(controller)}/api/v1/dotnet/quizapi/GetQuizImage/{quizId}/{qstId}/{preHash}";
            var stream = new FileStream($"{STATIC_UPLOADS_DIR}/{quizId}/{fileName}", FileMode.Create);
            encodedData.SaveTo(stream);
            stream.Close();
            return url;
        }
        AsyncFtpClient client = await _asyncSftpConnector.Connect();
        encodedData.SaveTo(outputMemoryStream);
        await client.UploadBytes(outputMemoryStream.ToArray(), $"{REMOTE_UPLOADS_DIR}/{quizId}/{fileName}");
        await _asyncSftpConnector.Disconnect();
        outputMemoryStream.Close();
        return imageCndEndpoint;
    }

    public async Task DeleteQuizImages(long quizId)
    {
        if (!ConfigLoader.ExternalContentServerActive)
        {
            string dir = $"{STATIC_UPLOADS_DIR}/{quizId}";
            Directory.Delete(dir, true);
            return;
        }
        string dirName = $"{REMOTE_UPLOADS_DIR}/{quizId}";
        AsyncFtpClient client = await _asyncSftpConnector.Connect();
        if (await client.DirectoryExists(dirName))
        {
            await client.DeleteDirectory(dirName);
        }
        await _asyncSftpConnector.Disconnect();
    }

    public async Task<string> GetImagePath(string basePath, long quizId, int qstId, DateTime qDt)
    {
        string rawToken = qDt.ToString("yyyyMMddHHmmss");
        string fileName = $"question{qstId}_{rawToken}.jpg";
        if (ConfigLoader.ExternalContentServerActive)
        {
            string imageCndEndpoint =
                $"{ConfigLoader.SftpHref}{ConfigLoader.ExternalContentServerUploadDir}/{quizId}/{fileName}";
            AsyncFtpClient client = await _asyncSftpConnector.Connect();
            string imageRemoteUrl = $"{REMOTE_UPLOADS_DIR}/{quizId}/{fileName}";
            if (!await client.FileExists(imageRemoteUrl))
            {
                imageCndEndpoint = string.Empty;
            }
            await _asyncSftpConnector.Disconnect();
            return imageCndEndpoint;
        }
        string imageEndpoint = $"{basePath}/api/v1/dotnet/quizapi/GetQuizImage/{quizId}/{qstId}/{rawToken}";
        string imageUrl = $"{STATIC_UPLOADS_DIR}/{quizId}/{fileName}";
        if (!File.Exists(imageUrl))
        {
            imageEndpoint = string.Empty;
        }
        return imageEndpoint;
    }

    public async Task<List<string>> GetAllQuizImagesPath(string basePath, long quizId, List<DateTime> questionsDateTime)
    {
        List<string> imagesLinks = new List<string>();
        if (ConfigLoader.ExternalContentServerActive)
        {
            AsyncFtpClient client = await _asyncSftpConnector.Connect();
            for (int i = 1; i <= questionsDateTime.Count; i++)
            {
                string preHash = questionsDateTime[i - 1].ToString("yyyyMMddHHmmss");
                string imageFile = 
                    $"{ConfigLoader.SftpHref}{ConfigLoader.ExternalContentServerUploadDir}/{quizId}/question{i}_{preHash}.jpg";
                string imageRemoteUrl = $"{REMOTE_UPLOADS_DIR}/{quizId}/question{i}_{preHash}.jpg";
                if (!await client.FileExists(imageRemoteUrl))
                {
                    imagesLinks.Add(string.Empty);
                    continue;
                }
                imagesLinks.Add(imageFile);
            }
            await _asyncSftpConnector.Disconnect();
            return imagesLinks;
        }
        for (int i = 1; i <= questionsDateTime.Count; i++)
        {
            string preHash = questionsDateTime[i].ToString("yyyyMMddHHmmss");
            string imagePath = $"{STATIC_UPLOADS_DIR}/{quizId}/question{i}_{preHash}.jpg";
            string imageEndpoint = $"{basePath}/api/v1/dotnet/quizapi/GetQuizImage/{quizId}/{i}/{preHash}";
            if (!File.Exists(imagePath))
            {
                imagesLinks.Add(string.Empty);
                continue;
            }
            imagesLinks.Add(imageEndpoint);
        }
        return imagesLinks;
    }
    
    public async Task<byte[]> GetQuizQuestionImageAsBytesArray(long quizId, int questionId, string qDt)
    {
        if (ConfigLoader.ExternalContentServerActive)
        {
            string imageFile = $"{REMOTE_UPLOADS_DIR}/{quizId}/question{questionId}_{qDt}.jpg";
            AsyncFtpClient client = await _asyncSftpConnector.Connect();
            if (!await client.FileExists(imageFile))
            {
                await _asyncSftpConnector.Disconnect();
                return new byte[0];
            }
            byte[] fileInBytes = await client.DownloadBytes(imageFile, 0);
            await _asyncSftpConnector.Disconnect();
            return fileInBytes;
        }
        string imageFileStatic = $"{STATIC_UPLOADS_DIR}/{quizId}/question{questionId}_{qDt}.jpg";
        if (!File.Exists(imageFileStatic))
        {
            return new byte[0];
        }
        return File.ReadAllBytes(imageFileStatic);
    }
}