using System.ComponentModel.DataAnnotations;

namespace Kosuma.Models
{
    public class LiveChat
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }
        public string WritedBy { get; set; }
        public int WritedTime { get; set; }
        public Guid LiveStreamId { get; set; }
        public LiveStream LiveStream { get; set; }
    }
}