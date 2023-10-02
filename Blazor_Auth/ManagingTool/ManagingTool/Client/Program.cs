using ManagingTool.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using Blazored.SessionStorage;
using ManagingTool.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddAntDesign();
builder.Services.AddBlazoredSessionStorage();

builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();
builder.Services.AddTransient<TokenManager>();


builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

UserService._httpClient = builder.Services.BuildServiceProvider().GetRequiredService<HttpClient>();
AuthService._httpClient = builder.Services.BuildServiceProvider().GetRequiredService<HttpClient>();

await builder.Build().RunAsync();