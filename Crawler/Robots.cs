using System.Text;

namespace Crawler;

// partial port of https://github.com/google/robotstxt/blob/master/robots.cc
public class Robots
{
    // Returns true if URI path matches the specified pattern. Pattern is anchored
    // at the beginning of path. '$' is special only at the end of pattern.
    //
    // Since 'path' and 'pattern' are both externally determined (by the webmaster),
    // we make sure to have acceptable worst-case performance.
    public static bool Matches(string path, string pattern)
    {
        int pathlen = path.Length;
        int patternlen = pattern.Length;

        int[] pos = new int[pathlen];

        int numpos = 1;

        // The pos[] array holds a sorted list of indexes of 'path', with length 'numpos'.
        // At the start and end of each iteration of the main loop below,
        // the pos[] array will hold a list of the prefixes of the 'path' which can
        // match the current prefix of 'pattern'. If this list is ever empty,
        // return false. If we reach the end of 'pattern' with at least one element
        // in pos[], return true.

        for (int patIndex = 0; patIndex < patternlen; patIndex++)
        {
            char pat = pattern[patIndex];

            if (pat == '$' && patIndex + 1 == patternlen)
            {
                return pos[numpos - 1] == pathlen;
            }

            if (pat == '*')
            {
                numpos = pathlen - pos[0] + 1;
                for (int i = 1; i < numpos; i++)
                {
                    pos[i] = pos[i - 1] + 1;
                }
            }
            else
            {
                // Includes '$' when not at end of pattern.
                int newnumpos = 0;
                for (int i = 0; i < numpos; i++)
                {
                    if (pos[i] < pathlen && path[pos[i]] == pat)
                    {
                        pos[newnumpos++] = pos[i] + 1;
                    }
                }
                numpos = newnumpos;
                if (numpos == 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    // Extracts path (with params) and query part from URL. Removes scheme,
    // authority, and fragment. Result always starts with "/".
    // Returns "/" if the url doesn't have a path or is not valid.
    public static string GetPathParamsQuery(string url)
    {
        // Initial two slashes are ignored.
        int searchStart = 0;
        if (url.Length >= 2 && url[0] == '/' && url[1] == '/')
        {
            searchStart = 2;
        }

        int earlyPath = url.IndexOfAny(['/', '?', ';'], searchStart);
        int protocolEnd = url.IndexOf("://", searchStart);

        if (earlyPath < protocolEnd)
        {
            // If path, param or query starts before ://, :// doesn't indicate protocol.
            protocolEnd = -1;
        }

        if (protocolEnd == -1)
        {
            protocolEnd = searchStart;
        }
        else
        {
            protocolEnd += 3;
        }

        int pathStart = url.IndexOfAny(['/', '?', ';'], protocolEnd);

        if (pathStart != -1)
        {
            int hashPos = url.IndexOf('#', searchStart);
            if (hashPos != -1 && hashPos < pathStart)
            {
                return "/";
            }

            int pathEnd = (hashPos == -1) ? url.Length : hashPos;

            if (url[pathStart] != '/')
            {
                // Prepend a slash if the result would start e.g. with '?'.
                return "/" + url.Substring(pathStart, pathEnd - pathStart);
            }

            return url.Substring(pathStart, pathEnd - pathStart);
        }

        return "/";
    }

    //
    // Canonicalize the allowed/disallowed paths. For example:
    //     /SanJoséSellers ==> /Sanjos%C3%A9Sellers
    //     %aa ==> %AA
    // When the function returns, (*dst) either points to src, or is newly
    // allocated.
    // Returns true if dst was newly allocated.
    public static bool MaybeEscapePattern(string src, out string dst)
    {
        int numToEscape = 0;
        bool needCapitalize = false;

        // First, scan the buffer to see if changes are needed. Most don't.
        for (int i = 0; i < src.Length; i++)
        {
            // (a) % escape sequence.
            if (src[i] == '%' &&
                Uri.IsHexDigit(src[i + 1]) && Uri.IsHexDigit(src[i + 2]))
            {
                if (char.IsLower(src[i + 1]) || char.IsLower(src[i + 2]))
                {
                    needCapitalize = true;
                }
                i += 2;
            }
            // (b) needs escaping.
            else if ((src[i] & 0x80) != 0)
            {
                numToEscape++;
            }
            // (c) Already escaped and escape-characters normalized (eg. %2f -> %2F).
        }

        // Return if no changes needed.
        if (numToEscape == 0 && !needCapitalize)
        {
            dst = src;
            return false;
        }

        StringBuilder sb = new StringBuilder(src.Length + numToEscape * 2);
        for (int i = 0; i < src.Length; i++)
        {
            // (a) Normalize %-escaped sequence (eg. %2f -> %2F).
            if (src[i] == '%' &&
                Uri.IsHexDigit(src[i + 1]) && Uri.IsHexDigit(src[i + 2]))
            {
                sb.Append(src[i++]);
                sb.Append(char.ToUpper(src[i++]));
                sb.Append(char.ToUpper(src[i]));
            }
            // (b) %-escape octets whose highest bit is set. These are outside the
            // ASCII range.
            else if ((src[i] & 0x80) != 0)
            {
                sb.Append(Uri.EscapeDataString(src[i].ToString()));
            }
            // (c) Normal character, no modification needed.
            else
            {
                sb.Append(src[i]);
            }
        }

        dst = sb.ToString();
        return true;
    }


}
