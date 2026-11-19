namespace ClickbaitDecoder.Story;

// A red flag the newcomer can discover by clicking a phrase.
// A class (reference equality) on purpose: the scanner can produce several "linked to" flags
// with identical text, and each one should be found separately.
public sealed class Flag(string name, string why)
{
    public string Name { get; } = name;
    public string Why { get; } = why;
}

// One clickable run of text. Flag == null means "nothing fishy here" (a decoy).
public sealed record Segment(string Text, Flag? Flag = null);

// A paragraph-ish chunk of a layer. Kind: "h" (headline), "p" (body), "small" (fine print).
public sealed record Block(string Kind, IReadOnlyList<Segment> Segments);

// One link in the telephone chain, from loudest (headline) to quietest (fine print).
public sealed record Layer(string Kicker, string Caption, IReadOnlyList<Block> Blocks)
{
    public IEnumerable<Flag> Flags =>
        Blocks.SelectMany(b => b.Segments).Select(s => s.Flag).OfType<Flag>();
}

/// <summary>
/// The invented study. Everything here (university, people, numbers, funder) is fictional,
/// written to teach how claims change as they travel from a paper to a headline.
/// </summary>
public static class CaseStudy
{
    static Segment T(string text) => new(text);
    static Segment F(string text, string name, string why) => new(text, new Flag(name, why));
    static Block H(params Segment[] s) => new("h", s);
    static Block P(params Segment[] s) => new("p", s);
    static Block Small(params Segment[] s) => new("small", s);

    public const string HonestHeadline =
        "Small early study: coffee drinkers at one gym had slightly fewer colon cancers (3 vs 5 out of 600 each). " +
        "Could be the exercise. Needs a bigger study.";

    public static readonly IReadOnlyList<Layer> Layers =
    [
        new("The Headline",
            "You're scrolling. This stops you cold.",
            [
                H(F("SCIENTISTS PROVE", "\"Prove\"",
                      "One study doesn't prove anything. It adds a bit of evidence. Be wary of words like 'proves', 'cure' and 'miracle'."),
                  T(": "),
                  F("COFFEE SLASHES CANCER RISK BY 40%!", "40% of what?",
                      "A relative number with no starting point. 40% off a tiny risk is still a tiny change. Keep digging for the real numbers.")),
                P(T("Drink "),
                  F("four cups a day", "Advice nobody gave",
                      "The study never recommended an amount. That advice got invented somewhere along the telephone chain."),
                  T(" to stay safe, "),
                  F("experts say", "Which experts?",
                      "Unnamed 'experts' are a red flag. Who said it, and did they actually run the study?"),
                  T(". Doctors HATE this one weird trick!")),
                Small(T("Shared 48,000 times · 2 min read")),
            ]),

        new("The Press Release",
            "MEANWHILE, at the university press office...",
            [
                H(T("Coffee drinkers may have lower colon cancer rates, early study suggests")),
                P(T("NORTHFIELD UNIVERSITY — Researchers at the School of Public Health found that adults who drank four or more cups of coffee daily "),
                  F("may have", "The hedge word vanished",
                      "The press release says 'may'. The headline said 'PROVE'. Hedge words like 'may', 'suggests' and 'linked to' get dropped as a story travels."),
                  T(" a lower rate of "),
                  F("colon cancer", "One cancer, not all cancer",
                      "The study was only about colon cancer. The headline quietly turned that into 'cancer'."),
                  T(" than adults who drank none.")),
                P(T("\"These findings are "),
                  F("exciting but preliminary", "Even the author is cautious",
                      "The lead researcher called it preliminary. That caution never made it into the headline."),
                  T(",\" said lead author Dr. Morgan Roast. \"We'd love to see larger studies.\"")),
            ]),

        new("The Abstract",
            "The study's own 200-word summary. Already fewer exclamation points.",
            [
                H(T("Coffee consumption and colon cancer incidence: a six-year cohort study")),
                P(T("Methods: We "),
                  F("followed 1,200 adults", "Watching, not testing",
                      "Researchers just watched what people already did. Nobody was assigned to drink coffee. That can show a link (correlation), but it can't show that coffee causes anything."),
                  T(" aged 50–70 for six years. Coffee intake was "),
                  F("self-reported on a single questionnaire", "Memory is fuzzy",
                      "One survey at the start, filled in from memory, and never checked again in six years. Habits change and people misremember."),
                  T(" at enrollment.")),
                P(T("Results: Colon cancer occurred in "),
                  F("3 of 600 heavy coffee drinkers (0.5%) versus 5 of 600 non-drinkers (0.83%)", "There's the 40%",
                      "0.5% vs 0.83% is where the '40% lower' comes from. In real people that's 3 vs 5, about 3 fewer cases per 1,000 people. Same data, much less dramatic."),
                  T(" (relative risk 0.60, "),
                  F("p = 0.04", "Barely over the line",
                      "Scientists often treat p < 0.05 as 'probably not a fluke'. 0.04 just squeaks in, and with only 8 cases in total, a single person could flip the result."),
                  T(").")),
                P(T("Conclusion: Higher coffee intake was associated with lower colon cancer incidence in this cohort.")),
            ]),

        new("The Methods",
            "The part nobody reads. Which is exactly why we're here.",
            [
                H(T("2. Methods")),
                P(T("Participants were "),
                  F("recruited from members of a regional fitness club", "Not a typical crowd",
                      "Gym members aren't typical adults. What's true for them may not hold for everyone else."),
                  T(". At baseline, "),
                  F("coffee drinkers were more likely to exercise daily and less likely to smoke", "The hidden third factor",
                      "The coffee drinkers also exercised more and smoked less. Maybe that's what lowered their risk. A hidden factor like this is called a confounder."),
                  T(".")),
                P(T("Models were "),
                  F("adjusted for age only", "Knew it, didn't fix it",
                      "They saw the differences in exercise and smoking but only corrected for age, so the confounder is still mixed into the result."),
                  T(". We examined "),
                  F("14 cancer types; colon cancer was the only one with a significant difference", "Fourteen tries",
                      "Test 14 things and one will often look 'significant' by luck alone. This is called multiple comparisons. Reporting only the hit is called cherry-picking."),
                  T(".")),
            ]),

        new("The Fine Print",
            "Funding, conflicts, limitations. Small font, big deal.",
            [
                H(T("Declarations")),
                P(T("Funding: "),
                  F("Supported by a grant from the Global Bean Council", "Who paid?",
                      "A coffee-industry group paid for the study. That doesn't make it wrong, but it's worth knowing who benefits from the result."),
                  T(".")),
                P(T("Competing interests: "),
                  F("two authors have received consulting fees from coffee producers", "Conflict of interest",
                      "Two of the researchers are paid by the coffee industry. They disclosed it (good!), but it's a reason to look more closely."),
                  T(".")),
                P(T("Limitations: small number of cases. "),
                  F("Findings should be confirmed in a larger randomized trial", "The authors' own warning",
                      "The researchers themselves say this needs a bigger, better study. That's a long way from 'SCIENTISTS PROVE'."),
                  T(".")),
                P(T("Status: "),
                  F("preprint, not yet peer reviewed", "Not checked yet",
                      "Peer review is when other scientists check the work before a journal publishes it. This study hasn't been through that yet.")),
            ]),
    ];

