using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.Sftp;

public interface IAsyncSftpService
{
    /// <summary>
    /// Method that prapare directory for images and quizes
    /// </summary>
    /// <param name="imagesCount">Number of images</param>
    /// <param name="quizId">Quiz Id</param>
    Task PrepareDirectory(int imagesCount, long quizId);
    
    /// <summary>
    /// Method that update image in question
    /// </summary>
    /// <param name="formFile">File</param>
    /// <param name="quizId">Quiz Id</param>
    /// <param name="qstId">Question Id</param>
    /// <param name="qDt">Time when image is updated</param>
    /// <param name="controller"></param>
    /// <returns>imageCndEndpoint</returns>
    Task<string> UpdateQuizQuestionImage(IFormFile formFile, long quizId, string qstId, DateTime qDt, Controller controller);
    
    /// <summary>
    /// Method that delete image from question
    /// </summary>
    /// <param name="quizId">Quiz Id</param>
    Task DeleteQuizImages(long quizId);
    
    /// <summary>
    /// Mathod that get image path
    /// </summary>
    /// <param name="basePath">Base path</param>
    /// <param name="quizId">Quiz Id</param>
    /// <param name="qstId">Question Id</param>
    /// <param name="qDt">Time when image is updated</param>
    /// <returns>Images path</returns>
    Task<string> GetImagePath(string basePath, long quizId, int qstId, DateTime qDt);
    
    /// <summary>
    /// Method that get all quiz images path
    /// </summary>
    /// <param name="basePath">Base path</param>
    /// <param name="quizId">Quiz Id</param>
    /// <param name="questionsDateTime">Time when questions are updated</param>
    /// <returns>Images path</returns>
    Task<List<string>> GetAllQuizImagesPath(string basePath, long quizId, List<DateTime> questionsDateTime);
    
    /// <summary>
    /// Method that get quiz questions image as bytes array
    /// </summary>
    /// <param name="quizId">Quiz Id</param>
    /// <param name="questionId">Question Id</param>
    /// <param name="qDt">Time when image is updated</param>
    /// <returns>Question image as bytes array</returns>
    Task<byte[]> GetQuizQuestionImageAsBytesArray(long quizId, int questionId, string qDt);
}