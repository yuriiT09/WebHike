namespace WebHike.Models.Cart;

public class CartItemViewModel
{
    public int ItemId { get; set; }

    public string Name { get; set; } = null!;

    public string Image { get; set; } = null!;

    public int Quantity { get; set; }
}