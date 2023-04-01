namespace CollegeQuizWeb.Utils;

public static class Lang
{
    public const string FIRST_NAME_TOO_SHORT_ERROR = "Podane imię jest za krótkie dla naszej bazy, przepraszamy :(";
    public const string FIRST_NAME_TOO_LONG_ERROR = "Podane imię jest za długie dla naszej bazy, przepraszamy :(";
    public const string FIRST_NAME_REGEX_ERROR = 
        "W polu imię znalazły się niedozwolone znaki, usuń je i spróbuj ponownie";
    
    public const string LAST_NAME_TOO_SHORT_ERROR = "Podane nazwisko jest za krótkie dla naszej bazy, przepraszamy :(";
    public const string LAST_NAME_TOO_LONG_ERROR = "Podane nazwisko jest za długie dla naszej bazy, przepraszamy :(";
    public const string LAST_NAME_REGEX_ERROR = 
        "W polu nazwisko znalazły się niedozwolone znaki, usuń je i spróbuj ponownie";
    
    public const string USERNAME_TOO_SHORT_ERROR = "Podany login jest za krótki";
    public const string USERNAME_TOO_LONG_ERROR = "Podany login jest za długi";
    public const string USERNAME_REGEX_ERROR = "W loginie powinny znaleźć się tylko małe litery oraz cyfry";
    
    public const string EMAIL_INCORRECT_ERROR = "Wpisana treść nie jest emailem";
    public const string EMAIL_TOO_LONG_ERROR = "Podana treść nie spełnia założeń emaila";
    
    public const string PASSWORD_TO_SHORT_ERROR = "Podano za krótkie hasło";
    public const string PASSWORD_REGEX_ERROR =
        "Hasło powinno zawierać co najmniej jedną dużą literę, jedną małą literę, " +
        "jedną cyfrę oraz jeden znak specjalny";

    public const string USERNAME_OR_EMAIL_REQUIRED = "Pole email/nazwa użytkownika jest wymanage";
    public const string USERNAME_OR_EMAIL_INVALID_CHARS = "Pole email/nazwa użytkownika zawiera niedozwolone znaki";
    public const string PASSWORD_REQUIRED = "Pole hasła jest wymagane";
    public const string PASSWORDS_NOT_MATCH = "Podane hasła nie są takie same";
}