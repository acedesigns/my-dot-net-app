/* =======================================================
 *
 * Created by anele on 27/08/2025.
 *
 * @anele_ace
 *
 * =======================================================
 */

namespace MyApi.Models
{
    public class Like
    {
        public int Id { get; set; }

        public int PostId { get; set; }
        public Post? Post { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
