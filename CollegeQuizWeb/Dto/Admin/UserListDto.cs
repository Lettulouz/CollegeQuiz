using System;

namespace CollegeQuizWeb.Dto.Admin;

public class UserListDto
{
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string UserName { get; set; }
    
    public string? Email { get; set; }
    
    public long Id { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public int AccountStatus { get; set; }
    
    public bool IsAccountActivated { get; set; }
}