using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebHike.Data.Entities;

[Table("tblItems")]
public class ItemEntity
{
    [Key]
    public int Id { get; set; }

    [StringLength(250)]
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int CategoryId { get; set; }

    public CategoryEntity Category { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    public ICollection<ItemImageEntity> Images { get; set; } = new List<ItemImageEntity>();
}