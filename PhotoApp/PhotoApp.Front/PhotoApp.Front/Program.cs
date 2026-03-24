using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using MudBlazor.Services;
using PhotoApp.Front.Components;
using PhotoApp.Front.Connection;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddTransient<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient("PhotoApp.Api", client =>
{
    var baseUrl = builder.Configuration["VITE_API_URL"] ?? "http://photoapp-api";
    client.BaseAddress = new Uri(baseUrl);
})
.AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("PhotoApp.Api"));

builder.Services.AddScoped<ApiConnection>();

//AutoMapper
var configuration = new MapperConfiguration(config =>
{
    config.AddProfile(new MappingProfile());
}, new NullLoggerFactory());

var mapper = configuration.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddMudServices();


var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
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
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(PhotoApp.Front.Client._Imports).Assembly);

app.Run();
