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
    
    public const string CATEGORYNAME_MUST_BE_UNIQUE = "*Podana kategoria już istnieje.";
    public const string CATEGORYNAME_IS_REQUIRED_ERROR = "*Pole kategoria jest wymagane.";
    public const string CATEGORYNAME_TOO_SHORT_ERROR = "*Podana kategoria jest za krótka.";
    public const string CATEGORYNAME_TOO_LONG_ERROR = "*Podana kategoria jest za długa.";
    public const string CATEGORYNAME_REGEX_ERROR = "*W kategorii powinny znaleźć się tylko małe litery oraz cyfry.";
    
    public const string EMAIL_IS_REQUIRED_ERROR = "*Pole Email jest wymagane.";
    public const string EMAIL_INCORRECT_ERROR = "*Wpisana treść nie jest emailem.";
    public const string EMAIL_TOO_LONG_ERROR = "*Podana treść nie spełnia założeń emaila.";
    
    public const string PASSWORD_IS_REQUIRED_ERROR = "*Pole Hasło jest wymagane.";
    public const string PASSWORD_TO_SHORT_ERROR = "*Podano za krótkie hasło (minimum 8).";
    public const string PASSWORD_TO_LONG_ERROR = "*Podano za długie hasło (maksimum 25).";
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
    
    public const string QUIZ_SHARED_TOKEN_ERROR = "Niepoprawny token.";
    public const string QUIZ_SHARED_TOKEN_BAD_ERROR = "Podany token nie istnieje lub jest nie prawidłowy.";
    public const string QUIZ_SHARED_TOKEN_DUPLICATE_ERROR = "Podany quiz należy już do Ciebie lub został już udostepniony.";
    public const string QUIZ_SHARED_TOKEN_SUCCESS= "Quiz został dodany do twojej listy quizów.";

    public const string INVALID_COUPON_CODE_ERROR = "Należy podać prawidłowy kupon.";
    public const string INACTIVE_COUPON_CODE_ERROR = "Podany kupon jest już nieaktywny.";
    public const string USED_COUPON_CODE_ERROR = "Podany kupon został już zużyty.";
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

    public const string COUPON_ACTIVATED_MESSAGE = "Sukces! Do Twojego konta przypisano właśnie subskrybcję " + 
                                                   "typu <strong>{0}</strong> na <strong>{1}</strong> dni. " +
                                                   "Aktualna subskrypcja będzie ważna jeszcze <strong>{2}</strong> dni, do <strong>{3}</strong>.";
    
    public const string COUPON_ACTIVATED_WITH_COMPENSATION_MESSAGE = 
        "Sukces! Do Twojego konta przypisano właśnie subskrybcję zmieniającą Twoje konto na status <strong>{0}" +
        "</strong> na <strong>{1}</strong> dni. Przekonwertowano również <strong>{2}</strong> dni aktualnego" +
        " pakietu na <strong>{3}</strong> dni nowego pakietu. Aktualna subskrypcja będzie ważna jeszcze " +
        "<strong>{4}</strong> dni, do <strong>{5}</strong>.";

    public const string COUPON_CODE_LOWER_LEVEL_ERROR =
        "Nie można aktywować kuponu z subskrypcją niższą niż obecnie posiadana!";

    public const string COUPON_CODE_FILL_CREATE_ERROR =
        "Należy wypełnić wszystkie wymagane pola.";

    public const string COUPON_CODE_CREATE_AMOUNT_ERROR = "*Niepoprawnie wypełniono pole Ilość kuponów.";
    public const string COUPON_CODE_CREATE_TILL_ERROR = "*Niepoprawnie wypełniono pole Ważne do.";
    public const string COUPON_CODE_CREATE_FOR_ERROR = "*Niepoprawnie wypełniono pole Przedłuża subskrypcję o.";
    
   public const string CATEGORIES_GENERATED_INFO_STRING =
        "Stworzono kategorię <strong>{0}</strong.";
        
    
    
    public const string USER_NOT_EXIST = "*Wybrany użytkownik nie istnieje.";
    public const string USER_DELETED = "*Użytkownik {0} został usunięty.";
    public const string BAN_ERROR = "*Należy zawiesić użytkownika permanentnie lub wybrać datę wygaśnięcia bana.";
    public const string CANNOT_SUSPEND_YOURSELF = "*Nie możesz zawiesić swojego konta.";
    public const string CANNOT_DELETE_YOURSELF = "*Nie możesz usunąć swojego konta.";
    public const string ACCOUNT_SUSPENDED = "*Twoje konto zostało zawieszone {0}";
    
    public const string USER_SUSPENDED = "*Użytkownik {0} został zawieszony {1}.";
    public const string USER_UNBAN = "*Użytkownik {0} został odblokowany.";
    public const string USER_ADDED = "*Pomyślnie utworzono użytkownika {0}";
    public const string PASS_REQUIRED = "*Aktywowane konto wymaga podania hasła.";
    public const string PASS_LEN_ERROR = "*Hasło powinno mieć pomiędzy 8 a 25 znaków.";
    public const string USER_UPDATED = "Użytkownik {0} został zaktualizowany.";
    
    public const string EMAIL_SENDING_ERROR = "Nieudane wysłanie wiadomości email na adres {0}. Spróbuj ponownie później.";
    public const string EMAIL_SENT = "Wysłano wiadomość email na adres {0}.";
    public const string EMAIL_ACCOUNT_CRETED_INFROMATION = "Tworzenie konta dla {0} {1} {2}.";
    public const string EMAIL_PASSWORD_RESET_INFROMATION = "Reset hasła dla {0} {1} {2}.";
    public const string EMAIL_PASSWORD_RESET_SENT = 
        "Na adres email <strong>{0}</strong> została wysłana wiadomość z linkiem umożliwiającym zmianę hasła.";
    public const string ACCOUNT_ACTIVATED_SUCCESSFULLY = "Pomyślnie aktywowano nowe konto. Możesz teraz się zalogować.";
    public const string ACCOUNT_ACTIVATION_LINK_EXPIRED = "Podany link nie istnieje, wygasł bądź został już wykorzystany.";
    public const string USER_NOT_FOUND = "Nie znaleziono użytkownika na podstawie przekazanych danych.";
    public const string ERROR_PASSWORD_DIFFERENCE = "Wartości w polach nowego hasła i potwórzonego hasła nie są takie same.";
    public const string ERROR_TOKEN = "Podany przez Ciebie kod <strong>{0}</strong> nie istnieje lub uległ przedawnieniu.";
    public const string PASSWORD_CHANGED = "Hasło do Twojego konta zostało pomyślnie zmienione.";
    public const string QUIZ_ALREADY_EXISTS = "Quiz o wybranej nazwie istnieje już na Twoim koncie. Wprowadź inną nazwę.";
    public const string QUIZ_ALREADY_EXISTS_NAME = "Quiz o nazwie <strong>{0}</strong> został pomyślnie utworzony.";
    public const string ACCOUNT_UPDATE_FOR = "Aktualizacja konta dla {0} {1} ({2})";

    public const string CATEGORY_REMOVED = "Kategoria {0} została usunięta.";
    
    public const string QUIZ_REMOVED = "Quiz {0} został usunięty.";
    public const string QUIZ_LOCKED = "Quiz {0} został zablokowany.";
    public const string QUIZ_UNLOCKED = "Quiz {0} został odblokowany.";
    public const string QUIZ_NOT_EXIST = "Quiz nie istnieje.";
    // subscription payment

    public const string INVALID_PHONE_NUMBER = "*Podano nieprawidłowy numer telefonu.";
    public const string COUNTRY_IS_REQUIRED = "*Pole Państwo jest wymagane.";
    public const string COUNTRY_TOO_LONG = "*Pole Państwo nie może przekraczać 40 znaków.";
    public const string COUNTRY_TOO_SHORT = "*Pole Państwo nie może być krótsze niż 3 znaki.";
    public const string STATE_IS_REQUIRED = "*Pole Województwo jest wymagane.";
    public const string STATE_TOO_LONG = "*Pole Województwo nie może przekraczać 40 znaków.";
    public const string STATE_TOO_SHORT = "*Pole Województwo nie może być krótsze niż 3 znaki.";
    public const string ADDRESS1_IS_REQUIRED = "*Pole Adres 1 jest wymagane.";
    public const string ADDRESS1_TOO_LONG = "*Pole Adres 1 nie może przekraczać 40 znaków.";
    public const string ADDRESS1_TOO_SHORT = "*Pole Adres 1 nie może być krótsze niż 3 znaki.";
    
    public const string EMAIL_CONFIRM_PAYMENT = "Potwierdzenie płatności dla {0} {1} {2}.";
    public const string SUBSCRIPTION_IS_ACTIVE = "Wygląda na to, że posiadasz już aktywną subskrypcję, " +
                                                 "dokonanie przedłużenia ważności konta premium jest możliwe na 7 dni " +
                                                 "przed zakończeniem ważności poprzedniej.";
    
    public const string SUBSCRIPTION_IS_LOWER = "Nie można kupić subskrypcji typu niższego niż aktualnie posiadany.";

    public const string PROFILE_UPDATED = "Pomyślnie zaktualizowano twój profil.";
    public const string DIFFERENT_PASSWORDS = "*Podano różne hasła.";
    
    public const string DELETE_QUIZ_NOT_FOUND = 
        "Quiz nie istnieje lub nie jest przypisany do Twojego konta. Możesz usunąc tylko quizy, które są przypisane do Twojego konta.";
    public const string SUCCESSFULL_DELETED_QUIZ = "Quiz <strong>{0}</strong> został pomyślnie usunięty.";
    public const string DISABLE_EDITABLE_QUIZ =
        "Edycja quizu, który jest aktualnie używany w aktywnej sesji nie jest możliwa.";
    public const string DISABLE_REMOVABLE_QUIZ = "Usunięcie quizu, który jest aktualnie używany w aktywnej sesji nie jest możliwe.";
    
    public const string DELETE_SHARED_QUIZ_NOT_FOUND = "Udostępniony quiz nie istnieje lub nie jest przypisany do Twojego konta.";
    public const string SUCCESSFULL_DELETED_SHARED_QUIZ = "Udostępniony quiz <strong>{0}</strong> został pomyślnie usunięty.";
    public const string DISABLE_DELETE_SHARED_QUIZ =
        "Usunięcie udostępnionego quizu, który jest aktualnie używany w aktywnej sesji nie jest możliwe.";

    public const string PAYU_PENDING = "W toku";
    public const string PAYU_WAITING = "Oczekuje";
    public const string PAYU_COMPLETED = "Zakończona";
    public const string PAYU_CANCELED = "Anulowana";
    
    public const string SUB_UPDATED = "Pakiet {0} został zaktualizowamy.";
    public const string SUB_ERROR = "Pola nazwa i cena nie mogą być puste.";
    
    public const string HOST_DISCONECTED = "Host zakończył sesję.";
    public const string HOST_ERROR = "Host gry nie może jednocześnie być hostem i brać w niej udziału.";
    public const string QUIZ_NOT_FOUND = "Nie znaleziono quizu przypisanego do Twojego konta.";
    public const string QUESTIONS_REPEATED = "Pytania w edytowanym quizie nie mogą się powtarzać.";
    public const string PACKAGE_DOES_NOT_ALLOW = "Twoje konto nie posiada wykupionego pakietu umożliwiającego wybór " +
                                                 "trybu <strong>{0}</strong> rozgrywki.";
    public const string GIVEN_VALUES_NOT_NUMBERS = "Podane wartości czasu nie są liczbami.";
    public const string WRONG_TIME_INTERVALS = "Wartość sekund nie może być mniejsza od 5 i większa od 59.";
    public const string WRONG_QUESTION_TYPE = "Nie znaleziono typu pytania.";
    public const string MIN_MAX_DIFFERENCE = "Odpowiedź minimum i maksimum może dzielić do 20 wartości przejścia.";
    public const string QUESTION_REPEATED = "Odpowiedzi w pytaniu <strong>{0}</strong> nie mogą być takie same.";
    public const string QUIZ_NAME_UPDATED = "Quiz o nazwie <strong>{0}</strong> został pomyślnie zaktualizowany.";
    public const string QUIZ_NAME_REPEATED = "Quiz o nazwie <strong>{0}</strong> istnieje już na Twoim koncie. Podaj inną nazwę.";
    public const string QUIZ_NAME_UPDATED2 = "Nazwa quizu o nazwie <strong>{0}</strong> została pomyślnie zmieniona na <strong>{1}</strong>.";
    public const string IMAGE_ERROR = "Wystąpił problem z załadowaniem grafiki.";
    public const string IMAGE_ACCEPTED_EXTENSIONS = "Akceptowane rozszerzenia pliku to <strong>{0}</strong>.";
    public const string CURRNETLY_IN_GAME = "Na tym koncie obecnie prowadzona jest rozgrywka w sesji. Użyj innego konta.";
    public const string CURRNETLY_NOT_IN_GAME = "Obecnie nie jesteś w wybranej grze.";
    public const string SESSION_LEFT_ENTER_TOKEN = "Wyszedłeś z sesji, aby wejść ponownie wprowadź kod quizu.";
    public const string SESSION_REMOVE_USER = "Użytkownik <strong>{0}</strong> został wyrzucony przez ciebie z sesji.";
    public const string SESSION_BAN_USER = "Użytkownik <strong>{0}</strong> został zbanowany przez ciebie z sesji.";
    public const string SESSION_UNBAN_USER = "Użytkownik <strong>{0}</strong> został odbanowany przez ciebie z sesji.";
    public const string HOST_NOT_FOUND = "Nie znaleziono aktywnego hosta sesji gry. Odśwież stronę, aby ponowić wyszukiwanie.";
    public const string HOST_DISCONECT_USER = "Zostałeś rozłączony przez hosta.";
    public const string HOST_BANNED_USER = "Zostałeś zbanowany przez hosta, przez co nie możesz dołączyć już do quizu.";
    
    public const string CANNOT_DELETE_USER_IN_GAME = "Nie można usunąć użytkownika znajdującego się w rozgrywce.";

    public const string RANGE_QST_MIN_INVALID =
        "Wartość minimalna nie może być większa od wartości maksymalnej w pytaniu <strong>{0}</strong>.";
    public const string RANGE_QST_COUNTED_MIN_INVALID =
        "Wartość minimalna punktowana nie może być więszka od wartości maksymalnego w pytaniu <strong>{0}</strong>.";
    public const string RANGE_QST_COUNTED_OUT_OF_RANGE = "Wartość punktowana w pytaniu <strong>{0}</strong> wykracza poza zakres.";
    public const string RANGE_QST_STEP_INVALID =
        "Wartość przejścia w pytaniu <strong>{0}</strong> musi być dzielnikiem pozostałych wartości.";
    public const string RANGE_QST_CORRECT_QST_INVALID =
        "Nieprawidłowa wartość prawidłowej odpowiedzi w pytaniu <strong>{0}</strong>.";
    
    public const string SELECTED_QUIZ_IS_ALREADY_HOSTED = "Wybrany quiz <strong>{0}</strong> jest już hostowany na tym koncie.";
}