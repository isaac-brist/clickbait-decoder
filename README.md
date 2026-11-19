<div align="center">

# Clickbait Decoder

**Read past the headline.**<br>
An interactive, comic-book explainer that teaches newcomers how to tell whether a science headline is lying to them.

[![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Blazor Server](https://img.shields.io/badge/Blazor-Interactive%20Server-512BD4?style=for-the-badge&logo=blazor&logoColor=white)](https://learn.microsoft.com/aspnet/core/blazor/)
[![C#](https://img.shields.io/badge/C%23-Razor-239120?style=for-the-badge)](https://learn.microsoft.com/dotnet/csharp/)
<br>
[![Plain CSS (no framework)](https://img.shields.io/badge/CSS-no%20framework-E4002B?style=for-the-badge&logo=css&logoColor=white)](src/ClickbaitDecoder/wwwroot/decoder.css)
[![Web Audio](https://img.shields.io/badge/Sound-Web%20Audio%20API-FFE600?style=for-the-badge&logo=javascript&logoColor=black)](src/ClickbaitDecoder/wwwroot/sfx.js)
[![NuGet packages: 0](https://img.shields.io/badge/NuGet%20packages-0-00B7EB?style=for-the-badge)](src/ClickbaitDecoder/ClickbaitDecoder.csproj)
<br>
[![Built with Claude Code](https://img.shields.io/badge/Built%20with-Claude%20Code-D97757?style=for-the-badge&logo=claude&logoColor=white)](https://claude.com/claude-code)
[![Timebox: 45 min](https://img.shields.io/badge/Timebox-45%20min-111111?style=for-the-badge)](#the-story-behind-it)
[![License: MIT](https://img.shields.io/badge/License-MIT-FFE600?style=for-the-badge)](LICENSE)

</div>

> [!NOTE]
> The coffee study in the app is **invented** (university, researchers, funder and numbers) to teach the red flags.
> The chocolate hoax on the verdict screen is **real** and sourced. Not medical advice.

---

## What it does

1. **Start at the headline.** _"SCIENTISTS PROVE: COFFEE SLASHES CANCER RISK BY 40%!"_ A telephone chain across
   the top shows the real study is four layers down.
2. **Hunt the red flags.** Click anything fishy. Each find gets a **POW!**, a screen shake and a speech bubble
   explaining the flag. Find them fast for a **COMBO**, and clear a page to get it **stamped DEBUNKED!**
3. **Dig deeper.** Headline → press release → abstract → methods → fine print. The page gets calmer with every
   layer, from loud tabloid to plain journal. Rate how much you trust the claim at each step.
4. **See the verdict.** Your trust line draws itself, you earn a rank (Rookie Reader → Master Debunker), and you
   get the honest headline, a pocket checklist and confetti.
5. **True story.** A 2015 journalist ran a deliberately terrible chocolate diet study to test the press, and it
   hit the same red flags you just learned.
6. **Bring your own article.** Paste a URL (or the text). A rule-based scanner flags suspicious phrases, and you
   hunt them the same way.

## Built with

| What                                                                                 | Why                                                                                                                                                                 |
| ------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **.NET 10** · **ASP.NET Core Blazor Web App** (Interactive Server, `.slnx` solution) | Everything is C# components over one SignalR circuit, with no separate front-end build                                                                              |
| **C# / Razor components**                                                            | The case study is plain data (`Layer` → `Block` → `Segment` + `Flag`); one `LayerView` component renders both the story and scanned articles                        |
| **Plain CSS (no framework)**                                                         | Halftone dots, comic bursts (`clip-path`), stamp, shake and confetti keyframes, and a self-drawing SVG chart (`pathLength` + `stroke-dashoffset`). No CSS framework |
| **Web Audio API**                                                                    | Every pow, boing, stamp, whoosh and ta-da is synthesized in the browser. No audio files                                                                             |
| **Google Fonts**: _Bangers_ + _Comic Neue_                                           | Falls back to Impact and Comic Sans when offline                                                                                                                    |
| **`HttpClient` + source-generated `Regex`**                                          | Fetches a public article and extracts the headline and paragraphs. No HTML-parser dependency                                                                        |
| **Zero NuGet packages**                                                              | Only the .NET 10 shared framework                                                                                                                                   |
| **[Claude Code](https://claude.com/claude-code)** (Claude Opus 5.5)                  | Pair-programmed: interview, scaffold, build, browser testing and fact-checking the real-world sources                                                               |

## Run it

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```powershell
dotnet run --project src/ClickbaitDecoder --launch-profile http
```

Then open **http://localhost:5002**. Or open `ClickbaitDecoder.slnx` in Visual Studio and press **F5**.

> [!TIP]
> Use `dotnet watch --project src/ClickbaitDecoder --launch-profile http` to pick up edits as you go.
> The speaker button (top right) mutes sound, and it remembers your choice per browser.

## Project tour

| Path                                                                                                                                                                                                      | What it does                                                                                                                              |
| --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------- |
| [`Story/CaseStudy.cs`](src/ClickbaitDecoder/Story/CaseStudy.cs)                                                                                                                                           | The invented coffee study (5 layers, 19 flags), the honest headline, the checklist, the real chocolate-hoax card and the learn-more links |
| [`Components/LayerView.razor`](src/ClickbaitDecoder/Components/LayerView.razor)                                                                                                                           | One clickable "page" of the hunt: flags, bursts, combos, stamp, speech bubbles and trust slider                                           |
| [`Components/Pages/Home.razor`](src/ClickbaitDecoder/Components/Pages/Home.razor)                                                                                                                         | The guided walkthrough: telephone chain, layers, verdict, confetti                                                                        |
| [`Components/Pages/Scan.razor`](src/ClickbaitDecoder/Components/Pages/Scan.razor)                                                                                                                         | "Bring your own article": paste a URL or the article text                                                                                 |
| [`Services/ArticleFetcher.cs`](src/ClickbaitDecoder/Services/ArticleFetcher.cs)                                                                                                                           | Downloads a public page and extracts the article text. Refuses private and loopback addresses                                             |
| [`Services/RedFlagScanner.cs`](src/ClickbaitDecoder/Services/RedFlagScanner.cs)                                                                                                                           | 12 regex rules that turn any article into the same clickable layer as the story                                                           |
| [`Components/LearnMore.razor`](src/ClickbaitDecoder/Components/LearnMore.razor)                                                                                                                           | "Want to get good at this?" links (Cornell, Wikipedia, Open University)                                                                   |
| [`Components/SoundToggle.razor`](src/ClickbaitDecoder/Components/SoundToggle.razor) · [`Services/Sfx.cs`](src/ClickbaitDecoder/Services/Sfx.cs) · [`wwwroot/sfx.js`](src/ClickbaitDecoder/wwwroot/sfx.js) | Synthesized sound effects and the mute toggle                                                                                             |
| [`wwwroot/decoder.css`](src/ClickbaitDecoder/wwwroot/decoder.css) · [`wwwroot/decoder.js`](src/ClickbaitDecoder/wwwroot/decoder.js)                                                                       | All the styling and animation, plus a helper that keeps the newest bubble in view                                                         |

All paths are under `src/ClickbaitDecoder/`.

## The extras

- **POW bursts + screen shake** on every flag found. Find flags within 2.5 seconds of each other for **2x / 3x / 4x COMBO!**
- **Rubber stamp** when a page is cleared: _DEBUNKED! / BUSTED! / FACT-CHECKED!_ if you found them all, a grey
  _CASE CLOSED_ if you used "give up".
- **Bubbles appear newest first**, and the side column **follows you down** long scanned articles.
- **Verdict:** the trust line draws itself, you get a rank, and confetti fires: a full shower at 75%+ of flags found,
  a small sprinkle otherwise.
- **Sounds:** pow, boing (miss), combo arpeggio, stamp thud, whoosh between layers, ta-da at the verdict.
- **Accessibility:** animations shorten automatically for people with _reduce motion_ switched on.

## Learn more

- [Reading Science Info](https://guides.library.cornell.edu/evaluate/readsci): Cornell University Library. A short,
  free starting point.
- [Correlation does not imply causation](https://en.wikipedia.org/wiki/Correlation_does_not_imply_causation): Wikipedia.
- [Assessing contemporary science](https://www.open.edu/openlearn/science-maths-technology/assessing-contemporary-science/content-section-0):
  The Open University (OpenLearn). Free, intermediate, about 9 hours.

## The story behind it

Built for a team dev-meeting build challenge. The randomly drawn theme was **Wild Card → _Explain it to a
newcomer_**, with a **45-minute timebox** to get something demo-ready (the polish came afterwards). The idea:
pick something you know well and make it clickable for someone who's never heard of it.

## Honesty notes

- **The coffee study is invented**: the university, researchers, funder and numbers. It's labelled in the app,
  and no real company or customer data is used anywhere.
- **The chocolate hoax is real.** The facts were checked on 2026-11-18 against John Bohannon's own account:
  ["I Fooled Millions Into Thinking Chocolate Helps Weight Loss. Here's How." (io9/Gizmodo, 2015)](https://gizmodo.com/i-fooled-millions-into-thinking-chocolate-helps-weight-1707251800).
  The "millions" is his claim, and other journalists disputed it
  ([Faye Flam, NASW](https://www.nasw.org/node/254369)). The card says so.
- **The learn-more links** were each fetched and checked on 2026-11-18.
- **The scanner is regex rules, not a fact checker.** It misses things and sometimes flags innocent sentences. Any
  "suggests" counts as a hedge word (a _good_ sign).
- **The fetcher refuses private and loopback addresses** with a DNS check, but redirects aren't re-checked. That's
  fine for running it locally, not for hosting it publicly as-is.

## License

Released under the [MIT License](LICENSE). Use it, remix it, teach with it.

**Third-party pieces**

| What                                                                                                                        | License                                                                                                          | Notes                                            |
| --------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------- | ------------------------------------------------ |
| Blazor Web App template files (`App.razor`, `Routes.razor`, `Layout/*`, `Error.razor`, `NotFound.razor`, `wwwroot/app.css`) | [MIT](https://github.com/dotnet/aspnetcore/blob/main/LICENSE.txt), © .NET Foundation and Contributors            | Generated by `dotnet new blazor`                 |
| [Bangers](https://fonts.google.com/specimen/Bangers) font                                                                   | [SIL OFL 1.1](https://github.com/google/fonts/blob/main/ofl/bangers/OFL.txt), © The Bangers Project Authors      | Loaded from Google Fonts at runtime, not bundled |
| [Comic Neue](https://fonts.google.com/specimen/Comic+Neue) font                                                             | [SIL OFL 1.1](https://github.com/google/fonts/blob/main/ofl/comicneue/OFL.txt), © The Comic Neue Project Authors | Loaded from Google Fonts at runtime, not bundled |

The real-world chocolate-hoax card is a short paraphrase with a link to the original article. The learn-more
resources are linked, not copied.
