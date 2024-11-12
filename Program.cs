using System.Text.Json.Serialization;
using Kosuma.Hubs;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });
builder.Services.AddSignalR();

builder.Services.AddCors(c =>
{
    c.AddPolicy("MatcXCors", c =>
    {
        c.AllowAnyMethod()
         .AllowAnyHeader()
         .SetIsOriginAllowed(origin => true)
         .AllowCredentials();
    });
});
var app = builder.Build();
app.UseWhen(context => context.Request.Path.StartsWithSegments("/assets"), appBuilder =>
{
    appBuilder.UseCors("MatcXCors");
    var provider = new FileExtensionContentTypeProvider();
    provider.Mappings[".m3u8"] = "application/x-mpegURL";
    provider.Mappings[".ts"] = "video/MP2T";
    appBuilder.UseStaticFiles(new StaticFileOptions
    {
        ContentTypeProvider = provider,
        FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.WebRootPath, "assets")),
        RequestPath = "/assets"
    });
});
app.UseStaticFiles();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.UseCors("MatcXCors");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<StreamingHub>("/chatHub");

app.Run();