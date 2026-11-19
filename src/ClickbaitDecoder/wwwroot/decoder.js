// Small UI helpers called from Blazor. Failures are harmless (purely cosmetic).
window.decoderUi = {
    // Bring the newest speech bubble (top of the list) back into view inside the sticky side column.
    toTop(el) {
        try { if (el && el.scrollTop > 0) el.scrollTo({ top: 0, behavior: "smooth" }); } catch { /* ignore */ }
    },
};
