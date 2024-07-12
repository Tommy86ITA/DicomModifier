// Interfaces/Logger.cs

using System.Diagnostics;

public static class Logger
{
    public static void Log(string message)
    {
        Debug.WriteLine($"[{DateTime.Now}] {message}");
    }
}
