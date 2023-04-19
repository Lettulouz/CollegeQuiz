using System;
using System.ComponentModel.DataAnnotations;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Utils;

namespace CollegeQuizWeb.Dto.Admin;

public class QuizListDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    
    public bool IsPublic { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public long UserId { get; set; }

    public string UserName;
}