namespace NFM.Common;

public static class Log
{
    public static void Info(object? message)
    {
        Write("INFO", message, ConsoleColor.Blue);
    }

    public static void Warn(object? message)
    {
        Write("WARN", message, ConsoleColor.Yellow);
    }

    public static void Error(object? message)
    {
        Write("ERROR", message, ConsoleColor.Red);
    }

    private static void Write(string level, object? message, ConsoleColor color)
    {
        string line = $"{level}: {message}";

        Console.ForegroundColor = color;
        Console.WriteLine(line);
        Console.ResetColor();
    }
}
