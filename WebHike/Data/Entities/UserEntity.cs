using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebHike.Data.Entities;

[Table("tblUsers")]
public class UserEntity
{
    [Key]
    public int Id { get; set; }

    [StringLength(150)]
    public string Email { get; set; } = null!;

    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string LastName { get; set; } = null!;

    [StringLength(250)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(150)]
    public string Image { get; set; } = "default.jpg";

    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
}