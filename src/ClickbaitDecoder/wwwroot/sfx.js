// Comic sound effects, synthesized with the Web Audio API (no audio files).
// Blazor calls window.sfx.play(name, delayMs, n). Everything fails silently: sound is a bonus, never a blocker.
window.sfx = (() => {
    let ctx = null;
    let muted = false;
    try { muted = localStorage.getItem("cd-muted") === "1"; } catch { /* storage blocked: default to sound on */ }

    const ac = () => {
        if (!ctx) ctx = new (window.AudioContext || window.webkitAudioContext)();
        if (ctx.state === "suspended") ctx.resume();
        return ctx;
    };

    // A pitched blip that slides from f0 to f1 and fades out.
    function tone(type, f0, f1, dur, vol, t0 = 0, vibratoHz = 0, vibratoDepth = 0) {
        const c = ac(), t = c.currentTime + t0;
        const o = c.createOscillator(), g = c.createGain();
        o.type = type;
        o.frequency.setValueAtTime(f0, t);
        o.frequency.exponentialRampToValueAtTime(Math.max(f1, 1), t + dur);
        if (vibratoHz) {
            const lfo = c.createOscillator(), lg = c.createGain();
            lfo.frequency.value = vibratoHz; lg.gain.value = vibratoDepth;
            lfo.connect(lg).connect(o.frequency); lfo.start(t); lfo.stop(t + dur);
        }
        g.gain.setValueAtTime(vol, t);
        g.gain.exponentialRampToValueAtTime(0.0001, t + dur);
        o.connect(g).connect(c.destination);
        o.start(t); o.stop(t + dur + 0.02);
    }

    // A burst of filtered white noise (punches, thuds, whooshes).
    function noise(dur, vol, fFrom, fTo, t0 = 0, type = "lowpass") {
        const c = ac(), t = c.currentTime + t0;
        const len = Math.floor(c.sampleRate * dur);
        const buf = c.createBuffer(1, len, c.sampleRate), d = buf.getChannelData(0);
        for (let i = 0; i < len; i++) d[i] = Math.random() * 2 - 1;
        const src = c.createBufferSource(); src.buffer = buf;
        const f = c.createBiquadFilter(); f.type = type;
        f.frequency.setValueAtTime(fFrom, t);
        f.frequency.exponentialRampToValueAtTime(fTo, t + dur);
        const g = c.createGain();
        g.gain.setValueAtTime(vol, t);
        g.gain.exponentialRampToValueAtTime(0.0001, t + dur);
        src.connect(f).connect(g).connect(c.destination);
        src.start(t);
    }

    const sounds = {
        pow:    () => { noise(0.22, 0.45, 3500, 200); tone("sine", 190, 45, 0.2, 0.55); },
        miss:   () => tone("sine", 420, 210, 0.45, 0.22, 0, 14, 35),                 // boi-oi-oing
        combo:  (n) => { for (let i = 0; i <= Math.min(n, 6); i++)                     // rising arpeggio, longer for bigger combos
                             tone("square", 392 * Math.pow(2, (i * 4) / 12), 392 * Math.pow(2, (i * 4) / 12), 0.09, 0.09, i * 0.06); },
        stamp:  () => { tone("sine", 150, 38, 0.32, 0.8); noise(0.1, 0.35, 1400, 120); },
        whoosh: () => noise(0.4, 0.3, 300, 5000, 0, "bandpass"),
        tada:   () => [523.25, 659.25, 783.99, 1046.5].forEach((f, i) => tone("square", f, f, i === 3 ? 0.55 : 0.13, 0.1, i * 0.11)),
    };

    return {
        play(name, delayMs = 0, n = 0) {
            if (muted || !sounds[name]) return;
            const go = () => { try { sounds[name](n); } catch { /* audio unavailable */ } };
            delayMs > 0 ? setTimeout(go, delayMs) : go();
        },
        isMuted: () => muted,
        toggle() {
            muted = !muted;
            try { localStorage.setItem("cd-muted", muted ? "1" : "0"); } catch { /* ignore */ }
            return muted;
        },
    };
})();
