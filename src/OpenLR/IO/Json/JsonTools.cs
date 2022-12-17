using System;
using System.Text;

namespace OpenLR.IO.Json;

/// <summary>
/// Contains some tools for json.
/// </summary>
internal static class JsonTools
{
    /// <summary>
    /// Escape a string.
    /// </summary>
    public static string Escape(string? s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return "";
        }

        char c = '\0';
        int i;
        int len = s.Length;
        var sb = new StringBuilder(len + 4);
        string t;

        for (i = 0; i < len; i += 1)
        {
            c = s[i];
            switch (c)
            {
                case '\\':
                case '"':
                    sb.Append('\\');
                    sb.Append(c);
                    break;
                case '/':
                    sb.Append('\\');
                    sb.Append(c);
                    break;
                case '\b':
                    sb.Append("\\b");
                    break;
                case '\t':
                    sb.Append("\\t");
                    break;
                case '\n':
                    sb.Append("\\n");
                    break;
                case '\f':
                    sb.Append("\\f");
                    break;
                case '\r':
                    sb.Append("\\r");
                    break;
                default:
                    if (c < ' ')
                    {
                        t = "000" + string.Format("X", c);
                        sb.Append("\\u" + t[^4..]);
                    }
                    else {
                        sb.Append(c);
                    }
                    break;
            }
        }
        return sb.ToString();
    }
}
