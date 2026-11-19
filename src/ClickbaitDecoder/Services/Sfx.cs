using Microsoft.JSInterop;

namespace ClickbaitDecoder.Services;

/// <summary>
/// Thin wrapper over wwwroot/sfx.js. Sound is decoration, so every call swallows interop failures
/// (prerendering, a dropped circuit, a browser without Web Audio) instead of breaking the page.
/// </summary>
public sealed class Sfx(IJSRuntime js)
{
    public Task Play(string name, int delayMs = 0, int n = 0) => Try(() => js.InvokeVoidAsync("sfx.play", name, delayMs, n).AsTask());

    public async Task<bool> IsMuted()
    {
        bool muted = false;
        await Try(async () => muted = await js.InvokeAsync<bool>("sfx.isMuted"));
        return muted;
    }

    public async Task<bool> Toggle()
    {
        bool muted = false;
        await Try(async () => muted = await js.InvokeAsync<bool>("sfx.toggle"));
        return muted;
    }

    static async Task Try(Func<Task> call)
    {
        try { await call(); }
        catch (JSException) { }
        catch (JSDisconnectedException) { }
        catch (InvalidOperationException) { } // JS interop not available during prerender
        catch (TaskCanceledException) { }
    }
}
