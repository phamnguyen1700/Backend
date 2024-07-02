namespace BE_V2.DataDB
{
    public class CartItemDTO
    {
        public int CartID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; } // Add this line
        public int UserID { get; set; }
    }
}