using Kosuma.Hubs;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MatcXCors", policy =>
    {
        policy.AllowAnyMethod()
              .AllowAnyHeader()
              .SetIsOriginAllowed(origin => true)
              .AllowCredentials();
    });
});

var app = builder.Build();

// Use CORS globally for all requests
app.UseCors("MatcXCors");

// Serve static files from /assets with CORS policy and custom MIME types
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".m3u8"] = "application/x-mpegURL";
provider.Mappings[".ts"] = "video/MP2T";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider,
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.WebRootPath, "assets")),
    RequestPath = "/assets"
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<StreamingHub>("/chatHub");

app.Run();
