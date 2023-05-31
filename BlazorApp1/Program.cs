using BlazorApp1.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Serilog;

var Configuration = new ConfigurationBuilder()
        //设置工作主目录
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile(string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")) ? "appsettings.json" : $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();

var builder = WebApplication.CreateBuilder(args);
//指定应用运行时配置文件
builder.Configuration.AddJsonFile(string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")) ? "appsettings.json" : $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: false, reloadOnChange: true);
builder.Configuration.AddCommandLine(args);
// 配置日志接收器
builder.Host.UseSerilog((hostContext, loggerConfig) =>
{
    loggerConfig
        .ReadFrom.Configuration(hostContext.Configuration)
        .Enrich.WithProperty("ApplicationName", hostContext.HostingEnvironment.ApplicationName);
});
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()|| Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Localhost")
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

try
{
    Log.Information("Web主机服务启动.......");
    Log.Information($"当前环境变量为:【{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}】");
    Log.Information($"当前工作目录为：【{Directory.GetCurrentDirectory()}】");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Web主机遇到无法处理的致命异常将退出服务........");
}
finally
{
    await Log.CloseAndFlushAsync();
}
