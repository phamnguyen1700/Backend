using BE_V2.DataDB;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public partial class Product
{
    public Product()
    {
        OrderDetails = new HashSet<OrderDetail>();
        CartItems = new HashSet<CartItem>();
        WishlistItems = new HashSet<WishlistItem>();
        EventItems = new HashSet<EventItem>();
        PriceDetails = new HashSet<PriceDetail>();
        Feedbacks = new HashSet<Feedback>();  // Added Feedbacks collection
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int? ProductType { get; set; }
    public string? Material { get; set; }
    public string? Size { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public decimal? ProcessingPrice { get; set; }
    public decimal? ExchangeRate { get; set; }
    public int? Quantity { get; set; }
    public int? MainDiamondId { get; set; }
    public string? Image1 { get; set; }
    public string? Image2 { get; set; }
    public string? Image3 { get; set; }
    public int? SecondaryDiamondId { get; set; }
    public int? RingMoldId { get; set; }  // New column
    public int? NecklaceMoldId { get; set; }  // New column
    public int? SecondaryDiamondCount { get; set; } // New column

    [ForeignKey("MainDiamondId")]
    public virtual Diamond? MainDiamond { get; set; }

    [ForeignKey("SecondaryDiamondId")]
    public virtual Diamond? SecondaryDiamond { get; set; }

    [ForeignKey("RingMoldId")]
    public virtual RingMold? RingMold { get; set; }

    [ForeignKey("NecklaceMoldId")]
    public virtual NecklaceMold? NecklaceMold { get; set; }

    [ForeignKey("ProductType")]
    public virtual ProductType? ProductTypeNavigation { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    public virtual ICollection<CartItem> CartItems { get; set; }
    public virtual ICollection<WishlistItem> WishlistItems { get; set; }
    public virtual ICollection<EventItem> EventItems { get; set; }
    public virtual ICollection<PriceDetail> PriceDetails { get; set; }
    public virtual ICollection<Feedback> Feedbacks { get; set; }
}