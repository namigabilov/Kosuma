using System.ComponentModel.DataAnnotations;

namespace Kosuma.Models
{
    public class LiveStream
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public bool IsLiveNow { get; set; }
        public string Description { get; set; }
        public string LiveUrl { get; set; }
        public List<LiveChat> Chats { get; set; }
    }
}