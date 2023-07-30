using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class StringExtensions
{
    public static string Capitalize(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return input.Substring(0, 1).ToUpper(CultureInfo.CurrentCulture) +
            input.Substring(1, input.Length - 1);
    }
}
