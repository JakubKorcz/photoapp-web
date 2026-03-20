using AutoMapper;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using MudBlazor.Services;
using PhotoApp.Client;
using PhotoApp.Client.Connection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<AuthenticationHeaderHandler>();

builder.Services.AddHttpClient("PhotoApp.Api", client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
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

await builder.Build().RunAsync();
