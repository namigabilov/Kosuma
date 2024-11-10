using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Kosuma.Models;
using Kosuma.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.AspNetCore.SignalR;
using Kosuma.Hubs;

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

    public async Task<IActionResult> NotFound()
    {
        return View();
    }

    public async Task<IActionResult> Privacy()
    {
        return View();
    }
}