    // ---- "This really happened" card -----------------------------------------------------------
    // Facts checked 2026-11-18 against John Bohannon's own account (io9/Gizmodo, 27 May 2015).
    // Paraphrased on purpose; keep any changes faithful to that source.
    public const string RealCaseSourceTitle = "I Fooled Millions Into Thinking Chocolate Helps Weight Loss. Here's How. (John Bohannon, io9/Gizmodo, 2015)";
    public const string RealCaseSourceUrl = "https://gizmodo.com/i-fooled-millions-into-thinking-chocolate-helps-weight-1707251800";

    public const string RealCaseIntro =
        "In 2015, science journalist John Bohannon ran a real but deliberately terrible diet trial to see whether the press would notice. " +
        "The result: \"chocolate helps you lose weight 10% faster\". Germany's Bild put it on the front page, and Bohannon says it " +
        "spread to more than 20 countries.";

    // (what happened, which red flag from the game it matches)
    public static readonly (string Fact, string Flag)[] RealCaseFacts =
    [
        ("16 people showed up and 15 were analyzed, split into 3 groups, for 21 days.", "Small sample"),
        ("They measured 18 different things (weight, cholesterol, sleep, well-being...) and reported the ones that came out 'significant'. Bohannon put the odds of at least one fluke at about 60%.", "Fourteen tries"),
        ("The 'Institute of Diet and Health' behind it was just a website.", "Which experts?"),
        ("A journal accepted it for a fee of about €600, with no real peer review.", "Not checked yet"),
        ("Reporters ran the press release without asking how many people, how big the effect, or what outside experts thought.", "The telephone game"),
    ];

    // ---- "Learn more" links ------------------------------------------------------------------------
    // Each page fetched and checked on 2026-11-18: free to read, no login, and right for the stated level.
    public sealed record LearnLink(string Title, string Who, string Blurb, string Url);

    public static readonly LearnLink[] LearnMore =
    [
        new("Reading Science Info", "Cornell University Library",
            "A short, free starting point: a beginner video on reading scientific articles, plus tips for judging science news.",
            "https://guides.library.cornell.edu/evaluate/readsci"),
        new("Correlation does not imply causation", "Wikipedia",
            "The most common headline mistake, explained with examples.",
            "https://en.wikipedia.org/wiki/Correlation_does_not_imply_causation"),
        new("Assessing contemporary science", "The Open University (OpenLearn)",
            "Ready to go deeper? A free course on judging science in the news. Intermediate, about 9 hours, readable without signing up.",
            "https://www.open.edu/openlearn/science-maths-technology/assessing-contemporary-science/content-section-0"),
    ];

    public static readonly string[] Checklist =
    [
        "Did the source actually say that? (hedge words, which cancer, who's quoted)",
        "How many people, and what are the real numbers, not just the percent?",
        "Did they watch people, or run an experiment? A link isn't a cause.",
        "What else could explain it? (gym members, exercise, smoking)",
        "Who paid for it, and has anyone checked the work yet?",
    ];
}
