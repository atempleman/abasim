namespace ABASim.api.Dtos
{
    public class TransactionDto
    {
        public string TeamMascot { get; set; }

        public string PlayerName { get; set; }

        public int PlayerId { get; set; }

        public string TransactionType { get; set; }

        public int Day { get; set; }
    }
}