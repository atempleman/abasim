namespace ABASim.api.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public int TeamId { get; set; }

        public int PlayerId { get; set; }

        public int TransactionType { get; set; }

        public int Day { get; set; }
    }
}