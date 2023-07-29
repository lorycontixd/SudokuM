using System;

public sealed class DateTimeX
{
    /**
     * Default date time format used while converting date / time
     * to string.
     */
    public static String DEFAULT_DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
    /**
     * Gets current date and time;
     * @return Current date / time object.
     */
    public static DateTime getCurrDateTime()
    {
        return DateTime.Now;
    }
    /**
     * Gets string representation of current date time.
     *
     * @param dateFormat  Format to applied while conversion.
     * @return            String representation of current date and time.
     */
    public static String getCurrDateTimeStr(String dateFormat)
    {
        return getCurrDateTime().ToString(dateFormat);
    }
    /**
     * Gets string representation of current date time applying
     * default date and time format.
     *
     * @return            String representation of current date and time.
     */
    public static String getCurrDateTimeStr()
    {
        return getCurrDateTimeStr(DEFAULT_DATE_TIME_FORMAT);
    }

    private static readonly DateTime Jan1st1970 = new DateTime
        (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    /**
     * Current date/time in milliseconds
     * @return Number of milliseconds
     */
    public static long currentTimeMillis()
    {
        return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
    }
}