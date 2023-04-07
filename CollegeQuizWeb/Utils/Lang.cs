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
    public const string UNACTIVATED_ACCOUNT = 
        "*Należy najpierw aktywować konto. Wiadomość została wysłana na twój adres e-mail.";
    
    public const string QUIZ_NAME_REQUIRED_ERROR = "*Nazwa quizu jest wymagana.";
    public const string QUIZ_NAME_TOO_SHORT_ERROR = "*Podana nazwa quizu jest zbyt krótka.";
    public const string QUIZ_NAME_TOO_LONG_ERROR = "*Podana nazwa quizu jest zbyt długa.";

    public const string INVALID_COUPON_CODE_ERROR = "*Należy podać prawidłowy kupon.";
    public const string INACTIVE_COUPON_CODE_ERROR = "*Podany kupon jest już nieaktywny.";
    public const string USED_COUPON_CODE_ERROR = "*Podany kupon został już zużyty.";
    public const string INVALID_COUPON_EXPIRING_DATE_ERROR = 
        "*Należy podać prawidłową datę wygaśnięcia kuponu.";
    public const string INVALID_COUPON_EXTENSION_TIME_ERROR = 
        "*Należy podać prawidłowy okres przedłużenia subskrypcji przez kupon.";
    public const string INVALID_COUPON_AMOUNT_ERROR = 
        "*Należy podać prawidłową ilość kuponów.";
    public const string INVALID_COUPON_SUBSCRIPTION_TIME_ERROR = 
        "*Należy wybrać jeden z możliwych typów subskrypcji.";

    public const string COUPONS_GENERATED_INFO_STRING =
        "Wygenerowano <strong>{0}</strong> kluczy dla subkypcji ważnych do <strong>{1}</strong> " +
        "aktywujących subskypcję typu <strong>{2}</strong> na <strong>{3}</strong> dni.</br>";
}