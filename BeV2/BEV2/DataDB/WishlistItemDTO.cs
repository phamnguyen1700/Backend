namespace BE_V2.DataDB
{
    public class WishlistItemDTO
    {
        public int WishlistId { get; set; }
        public int ProductId { get; set; }
        public DateTime AddedDate { get; set; }
        public int CustomerId { get; set; }  
    }
}
