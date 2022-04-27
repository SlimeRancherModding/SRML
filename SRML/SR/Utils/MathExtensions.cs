using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class MathExtensions
{
    #region Time
    public static bool HasReached(this double currentWorldTime, double targetWorldTime) => currentWorldTime >= targetWorldTime;

    /// <summary>
    /// Converts unix timestamp to <see cref="DateTime"/> with high precision.
    /// </summary>
    /// <param name="unixTime">Unix timestamp.</param>
    /// <returns>DateTime object that represents the same moment in time as provided Unix time.</returns>
    public static DateTime UnixTimestampToDateTime(this double unixTime)
    {
        var unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        var unixTimeStampInTicks = (long)(unixTime * TimeSpan.TicksPerSecond);
        return new DateTime(unixStart.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
    }

    /// <summary>
    /// Converts <see cref="DateTime"/> to unix timestamp with high precision
    /// </summary>
    /// <param name="dateTime">DateTime date representation.</param>
    /// <returns>unix timestamp that represents the same moment in time as provided DateTime object.</returns>
    public static double DateTimeToUnixTimestamp(this DateTime dateTime)
    {
        var unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        var unixTimeStampInTicks = (dateTime.ToUniversalTime() - unixStart).Ticks;
        return (double)unixTimeStampInTicks / TimeSpan.TicksPerSecond;
    }
    #endregion

    public static float Abs(this float value) => (double)value < 0.0 ? -value : value;

    public static bool Approx(this float v1, float v2)
    {
        float num = v1 - v2;
        return (double)num >= -1.0000000116861E-07 && (double)num <= 1.0000000116861E-07;
    }

    public static bool IsNotZero(this float value) => (double)value < -1.0000000116861E-07 || (double)value > 1.0000000116861E-07;

    public static bool IsZero(this float value) => (double)value >= -1.0000000116861E-07 && (double)value <= 1.0000000116861E-07;

    public static bool AbsoluteIsOverThreshold(this float value, float threshold) => (double)value < -(double)threshold || (double)value > (double)threshold;

    public static float NormalizeAngle(this float angle)
    {
        while ((double)angle < 0.0)
            angle += 360f;
        while ((double)angle > 360.0)
            angle -= 360f;
        return angle;
    }

    public static float Min(this float v0, float v1) => (double)v0 >= (double)v1 ? v1 : v0;

    public static float Max(this float v0, float v1) => (double)v0 <= (double)v1 ? v1 : v0;

    public static float Min(this float v0, float v1, float v2, float v3)
    {
        float num1 = (double)v0 < (double)v1 ? v0 : v1;
        float num2 = (double)v2 < (double)v3 ? v2 : v3;
        return (double)num1 >= (double)num2 ? num2 : num1;
    }

    public static float Max(this float v0, float v1, float v2, float v3)
    {
        float num1 = (double)v0 > (double)v1 ? v0 : v1;
        float num2 = (double)v2 > (double)v3 ? v2 : v3;
        return (double)num1 <= (double)num2 ? num2 : num1;
    }

    internal static float ValueFromSides(this float negativeSide, float positiveSide)
    {
        float v1 = negativeSide.Abs();
        float v2 = positiveSide.Abs();
        if (v1.Approx(v2))
            return 0.0f;
        return (double)v1 > (double)v2 ? -v1 : v2;
    }

    internal static float ValueFromSides(this float negativeSide, float positiveSide, bool invertSides) => invertSides ? positiveSide.ValueFromSides(negativeSide) : negativeSide.ValueFromSides(positiveSide);

    public static int NextPowerOfTwo(this int value)
    {
        if (value <= 0)
            return 0;
        --value;
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;
        ++value;
        return value;
    }

    public static long Floor(this double f) => (long)f;
    public static long Round(this double f) => (long)(f + 0.5d);
    public static long Ceil(this double f) => (long)(f + 1d);
    public static int Floor(this float f) => (int)f;
    public static int Round(this float f) => (int)(f + 0.5f);
    public static int Ceil(this float f) => (int)(f + 1f);

    public static double Pow(this double f, double p) => Math.Pow(f, p);
    public static int Pow(this int f, int p) => Convert.ToInt32(Pow(Convert.ToDouble(f), Convert.ToDouble(p)));
    public static long Pow(this long f, long p) => Convert.ToInt64(Pow(Convert.ToDouble(f), Convert.ToDouble(p)));
    public static float Pow(this float f, float p) => Convert.ToSingle(Pow(Convert.ToDouble(f), Convert.ToDouble(p)));

    /// <summary>
    /// Stop value from going above max or below min values.
    /// </summary>
    public static int Clamp(this int val, int min, int max)
    {
        if (val <= min)
            return min;

        if (val >= max)
            return max;

        return val;
    }

    /// <summary>
    /// Stop value from going above max or below min values.
    /// </summary>
    public static float Clamp(this float val, float min, float max)
    {
        if (val <= min)
            return min;

        if (val >= max)
            return max;

        return val;
    }

    /// <summary>
    /// Stop value from going above max or below min values.
    /// </summary>
    public static double Clamp(this double val, double min, double max)
    {
        if (val <= min)
            return min;

        if (val >= max)
            return max;

        return val;
    }

    /// <summary>
    /// Swap two reference values
    /// </summary>
    public static void Swap<T>(ref T a, ref T b)
    {
        T x = a;
        a = b;
        b = x;
    }

    /// <summary>
    /// Snap to grid of "round" size
    /// </summary>
    public static double Snap(this double val, double round)
    {
        return round * Math.Round(val / round);
    }

    /// <summary>
    /// Snap to grid of "round" size
    /// </summary>
    public static float Snap(this float val, float round) => (float)Snap((double)val, (double)round);

    /// <summary>
    /// Returns the sign 1/-1 evaluated at the given value.
    /// </summary>
    public static int Sign(IComparable x) => x.CompareTo(0);

    /// <summary>
    /// Value is in [0, 1) range.
    /// </summary>
    public static bool InRange01(this float value)
    {
        return InRange(value, 0, 1);
    }

    /// <summary>
    /// Value is in [closedLeft, openRight) range.
    /// </summary>
    public static bool InRange<T>(this T value, T closedLeft, T openRight)
        where T : IComparable =>
        value.CompareTo(closedLeft) >= 0 && value.CompareTo(openRight) < 0;

    /// <summary>
    /// Value is in [closedLeft, closedRight] range, max-inclusive.
    /// </summary>
    public static bool InRangeInclusive<T>(this T value, T closedLeft, T closedRight)
        where T : IComparable =>
        value.CompareTo(closedLeft) >= 0 && value.CompareTo(closedRight) <= 0;

    /// <summary>
    /// Clamp value to less than min or more than max
    /// </summary>
    public static float NotInRange(this float num, float min, float max)
    {
        if (min > max)
        {
            var x = min;
            min = max;
            max = x;
        }

        if (num < min || num > max) return num;

        float mid = (max - min) / 2;

        if (num > min) return num + mid < max ? min : max;
        return num - mid > min ? max : min;
    }

    /// <summary>
    /// Clamp value to less than min or more than max
    /// </summary>
    public static int NotInRange(this int num, int min, int max)
    {
        return (int)((float)num).NotInRange(min, max);
    }

    /// <summary>
    /// Return point A or B, closest to num
    /// </summary>
    public static float ClosestPoint(this float num, float pointA, float pointB)
    {
        if (pointA > pointB)
        {
            var x = pointA;
            pointA = pointB;
            pointB = x;
        }

        float middle = (pointB - pointA) / 2;
        float withOffset = num.NotInRange(pointA, pointB) + middle;
        return (withOffset >= pointB) ? pointB : pointA;
    }
}