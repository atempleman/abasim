namespace ABASim.api.Models
{
    public class Trade
    {
        public int Id { get; set; }

        public int TradingTeam { get; set; }

        public int ReceivingTeam { get; set; }

        public int TradeId { get; set; }

        public int PlayerId { get; set; }

        public int Pick { get; set; }

        public int Status { get; set; }

        public int Year { get; set; }

        public int OriginalTeam { get; set; }
    }
}