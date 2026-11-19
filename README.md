# Clickbait Decoder

**An interactive Blazor explainer that teaches newcomers to read past science headlines:** dig from a tabloid
headline down to the fine print, hunt for red flags, watch your trust change, see a real-world case, then scan
a real article.

Built for the dev-meeting build challenge. Draw: *Wild Card > Explain it to a newcomer*. 45-minute timebox, with Claude Code.

## Run it

```powershell
dotnet run --project src/ClickbaitDecoder --launch-profile http
```

Open http://localhost:5002. Or open `ClickbaitDecoder.slnx` in Visual Studio and press F5.

Requires the .NET 10 SDK. The comic fonts load from Google Fonts. Offline, the page falls back to Impact and Comic Sans.

## What's in it

| Path | What it does |
|---|---|
| `src/ClickbaitDecoder/Story/CaseStudy.cs` | The invented coffee study (5 layers, 19 flags), the honest headline, the checklist, and the real chocolate-hoax card |
| `src/ClickbaitDecoder/Components/LayerView.razor` | One clickable "page" of the hunt: flags, POW bursts, speech bubbles, trust slider. Shared by both pages |
| `src/ClickbaitDecoder/Components/Pages/Home.razor` | The guided walkthrough: telephone chain, layers, verdict chart |
| `src/ClickbaitDecoder/Components/Pages/Scan.razor` | "Bring your own article": paste a URL or the article text |
| `src/ClickbaitDecoder/Services/ArticleFetcher.cs` | Downloads a public page and pulls out the headline and paragraphs (regex, no extra packages) |
| `src/ClickbaitDecoder/Services/RedFlagScanner.cs` | Rule-based phrase scanner that produces the same clickable layer |
| `src/ClickbaitDecoder/Components/LearnMore.razor` | "Want to get good at this?" links (Cornell, Wikipedia, Open University), on the verdict and scanner pages |
| `src/ClickbaitDecoder/Components/SoundToggle.razor` + `Services/Sfx.cs` + `wwwroot/sfx.js` | Synthesized comic sound effects (Web Audio, no audio files) with a 🔊/🔇 toggle that's remembered per browser |
| `src/ClickbaitDecoder/wwwroot/decoder.js` | Keeps the newest bubble in view in the sticky side column |
| `src/ClickbaitDecoder/wwwroot/decoder.css` | Tabloid-to-journal styling and comic effects: shake, stamp, confetti, self-drawing chart |

### The extras

- **POW bursts + screen shake** on every flag found. Find flags within 2.5 seconds of each other for **2x / 3x COMBO!**
- **Rubber stamp** when a page is cleared: *DEBUNKED! / BUSTED! / FACT-CHECKED!* if you found them all, a grey
  *CASE CLOSED* if you used "give up".
- **Bubbles appear newest first**, and the side column **follows you down** long scanned articles.
- **Verdict:** the trust line draws itself, you get a rank (Rookie Reader → Master Debunker), and confetti fires:
  a full shower at 75%+ of flags found, a small sprinkle otherwise.
- **Sounds:** pow, boing (miss), combo arpeggio, stamp thud, whoosh between layers, ta-da at the verdict. Respects
  the 🔊/🔇 toggle. Animations shorten automatically for people with *reduce motion* switched on.

## Demo script (about 65 seconds)

> **(0:00)** "My draw was *Explain it to a newcomer*. My topic: how to tell if a science headline is lying to you.
> It's called **Clickbait Decoder**."
>
> **(0:07)** *(Headline screen)* "SCIENTISTS PROVE coffee slashes cancer risk by 40%. Up top is the telephone chain:
> the real study is four layers down. Every phrase here is a suspect, so click what looks fishy…"
>
> **(0:13)** *(Rapid-fire, under 2.5 seconds between clicks: "SCIENTISTS PROVE", "COFFEE SLASHES…40%!",
> "four cups a day", "experts say". You get POW!, 2x, 3x, 4x COMBO! with a shake each time, then the stamp
> slams down.)* "…and if you're fast, it's a combo. Four for four, and the page gets stamped." *(Point at a
> bubble)* "Each find explains itself: one study doesn't *prove* anything, and 40% of *what*?"
>
> **(0:22)** *(Dig deeper twice, whoosh)* "The page calms down as we dig: tabloid, press release, abstract. 'Prove' has
> already become 'may'. And here's the 40%…" *(Click the 3-of-600 phrase)* "…3 people versus 5."
>
> **(0:32)** *(Dig to Methods, click the confounder)* "The coffee drinkers were gym members who exercised more.
> That's the hidden third factor." *(Jump to the Verdict, ta-da)* "My trust line draws itself falling at every
> layer, and here's the headline it should have been."
>
> **(0:44)** *(Scroll to TRUE STORY)* "Think nobody falls for this? In 2015 a journalist ran a deliberately terrible
> chocolate diet study to test the press. Fifteen people, eighteen measurements, a fake institute, a
> pay-to-publish journal. It made the front page of Germany's biggest tabloid. Every flag you just learned is
> in it."
>
> **(0:56)** *(Click "Now try a real article")* "And you can scan your own article. It's a keyword scanner, not
> a fact checker, and it says so. Blazor Server on .NET 10, built in 45 minutes with Claude."

**About the combo beat:** the stamp word is random (*DEBUNKED!*, *BUSTED!* or *FACT-CHECKED!*), so say "stamped",
not a specific word. A pause of more than 2.5 seconds resets the combo, so click all four first and explain
afterwards. If you miss one, the stamp still lands when the fourth flag is found; it just won't be a 4x.

**If time is tight:** skip the live scan and end on the TRUE STORY card. **If there's time:** paste a
pre-tested article URL into the scanner and click "give up & show me".

**Presenter tips**
- Let the page sit for a second after loading before you click. Blazor Server connects right after the first
  render, and anything typed before that gets reset.
- **Check the room audio first.** The sounds play through whatever the laptop outputs to. Use 🔊/🔇 (top right)
  to mute, and it stays muted for that browser.
- **For the combo and stamp moment:** on the headline page, click the four flags quickly (SCIENTISTS PROVE,
  COFFEE SLASHES…, four cups a day, experts say) to get a combo and then the DEBUNKED stamp.
- Have a backup article on your clipboard. Many news sites block fetches or sit behind paywalls, and the paste
  box is the fallback.

**One-liner:** *An interactive Blazor explainer that teaches newcomers to read past science headlines: dig from
tabloid headline to fine print, hunt red flags, watch your trust change, then scan a real article.*

## Honesty notes and assumptions

- **The coffee study is invented**: the university, researchers, funder and numbers. It's labelled in the app.
  No SRC data and no coil engineering anywhere.
- **The chocolate hoax is real.** The facts were checked on 2026-11-18 against John Bohannon's own account:
  ["I Fooled Millions Into Thinking Chocolate Helps Weight Loss. Here's How." (io9/Gizmodo, 2015)](https://gizmodo.com/i-fooled-millions-into-thinking-chocolate-helps-weight-1707251800).
  The "millions" is his claim, and other journalists disputed it
  ([Faye Flam, NASW](https://www.nasw.org/node/254369)). The card says so.
- **The learn-more links** were each fetched and checked on 2026-11-18. All three are free with no login. The
  Open University course is labelled intermediate (about 9 hours), so it's presented as the go-deeper option.
- **The scanner is regex rules**: it misses things and sometimes flags innocent sentences. Any "suggests" counts
  as a hedge word.
- **The fetcher refuses private and loopback addresses** with a DNS check, but redirects aren't re-checked.
  That's fine for a local demo, not for hosting.
