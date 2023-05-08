using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.Sftp;

public interface IAsyncSftpService
{
    Task PrepareDirectory(int imagesCount, long quizId);
    Task<string> UpdateQuizQuestionImage(IFormFile formFile, long quizId, string qstId, DateTime qDt, Controller controller);
    Task DeleteQuizImages(long quizId);
    Task<string> GetImagePath(string basePath, long quizId, int qstId, DateTime qDt);
    Task<List<string>> GetAllQuizImagesPath(string basePath, long quizId, List<DateTime> questionsDateTime);
    Task<byte[]> GetQuizQuestionImageAsBytesArray(long quizId, int questionId, string qDt);
}