namespace CollegeQuizWeb.Utils;

public static class Lang
{
    public const string FirstNameTooShortErrorError = "Podane imię jest za krótkie dla naszej bazy, przepraszamy :(";
    public const string FirstNameTooLongErrorError = "Podane imię jest za długie dla naszej bazy, przepraszamy :(";
    public const string FirstNameRegexError = 
        "W polu imię znalazły się niedozwolone znaki, usuń je i spróbuj ponownie";
    
    public const string LastNameTooShortErrorError = "Podane nazwisko jest za krótkie dla naszej bazy, przepraszamy :(";
    public const string LastNameTooLongErrorError = "Podane nazwisko jest za długie dla naszej bazy, przepraszamy :(";
    public const string LastNameRegexError = 
        "W polu nazwisko znalazły się niedozwolone znaki, usuń je i spróbuj ponownie";
    
    public const string UsernameTooShortError = "Podany login jest za krótki";
    public const string UsernameTooLongError = "Podany login jest za długi";
    public const string UsernameRegexError = "W loginie powinny znaleźć się tylko małe litery oraz cyfry";
    
    public const string EmailNotCorrectError = "Wpisana treść nie jest emailem";
    public const string EmailTooLongError = "Podana treść nie spełnia założeń emaila";
    
    public const string PasswordTooShortError = "Podano za krótkie hasło";
    public const string PasswordTooLongError = "Podano za długie hasło";
    public const string PasswordRegexError =
        "Hasło powinno zawierać co najmniej jedną dużą literę, jedną małą literę, " +
        "jedną cyfrę oraz jeden znak specjalny";
}