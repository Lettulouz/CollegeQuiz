namespace CollegeQuizWeb.Utils;

public static class Lang
{
    public const string FIRST_NAME_IS_REQUIRED_ERROR = "*Pole Imię jest wymagane.";
    public const string FIRST_NAME_TOO_SHORT_ERROR = "*Podane imię jest za krótkie dla naszej bazy, przepraszamy :(";
    public const string FIRST_NAME_TOO_LONG_ERROR = "*Podane imię jest za długie dla naszej bazy, przepraszamy :(";
    public const string FIRST_NAME_REGEX_ERROR = 
        "*W polu imię znalazły się niedozwolone znaki, usuń je i spróbuj ponownie.";
    
    public const string LAST_NAME_IS_REQUIRED_ERROR = "*Pole Nazwisko jest wymagane.";
    public const string LAST_NAME_TOO_SHORT_ERROR = "*Podane nazwisko jest za krótkie dla naszej bazy, przepraszamy :(";
    public const string LAST_NAME_TOO_LONG_ERROR = "*Podane nazwisko jest za długie dla naszej bazy, przepraszamy :(";
    public const string LAST_NAME_REGEX_ERROR = 
        "*W polu nazwisko znalazły się niedozwolone znaki, usuń je i spróbuj ponownie.";
    
    public const string USERNAME_IS_REQUIRED_ERROR = "*Pole Login jest wymagane.";
    public const string USERNAME_TOO_SHORT_ERROR = "*Podany login jest za krótki.";
    public const string USERNAME_TOO_LONG_ERROR = "*Podany login jest za długi.";
    public const string USERNAME_REGEX_ERROR = "*W loginie powinny znaleźć się tylko małe litery oraz cyfry.";
    
    public const string EMAIL_IS_REQUIRED_ERROR = "*Pole Email jest wymagane.";
    public const string EMAIL_INCORRECT_ERROR = "*Wpisana treść nie jest emailem.";
    public const string EMAIL_TOO_LONG_ERROR = "*Podana treść nie spełnia założeń emaila.";
    
    public const string PASSWORD_IS_REQUIRED_ERROR = "*Pole Hasło jest wymagane.";
    public const string PASSWORD_TO_SHORT_ERROR = "*Podano za krótkie hasło.";
    public const string PASSWORD_TO_LONG_ERROR = "*Podano za długie hasło.";
    public const string PASSWORD_REGEX_ERROR =
        "*Hasło powinno zawierać co najmniej jedną dużą literę, jedną małą literę, " +
        "jedną cyfrę oraz jeden znak specjalny.";

    public const string USERNAME_OR_EMAIL_REQUIRED = "*Pole Login lub E-mail jest wymagane.";
    public const string USERNAME_OR_EMAIL_INVALID_CHARS = "*Pole Login lub E-mail zawiera niedozwolone znaki.";
    public const string PASSWORD_REQUIRED = "*Pole Hasło jest wymagane.";
    public const string PASSWORDS_NOT_MATCH = "*Podane Hasło nie są takie same.";

    public const string EMAIL_ALREADY_EXIST = "*Podany email już istnieje.";
    public const string USERNAME_ALREADY_EXIST = "*Podany login już istnieje.";

    public const string RULES_ACCEPT = "*Należy zaakceptować regulamin oraz politykę prywatności.";
    
    public const string INVALID_PASSWORD = "*Podane dane są nieprawidłowe.";
    public const string UNACTIVATED_ACCOUNT = "*Należy najpierw aktywować konto. Wiadomość została wysłana na twój adres e-mail";
    
    public const string QUIZ_NAME_REQUIRED_ERROR = "*Nazwa quizu jest wymagana";
    public const string QUIZ_NAME_TOO_SHORT_ERROR = "*Podana nazwa quizu jest zbyt krótka";
    public const string QUIZ_NAME_TOO_LONG_ERROR = "*Podana nazwa quizu jest zbyt długa";
}