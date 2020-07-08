namespace ABASim.api.Dtos
{
    public class TradeDto
    {
        public int TradingTeam { get; set; }

        public string TradingTeamName { get; set; }

        public int ReceivingTeam { get; set; }

        public string ReceivingTeamName { get; set; }

        public int TradeId { get; set; }

        public int PlayerId { get; set; }

        public string PlayerName { get; set; }

        public int Pick { get; set; }

        public int Year { get; set; }

        public int OriginalTeam { get; set; }

        public int Status { get; set; }
    }
}