namespace CollegeQuizWeb.Utils;

public static class RegexApp
{
    public const string ALL_LETTERS_EXCEPT_NUM_AND_SPEC = @"^[^(\d\[\]!@#$%^&*()_=+{}`~<>/?\\\|;:""]+$";
    public const string ONLY_SMALL_LETTERS_NUM = @"^[a-z0-9]+$";
    public const string MIN_ONE_UPPER_LOWER_NUM_SPEC =
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
    public const string ALL_LETTERS_AND_NUM_EXC_SPECIAL = @"^[^(\[\]!#$%^&*()=+{}`~<>/?\\\|;:""]+$";
}