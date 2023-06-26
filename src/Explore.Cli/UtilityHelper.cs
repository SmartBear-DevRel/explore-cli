using System.Text.RegularExpressions;

public static class UtilityHelper
{
    public static string CleanString(string? inputName)
    {
        if(string.IsNullOrEmpty(inputName))
        {
            return string.Empty;
        }

        try 
        {
           return Regex.Replace(inputName, @"[^a-zA-Z0-9 ._-]", "", RegexOptions.None, TimeSpan.FromSeconds(2));
        }
        // return empty string rather than timeout
        catch (RegexMatchTimeoutException) {
           return String.Empty;
        }
    }
}