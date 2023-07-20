using System.ComponentModel.DataAnnotations;

namespace MinimulAPIdemo;
public class User
{
    [Required]
    public int Id { get; set; }
    [Required,MaxLength(20)]
    public string Name { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [RegularExpression(@"^\d{10}$")]
    public string Mobile { get; set; }
}
