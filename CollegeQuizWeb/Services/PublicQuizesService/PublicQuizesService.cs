using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto.PublicQuizes;
using CollegeQuizWeb.Dto.Quiz;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeQuizWeb.Services.PublicQuizesService;

public class PublicQuizesService : IPublicQuizesService
{
    private readonly ApplicationDbContext _context;
    
    public PublicQuizesService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MyQuizDto>> GetPublicQuizes()
    {
        return await _context.ShareTokensEntities.Include(t => t.QuizEntity)
            .Where(q => q.QuizEntity.IsPublic.Equals(true))
            .Select(q => new MyQuizDto()
                { Name = q.QuizEntity.Name, Id = q.QuizEntity.Id, Token = q.Token})
            .ToListAsync();
    }

    public async Task<List<MyQuizDto>> FilterQuizes(PublicDtoPayLoad obj)
    {
        PublicQuizesController controller = obj.ControllerReference;
        
        return await _context.ShareTokensEntities.Include(t => t.QuizEntity)
            .Where(q => q.QuizEntity.IsPublic.Equals(true) && q.QuizEntity.Name.Contains(obj.Dto.Name))
            .Select(q => new MyQuizDto()
                { Name = q.QuizEntity.Name, Id = q.QuizEntity.Id, Token = q.Token})
            .ToListAsync();
        
    }
    
}