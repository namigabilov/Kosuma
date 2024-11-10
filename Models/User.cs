using System.ComponentModel.DataAnnotations;

namespace Kosuma.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ConnectionId { get; set; }
    }
}