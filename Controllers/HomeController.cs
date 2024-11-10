using Microsoft.AspNetCore.Mvc;
using Kosuma.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Kosuma.Hubs;
using Kosuma.Models;

namespace Kosuma.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHubContext<StreamingHub> _hub;

    public HomeController(ILogger<HomeController> logger, IHubContext<StreamingHub> hub)
    {
        _logger = logger;
        _hub = hub;
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

    public async Task<IActionResult> Stream(Guid id)
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

    public async Task<IActionResult> AddStream()
    {
        using (var context = new AppDbContext())
        {
            var streams = await context.LiveStreams.Include(c => c.Chats).ToListAsync();
            context.LiveStreams.RemoveRange(streams);

            List<LiveStream> s = new List<LiveStream>();

            s.Add(new LiveStream
            {
                Description = "Embark on a cosmic adventure as we explore the mysteries of the universe, from swirling galaxies to the outer reaches of our solar system. Get ready to have your mind blown by the wonders of space!",
                Name = "Journey Through the Cosmos 🌌",
                LiveUrl = "",
                Thumbnail = "https://img.freepik.com/free-photo/creative-crystal-lens-ball-photography-lake-with-greenery-around-dawn_181624-29379.jpg"
            });

            s.Add(new LiveStream
            {
                Description = "Join us for a taste of global cuisines as we travel from street food in Bangkok to fine dining in Paris—all from your screen! Learn cooking tips, unique recipes, and cultural food stories that bring the world to your kitchen.",
                Name = "Culinary World Tour: Flavors Across Continents 🍲",
                LiveUrl = "",
                Thumbnail = "https://t3.ftcdn.net/jpg/07/25/01/68/360_F_725016819_jOXJkOJZqRSEkdKhheFsLkxuYq59iwa0.jpg"
            });

            s.Add(new LiveStream
            {
                Description = "Delve into the most intriguing mysteries, unsolved cases, and paranormal tales! From famous historical enigmas to lesser-known cases, join the discussion, share theories, and try to crack the mysteries of our world.",
                Name = "Mystery Hour: Dive into Unsolved Cases 🕵️‍♀️",
                LiveUrl = "",
                Thumbnail = "https://img.freepik.com/free-photo/mythical-video-game-inspired-landscape-with-mountains_23-2150974373.jpg"
            });

            s.Add(new LiveStream
            {
                Description = "Relax and unwind as we dive into a fun, beginner-friendly painting session. Bring your own supplies, follow along, or just watch the art unfold! Perfect for artists, beginners, or anyone looking for a creative escape.",
                Name = "The Creative Hour: Painting with a Twist 🎨",
                LiveUrl = "",
                Thumbnail = "https://www.shutterstock.com/image-photo/awesome-pic-natureza-600nw-2408133899.jpg"
            });

            await context.LiveStreams.AddRangeAsync(s);
            await context.SaveChangesAsync();

            return Ok("Added !");
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
