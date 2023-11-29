using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;
using System.Reflection.PortableExecutable;
using Microsoft.AspNetCore.Hosting;
using PoetryViewerBack.External.Audiotor;
using PoetryViewerBack.External.Translator;
using PoetryViewerBack.DTO;
using PoetryViewerBack.Models;
using static System.Net.Mime.MediaTypeNames;

//await Auditor2.Test();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

var app = builder.Build();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(policy => policy.AllowAnyHeader()
                            .WithMethods("GET", "POST", "PUT", "DELETE")
                            .AllowAnyMethod()
                            .SetIsOriginAllowed(origin => true)
                            .AllowCredentials());

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
