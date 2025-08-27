/* =======================================================
 *
 * Created by anele on 27/08/2025.
 *
 * @anele_ace
 *
 * =======================================================
 */

using System.ComponentModel.DataAnnotations;

namespace MyApi.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Email { get; set; }

        [Required]
        public required string PasswordHash { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
