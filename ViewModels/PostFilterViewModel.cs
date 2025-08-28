/* =======================================================
 *
 * Created by anele on 27/08/2025.
 *
 * @anele_ace
 *
 * =======================================================
 */

 using Microsoft.AspNetCore.Mvc.Rendering;

public class PostFilterViewModel
{
    public int? SelectedAuthorId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public List<SelectListItem> AuthorsList { get; set; } = new();
    public IEnumerable<MyApi.Models.Post> Posts { get; set; } = new List<MyApi.Models.Post>();
}
