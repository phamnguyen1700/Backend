namespace BE_V2.DataDB
{
    public class EventItemDTO
    {
        public int EventItemID { get; set; }
        public int EventID { get; set; }
        public int ProductID { get; set; }
        public DateTime Date { get; set; }
        public decimal Discount { get; set; }
    }
}
