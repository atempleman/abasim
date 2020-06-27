namespace ABASim.api.Models
{
    public class TradeMessage
    {
        public int Id { get; set; }

        public int TradeId { get; set; }

        public int IsMessage { get; set; }

        public string Message { get; set; }
    }
}