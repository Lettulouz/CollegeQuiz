using System;
using System.Text;

namespace CollegeQuizWeb.Utils;

public static class Utilities
{
    private static readonly Random RANDOM = new Random();
    private static readonly string CHARACTERS = "abcdefghijklmnoprstquvwxyzABCDEFGHIJKLMNOPRSTQUWXYZ0123456789";
    
    public static String GenerateOtaToken(int length = 10)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            stringBuilder.Append(CHARACTERS[RANDOM.Next(CHARACTERS.Length)]);
        }
        return stringBuilder.ToString();
    }
}