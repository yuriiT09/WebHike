using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebHike.Data.Entities;

[Table("tblItemImages")]
public class ItemImageEntity
{
    [Key]
    public int Id { get; set; }

    [StringLength(150)]
    public string Image { get; set; } = null!;

    public int Priority { get; set; }

    public int ItemId { get; set; }

    public ItemEntity Item { get; set; } = null!;
}
