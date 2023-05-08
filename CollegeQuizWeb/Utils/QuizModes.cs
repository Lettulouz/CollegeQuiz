using System.Collections.Generic;
using System.Linq;

namespace CollegeQuizWeb.Utils;

public class QuizMode
{
    public string Title { get; set; }
    public string Slug { get; set; }
}

public static class QuizModes
{
    public const string SINGLE_FOUR_ANSWERS = "SINGLE_FOUR_ANSWERS";
    public const string TRUE_FALSE = "TRUE_FALSE";
    public const string MULTIPLE_FOUR_ANSWERS = "MULTIPLE_FOUR_ANSWERS";
    public const string SINGLE_SIX_ANSWERS = "SINGLE_SIX_ANSWERS";
    public const string RANGE = "RANGE";
    
    
    private static List<QuizMode> AllQuizModes { get; set; } = new List<QuizMode>()
    {
        new QuizMode() { Title = "4 odpowiedzi (jedna prawidłowa)", Slug = "SINGLE_FOUR_ANSWERS" },
        new QuizMode() { Title = "prawda/fałsz", Slug = "TRUE_FALSE" },
        new QuizMode() { Title = "4 odpowiedzi (wiele prawidłowych)", Slug = "MULTIPLE_FOUR_ANSWERS" },
        new QuizMode() { Title = "6 odpowiedzi (jedna prawidłowa)", Slug = "SINGLE_SIX_ANSWERS" },
        new QuizMode() { Title = "Zakres", Slug = "RANGE" },
    };

    public static List<QuizMode> GetModesForUserPacket(int userAccountState)
    {
        switch (userAccountState)
        {
            case 0: return GetForZeroPacket();
            case 1: return GetForFirstPacket();
            case 2: return GetForSecondPacket();
            default: return new List<QuizMode>();
        }
    }

    public static bool CheckIfUserHasPermissions(string slug, int userAccountState)
    {
        return GetModesForUserPacket(userAccountState).FirstOrDefault(m => m.Slug.Equals(slug)) != null;
    }

    public static string GetPermissionsMessage(int userAccountState)
    {
        string accountMode = string.Empty;
        switch (userAccountState)
        {
            case 0: accountMode = "podstawowego (FREE)"; break;
            case 1: accountMode = "płatnego (GOLD)"; break;
            case 2: accountMode = "płatnego (PLATINUM)"; break;
        }
        var modes = string.Join(", ", GetModesForUserPacket(userAccountState).Select(e => GetValueFromSlug(e.Slug)));
        return $"Posiadasz konto z pakietu <strong>{accountMode}</strong>. Umożliwia ono tworzenie następujących typów pytań " +
               $"quizu: <strong>{modes}</strong>";
    }

    public static string GetValueFromSlug(string slug)
    {
        var mode = AllQuizModes.FirstOrDefault(q => q.Slug.Equals(slug));
        if (mode == null) return string.Empty;
        return mode.Title;
    }
    
    private static List<QuizMode> GetForZeroPacket() => AllQuizModes.Where(m => m.Slug == SINGLE_FOUR_ANSWERS).ToList();
    private static List<QuizMode> GetForFirstPacket() => AllQuizModes.Where(m => m.Slug == SINGLE_FOUR_ANSWERS ||
        m.Slug == TRUE_FALSE || m.Slug == SINGLE_SIX_ANSWERS).ToList();
    private static List<QuizMode> GetForSecondPacket() => AllQuizModes;
}