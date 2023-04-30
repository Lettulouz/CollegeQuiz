using System;
using System.ComponentModel.DataAnnotations;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Utils;

namespace CollegeQuizWeb.Dto.Admin;

public class CategoryListDto
{
    public long CategoryId { get; set; }
    
    [Required(ErrorMessage = Lang.CATEGORYNAME_IS_REQUIRED_ERROR)]
    [RegularExpression(RegexApp.ONLY_SMALL_LETTERS_NUM, ErrorMessage = Lang.CATEGORYNAME_REGEX_ERROR)]
    [MinLength(5, ErrorMessage = Lang.CATEGORYNAME_TOO_SHORT_ERROR)]
    [MaxLength(25, ErrorMessage = Lang.CATEGORYNAME_TOO_LONG_ERROR)]
    public string CategoryName { get; set; }
}

public class CategoryListDtoPayload : AbstractControllerPayloader<AdminController>
{
    public CategoryListDto Dto { get; set; }
    
    public CategoryListDtoPayload(AdminController controllerReference) 
        : base(controllerReference) { }
}