namespace NFM.Common;

public static class Log
{
    public static void Info(object? message)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"INFO: {message}");
        Console.ResetColor();
    }

    public static void Warn(object? message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"WARN: {message}");
        Console.ResetColor();
    }

    public static void Error(object? message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"ERROR: {message}");
        Console.ResetColor();
    }
}
