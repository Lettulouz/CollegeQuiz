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

    public async Task<string> UpdateQuizQuestionImage(IFormFile formFile, long quizId, string qstId, Controller controller)
    {
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

        string fileName = $"question{qstId}.jpg";
        string imageCndEndpoint = 
            $"{ConfigLoader.SftpHref}{ConfigLoader.ExternalContentServerUploadDir}/{quizId}/question{qstId}.jpg";
        
        if (!ConfigLoader.ExternalContentServerActive)
        {
            string url = $"{Utilities.GetBaseUrl(controller)}/api/v1/dotnet/quizapi/GetQuizImage/{quizId}/{qstId}";
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
        await client.DeleteDirectory(dirName);
        await _asyncSftpConnector.Disconnect();
    }

    public async Task<string> GetImagePath(string basePath, long quizId, int qstId)
    {
        if (ConfigLoader.ExternalContentServerActive)
        {
            string imageCndEndpoint = 
                $"{ConfigLoader.SftpHref}{ConfigLoader.ExternalContentServerUploadDir}/{quizId}/question{qstId}.jpg";
            AsyncFtpClient client = await _asyncSftpConnector.Connect();
            string imageRemoteUrl = $"{REMOTE_UPLOADS_DIR}/{quizId}/question{qstId}.jpg";
            if (!await client.FileExists(imageRemoteUrl))
            {
                imageCndEndpoint = string.Empty;
            }
            await _asyncSftpConnector.Disconnect();
            return imageCndEndpoint;
        }
        string imageEndpoint = $"{basePath}/api/v1/dotnet/quizapi/GetQuizImage/{quizId}/{qstId}";
        string imageUrl = $"{STATIC_UPLOADS_DIR}/{quizId}/question{qstId}.jpg";
        if (!File.Exists(imageUrl))
        {
            imageEndpoint = string.Empty;
        }
        return imageEndpoint;
    }

    public async Task<List<string>> GetAllQuizImagesInBase64(long quizId, int imagesCount)
    {
        List<string> imagesBase64 = new List<string>();
        if (ConfigLoader.ExternalContentServerActive)
        {
            AsyncFtpClient client = await _asyncSftpConnector.Connect();
            for (int i = 1; i <= imagesCount; i++)
            {
                string imageFile = $"{REMOTE_UPLOADS_DIR}/{quizId}/question{i}.jpg";
                if (!await client.FileExists(imageFile))
                {
                    imagesBase64.Add(string.Empty);
                    continue;
                }
                byte[] fileInBytes = await client.DownloadBytes(imageFile, 0);

                MemoryStream inputStream = new MemoryStream(fileInBytes);
                imagesBase64.Add(ConvertStreamImageToBase64(SKBitmap.Decode(inputStream)));
                inputStream.Close();
            }
            await _asyncSftpConnector.Disconnect();
            return imagesBase64;
        }
        for (int i = 1; i <= imagesCount; i++)
        {
            string imagePath = $"{STATIC_UPLOADS_DIR}/{quizId}/question{i}.jpg";
            if (!File.Exists(imagePath))
            {
                imagesBase64.Add(string.Empty);
                continue;
            }
            var inputStream = new SKFileStream(imagePath);
            imagesBase64.Add(ConvertStreamImageToBase64(SKBitmap.Decode(inputStream)));
            inputStream.Dispose();
        }
        return imagesBase64;
    }

    public async Task<byte[]> GetQuizQuestionImageAsBytesArray(long quizId, int questionId)
    {
        if (ConfigLoader.ExternalContentServerActive)
        {
            string imageFile = $"{REMOTE_UPLOADS_DIR}/{quizId}/question{questionId}.jpg";
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
        string imageFileStatic = $"{STATIC_UPLOADS_DIR}/{quizId}/question{questionId}.jpg";
        if (!File.Exists(imageFileStatic))
        {
            return new byte[0];
        }
        return File.ReadAllBytes(imageFileStatic);
    }
    
    private string ConvertStreamImageToBase64(SKBitmap bitmap)
    {
        MemoryStream outputStream = new MemoryStream();
        
        var skImage = SKImage.FromBitmap(bitmap);
        var imageData = skImage.Encode(SKEncodedImageFormat.Jpeg, 100);
        imageData.SaveTo(outputStream);

        imageData.Dispose();
        skImage.Dispose();
        outputStream.Close();
        return Convert.ToBase64String(outputStream.ToArray());
    }
}