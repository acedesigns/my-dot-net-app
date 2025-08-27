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
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public required string Content { get; set; }

        public int PostId { get; set; }
        public Post? Post { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
