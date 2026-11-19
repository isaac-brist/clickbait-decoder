using System.Text.RegularExpressions;
using ClickbaitDecoder.Story;

namespace ClickbaitDecoder.Services;

/// <summary>
/// A deliberately simple, rule-based phrase scanner. It is NOT a fact checker: it finds wording
/// that deserves a second look and turns the article into the same clickable "Layer" the case study uses.
/// It will miss things and it will flag innocent sentences. The UI says so.
/// </summary>
public static class RedFlagScanner
{
    sealed record Rule(Regex Regex, string Name, string Why, Func<Match, bool>? Accept = null);

    static Rule R(string pattern, string name, string why, Func<Match, bool>? accept = null) =>
        new(new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant), name, why, accept);

    static readonly Rule[] Rules =
    [
        R(@"\b(proves?|proven|proof)\b", "\"Prove\"",
          "One study rarely proves anything. Look for what the researchers themselves claimed."),
        R(@"\b(cures?|miracle|breakthrough|game[- ]changer|revolutionary|superfood)\b", "Hype word",
          "Big, emotional words sell clicks. Real findings are usually smaller and duller."),
        R(@"\b(shocking|bombshell|you won'?t believe|one weird trick|doctors hate|secret)\b", "Pure clickbait",
          "This phrase exists to make you click, not to inform you."),
        R(@"\b(experts|scientists|researchers|doctors|studies)\s+(say|says|warn|claim|believe|agree|show)\b", "Which experts?",
          "Unnamed experts or 'studies show'. Who exactly, and can you find the actual study?"),
        R(@"\b\d+(\.\d+)?\s?(%|percent)\s+(lower|higher|less|more|greater|increased?|decreased?|reduc\w*|drop|rise|risk)\b", "Percent of what?",
          "A relative percentage. Ask for the real numbers: how many people out of how many?"),
        R(@"\b(doubles?|triples?|halves|slashes|cuts?|boosts?|skyrockets?)\b\s+(your\s+|the\s+)?(risk|chances?|odds)\b", "Percent of what?",
          "'Doubles your risk' sounds scary, but doubling 1 in 10,000 is 2 in 10,000. Find the baseline."),
        R(@"\b(linked (to|with)|associated with|tied to|connected to|correlated with)\b", "Link, not cause",
          "This usually means an observational study: it found a pattern, not proof that one thing causes the other."),
        R(@"\bin (mice|rats|animals|cells|a lab|test tubes?|petri dish(es)?)\b", "Not in humans",
          "Lots of things work in mice and never work in people."),
        R(@"\b(?<n>\d{1,3})\s+(people|participants|patients|adults|volunteers|subjects|women|men|children|students)\b", "Small sample?",
          "Fewer than 100 people is a small study. Results from small groups bounce around a lot.",
          m => int.TryParse(m.Groups["n"].Value, out var n) && n is > 1 and < 100),
        R(@"\b(funded|sponsored|paid for|supported|backed) by\b", "Who paid?",
          "Check who paid for the research and whether they gain from the result. That doesn't make it wrong."),
        R(@"\b(preprint|not (yet )?(been )?peer[- ]reviewed)\b", "Not checked yet",
          "Peer review is when other scientists check the work before publication. This one hasn't been through it yet."),
        R(@"\b(suggests?|preliminary|early (study|results|findings)|small study|more research is needed)\b", "Hedge word (good sign!)",
          "Careful language. Notice whether the headline kept this caution or dropped it."),
    ];

    /// <summary>Turns a headline + paragraphs into one clickable layer with flags attached to suspicious phrases.</summary>
    public static Layer Scan(string headline, IEnumerable<string> paragraphs, string caption)
    {
        var blocks = new List<Block> { new("h", Segmentize(headline)) };
        blocks.AddRange(paragraphs.Select(p => new Block("p", Segmentize(p))));
        return new Layer("Your Article", caption, blocks);
    }

    static List<Segment> Segmentize(string text)
    {
        // Every rule match, earliest first; on overlap the longer match wins.
        var hits = Rules
            .SelectMany(r => r.Regex.Matches(text)
                .Where(m => r.Accept?.Invoke(m) ?? true)
                .Select(m => (m.Index, m.Length, Rule: r)))
            .OrderBy(h => h.Index).ThenByDescending(h => h.Length);

        var segments = new List<Segment>();
        var pos = 0;
        foreach (var (index, length, rule) in hits)
        {
            if (index < pos) continue; // overlaps an earlier flag
            if (index > pos) segments.Add(new Segment(text[pos..index]));
            segments.Add(new Segment(text.Substring(index, length), new Flag(rule.Name, rule.Why)));
            pos = index + length;
        }
        if (pos < text.Length) segments.Add(new Segment(text[pos..]));
        return segments;
    }
}
