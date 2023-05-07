using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.Sftp;

public interface IAsyncSftpService
{
    Task PrepareDirectory(int imagesCount, long quizId);
    Task<string> UpdateQuizQuestionImage(IFormFile formFile, long quizId, string qstId, Controller controller);
    Task DeleteQuizImages(long quizId);
    Task<string> GetImagePath(string basePath, long quizId, int qstId);
    Task<List<string>> GetAllQuizImagesPath(string basePath, long quizId, int imagesCount);
    Task<List<string>> GetAllQuizImagesInBase64(long quizId, int imagesCount);
    Task<byte[]> GetQuizQuestionImageAsBytesArray(long quizId, int questionId);
}