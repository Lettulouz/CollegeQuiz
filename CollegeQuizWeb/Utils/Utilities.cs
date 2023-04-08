using System;
using System.Text;
using System.Text.Json;
using CollegeQuizWeb.Dto;
using Microsoft.AspNetCore.Http;

namespace CollegeQuizWeb.Utils;

public static class Utilities
{
    private static readonly Random RANDOM = new Random();
    private static readonly string CHARACTERS = "abcdefghijklmnoprstquvwxyzABCDEFGHIJKLMNOPRSTQUWXYZ0123456789";
    private static readonly string CHARACTERS2 = "ABCDEFGHIJKLMNOPRSTQUWXYZ";
    
    public static String GenerateOtaToken(int length = 10, int choice = 1)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            switch (choice)
            {
                case 1:
                    stringBuilder.Append(CHARACTERS[RANDOM.Next(CHARACTERS.Length)]);
                    break;
                case 2:
                    stringBuilder.Append(CHARACTERS2[RANDOM.Next(CHARACTERS2.Length)]);
                    break;
            }
            
        }
        return stringBuilder.ToString();
    }

    public static AlertDto? getSessionPropertyAndRemove(HttpContext context, string key)
    {
        string? jsonStringProperty = context.Session.GetString(key);
        if (jsonStringProperty == null) return null;
        context.Session.Remove(key);
        AlertDto? alertDto = JsonSerializer.Deserialize<AlertDto>(jsonStringProperty);
        return alertDto;
    }
    
}