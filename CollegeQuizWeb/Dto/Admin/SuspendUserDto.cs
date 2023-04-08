using System;
using System.ComponentModel.DataAnnotations;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Utils;

namespace CollegeQuizWeb.Dto.Admin;

public class SuspendUserDto
{

    public long Id { get; set; }
    public bool Perm { get; set; }
    
    public DateTime SuspendedTo { get; set; }

}

public class SuspendUserDtoPayload : AbstractControllerPayloader<AdminController>
{
    public SuspendUserDto Dto { get; set; }
    
    public SuspendUserDtoPayload(AdminController controllerReference) 
        : base(controllerReference) { }
}