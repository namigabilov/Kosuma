using System.Text.Json.Serialization;
using Kosuma.Db;
using Kosuma.Models;
using Kosuma.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Kosuma.Hubs
{
    public struct VideoData
        {
            public int Index { get; }
            public string Part { get; }


            [JsonConstructor]
            public VideoData(int index, string part) => (Index, Part) = (index, part);
        }
    public class Data
    {
        public string Message { get; set; }
        public int Second { get; set; }
        public string StreamId { get; set; }
    }

    public class StreamingHub : Hub
    { 
        public async Task JoinVideo(string videoId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, videoId);
        }

        public async Task SendMessage(Data data)
        {
            using (var _context = new AppDbContext())
            {
                var stream = await _context.LiveStreams
                .Include(c => c.Chats.OrderBy(c => c.WritedTime))
                .FirstOrDefaultAsync(c => c.Id == Guid.Parse(data.StreamId));

                var user = await _context.Users.FirstOrDefaultAsync(c => c.ConnectionId == Context.ConnectionId);

                var chat = new LiveChat
                {
                    Message = data.Message,
                    WritedTime = data.Second,
                    WritedBy = user is null ? "Anonymous" : user.Name
                };

                stream?.Chats.Add(chat);

                await _context.SaveChangesAsync();

                await Clients.Group(data.StreamId).SendAsync("ReceiveMessage", new { Message = chat.Message, User = chat.WritedBy });
            }
        }

        public override async Task OnConnectedAsync()
        {
            using (var _context = new AppDbContext())
            {
                var names = NameProvider.LoadNames();
                var random = new Random();
                var randomName = names[random.Next(names.Count)];

                var user = new User
                {
                    Name = randomName,
                    ConnectionId = Context.ConnectionId
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                await base.OnConnectedAsync();
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            using (var _context = new AppDbContext())
            {
                var user = _context.Users.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                }

                await base.OnDisconnectedAsync(exception);
            }
        }
    }

}