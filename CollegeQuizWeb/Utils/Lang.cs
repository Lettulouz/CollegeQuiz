namespace CollegeQuizWeb.Utils;

public static class Lang
{
    public const string FIRST_NAME_IS_REQUIRED_ERROR = "Pole Imię jest wymagane.";
    public const string FirstNameTooShortError = "Podane imię jest za krótkie dla naszej bazy, przepraszamy :(";
    public const string FirstNameTooLongError = "Podane imię jest za długie dla naszej bazy, przepraszamy :(";
    public const string FirstNameRegexError = 
        "W polu imię znalazły się niedozwolone znaki, usuń je i spróbuj ponownie.";
    
    public const string LAST_NAME_IS_REQUIRED_ERROR = "Pole Nazwisko jest wymagane.";
    public const string LastNameTooShortError = "Podane nazwisko jest za krótkie dla naszej bazy, przepraszamy :(";
    public const string LastNameTooLongError = "Podane nazwisko jest za długie dla naszej bazy, przepraszamy :(";
    public const string LastNameRegexError = 
        "W polu nazwisko znalazły się niedozwolone znaki, usuń je i spróbuj ponownie.";
    
    public const string USERNAME_IS_REQUIRED_ERROR = "Pole Login jest wymagane.";
    public const string UsernameTooShortError = "Podany login jest za krótki.";
    public const string UsernameTooLongError = "Podany login jest za długi.";
    public const string UsernameRegexError = "W loginie powinny znaleźć się tylko małe litery oraz cyfry.";
    public const string UsernameAlreadyExistsError = "Podany login już istnieje.";
    
    public const string EMAIL_IS_REQUIRED_ERROR = "Pole Email jest wymagane";
    public const string EmailNotCorrectError = "Wpisana treść nie jest emailem.";
    public const string EmailTooLongError = "Podana treść nie spełnia założeń emaila.";
    public const string EmailAlreadyExistsError = "Podany email już istnieje.";
    
    public const string PASSWORD_IS_REQUIRED_ERROR = "Pole Hasło jest wymagane.";
    public const string PasswordTooShortError = "Podano za krótkie hasło.";
    public const string PasswordTooLongError = "Podano za długie hasło.";
    public const string PasswordRegexError =
        "Hasło powinno zawierać co najmniej jedną dużą literę, jedną małą literę, " +
        "jedną cyfrę oraz jeden znak specjalny.";
}