using AutoMapper;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using MudBlazor.Services;
using PhotoApp.Front.Client.Connection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddHttpClient("PhotoApp.Api", client =>
{
    var baseUrl = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;
    if (!baseUrl.EndsWith("/api"))
    {
        baseUrl = $"{baseUrl.TrimEnd('/')}/api";
    }
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("PhotoApp.Api"));

builder.Services.AddScoped<ApiConnection>();

var configuration = new MapperConfiguration(config =>
{
    config.AddProfile(new MappingProfile());
}, new NullLoggerFactory());

var mapper = configuration.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddMudServices();

await builder.Build().RunAsync();
