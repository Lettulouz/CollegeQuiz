using System.Threading.Tasks;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.Controllers;

public class QuizController : Controller
{
    private readonly ApplicationDbContext _context;

    public QuizController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult AddQuiz()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddQuiz(QuizEntity question, QuestionEntity answer1, QuestionEntity answer2, QuestionEntity answer3, QuestionEntity answer4)
    {
        if (ModelState.IsValid)
        {
            _context.Add(question);
            await _context.SaveChangesAsync();

            answer1.Id = question.Id;
            answer2.Id = question.Id;
            answer3.Id = question.Id;
            answer4.Id = question.Id;

            _context.AddRange(answer1, answer2, answer3, answer4);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AddQuiz));
        }
        return View(question);
    }
}