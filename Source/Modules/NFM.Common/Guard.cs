using System;
using System.Diagnostics.CodeAnalysis;

namespace NFM.Common;

public static class Guard
{
    public static bool Require(bool value, string? message = null)
    {
        if (!value)
        {
            throw new GuardException(message ?? "Got unexpected false");
        }

        return value;
    }

    public static T NotNull<T>([NotNull] T? obj, string? message = null)
        where T : class
    {
        if (obj == null)
        {
            throw new GuardException(message ?? "Got unexpected null");
        }

        return obj;
    }

    public static string NotNullOrWhitespace([NotNull] string? obj, string? message = null)
    {
        if (obj == null)
        {
            throw new GuardException(message ?? "Got unexpected null");
        }

        if (string.IsNullOrWhiteSpace(obj))
        {
            throw new GuardException(message ?? "Got unexpected whitespace");
        }

        return obj;
    }
}

class GuardException : Exception
{
    public GuardException(string message)
        : base(message) { }
}
