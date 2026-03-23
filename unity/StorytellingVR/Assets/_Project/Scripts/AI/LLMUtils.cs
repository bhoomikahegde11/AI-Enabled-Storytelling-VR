using System.Text.RegularExpressions;

public static class LLMUtils
{
    // Remove surrounding quotes, bracketed stage directions, and collapse spaces.
    // Speaker labels (e.g. "Rajan:") are intentionally preserved — we add them in PromptBuilder
    // and display them as-is in the dialogue box.
    public static string SanitizeDialogue(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return raw;
        string s = raw.Trim();

        // Trim matching leading/trailing quotes
        if ((s.StartsWith("\"") && s.EndsWith("\"")) || (s.StartsWith("'") && s.EndsWith("'")))
            s = s.Substring(1, s.Length - 2).Trim();

        // Remove bracketed stage directions: (laughs), [mutters], {aside}
        s = Regex.Replace(s, @"[\(\[\{].*?[\)\]\}]", "").Trim();

        // Collapse multiple spaces/newlines
        s = Regex.Replace(s, @"\s{2,}", " ");

        return s;
    }
}