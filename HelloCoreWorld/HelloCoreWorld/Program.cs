using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDataProtection();

//If you would like to store the key in the file system, you need to configure the data protection
//builder.Services.AddDataProtection().PersistKeysToFileSystem(
//    new System.IO.DirectoryInfo(@"C:\Hooman\Temp")
//    );

//To configure the system to use a default key lifetime of 14 days instead of 90 days
//builder.Services.AddDataProtection().SetDefaultKeyLifetime(TimeSpan.FromDays(14));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//Hooman:Forwarded Headers Middleware helps your ASP.NET Core application to look for headers added by an upstream proxy or load balancer.
//If you are using another proxy besides IIS,  you need to add this. IIS takes care of Forwarded Headers by default.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
}
    );

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
