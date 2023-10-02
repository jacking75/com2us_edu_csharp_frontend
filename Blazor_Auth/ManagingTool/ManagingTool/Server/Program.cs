using ManagingTool.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.ResponseCompression;
using ManagingTool.Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();


// JwtBearer의 토큰 검증 옵션 및 검증 이벤트 정의
var jwtBearerConfig = new JwtBearerConfig();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = jwtBearerConfig.tokenValidatedParameters;

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    jwtBearerConfig.OnAuthenticationFailedHandler(context, options);
                    return Task.CompletedTask;
                }
            };
        });

// AccountId에 대한 인가 정책 정의
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AccountIdPolicy", policy => policy.RequireClaim("AccountId"));
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
