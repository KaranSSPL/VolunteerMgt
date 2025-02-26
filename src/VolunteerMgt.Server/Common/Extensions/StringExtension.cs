namespace VolunteerMgt.Server.Common.Extensions;

public static class StringExtension
{
    public static TEnum? ToEnum<TEnum>(this string value, bool ignoreCase = false) where TEnum : struct
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return Enum.TryParse(value, ignoreCase, out TEnum result) ? result : null;
    }
}