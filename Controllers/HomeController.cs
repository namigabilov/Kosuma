using Microsoft.AspNetCore.Mvc;
using Kosuma.Db;
using Microsoft.EntityFrameworkCore;
using Kosuma.Models;
using Kosuma.Models.Dtos;
using Kosuma.Services;

namespace Kosuma.Controllers;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment _env;

    public HomeController(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        using (var context = new AppDbContext())
        {
            var streams = await context.LiveStreams.ToListAsync();

            ViewData["Streams"] = streams;

            return View();
        }
    }
    [HttpGet]
    public IActionResult CreateStream()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateStream([FromForm] StreamDto dto)
    {
        using (var context = new AppDbContext())
        {
            var stream = new LiveStream
            {
                Description = dto.Description,
                Thumbnail = dto.Thumbnail,
                Name = dto.Name,
                LiveUrl = "",
                Id = Guid.NewGuid()
            };

            await context.LiveStreams.AddAsync(stream);
            await context.SaveChangesAsync();

            return RedirectToAction("Streaming", new { id = stream.Id });
        }
    }
    [HttpGet]
    public async Task<IActionResult> Streaming(Guid id)
    {
        using (var context = new AppDbContext())
        {
            var stream = await context.LiveStreams
                .Include(c => c.Chats.OrderBy(c => c.WritedTime))
                .FirstOrDefaultAsync(c => c.Id == id);

            if (stream is null)
                return RedirectToAction("NotFound");

            return View(stream);
        }
    }
    public async Task<IActionResult> Stream(Guid id)
    {
        using (var context = new AppDbContext())
        {
            var stream = await context.LiveStreams
                .Include(c => c.Chats.OrderBy(c => c.WritedTime))
                .FirstOrDefaultAsync(c => c.Id == id);

            if (stream is null)
                return RedirectToAction("NotFound");

            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return View(stream);
        }
    }
    public async Task<IActionResult> AddStream([FromForm] IFormFile file, string streamId)
    {
        if (file == null || file.Length <= 0)
        {
            return BadRequest(" Invalid File.");
        }
        var tempFilePath = Path.Combine(_env.WebRootPath + "/temps", file.FileName);
        try
        {
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            bool isNewStream = VideoService.InitializeStream(streamId, _env.WebRootPath + "/assets");

            VideoService.ConvertSegmentToTs(streamId, tempFilePath, _env.WebRootPath + $"/assets/{streamId}");

            if (isNewStream)
            {
                using (var context = new AppDbContext())
                {
                    var stream = await context.LiveStreams.FirstOrDefaultAsync(c => c.Id == Guid.Parse(streamId));

                    if (stream is null)
                        return RedirectToAction("NotFound");

                    stream.LiveUrl = $"http://localhost:5290/assets/{streamId}/stream.m3u8";

                    await context.SaveChangesAsync();
                }
            }

            return Ok(new { message = "Segment başarıyla yüklendi ve işlendi." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Hata oluştu: {ex.Message}");
        }
        finally
        {
            if (System.IO.File.Exists(tempFilePath))
            {
                System.IO.File.Delete(tempFilePath);
            }
        }
    }

    public async Task<IActionResult> NotFound()
    {
        return View();
    }

    public async Task<IActionResult> Privacy()
    {
        return View();
    }
}
