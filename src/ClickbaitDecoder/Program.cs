using ClickbaitDecoder.Components;
using ClickbaitDecoder.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<Sfx>(); // per-circuit wrapper over wwwroot/sfx.js

// Typed HttpClient for the "bring your own article" scanner.
// Generic browser-like UA (many news sites reject unknown bots); no personal details in it.
builder.Services.AddHttpClient<ArticleFetcher>(c =>
{
    c.Timeout = TimeSpan.FromSeconds(12);
    c.MaxResponseContentBufferSize = 4_000_000;
    c.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; ClickbaitDecoder/1.0)");
    c.DefaultRequestHeaders.Accept.ParseAdd("text/html");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
