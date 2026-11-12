using System.Text.RegularExpressions;

namespace Dsw2026Tpi.CrossCutting.Helpers;

public static class ValidationsExtensions
{
    public const string EmailPattern = @"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$";
    public static bool IsEmailValid(this string? email)
    {
        return !string.IsNullOrWhiteSpace(email) &&
            Regex.IsMatch(email, EmailPattern);
    }
    public static bool IsDniValid(this string? dni) 
    {
        return !string.IsNullOrWhiteSpace(dni) &&
            dni.All(char.IsDigit) &&
            dni.Length <= 8 &&
            dni.Length >= 8;
            
    }
}
