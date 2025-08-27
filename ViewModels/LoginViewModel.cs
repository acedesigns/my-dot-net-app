/* =======================================================
 *
 * Created by anele on 28/08/2025.
 *
 * @anele_ace
 *
 * =======================================================
 */

using System.ComponentModel.DataAnnotations;

namespace MyApi.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}