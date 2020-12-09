namespace ABASim.api.Models
{
    public class WaiverWire
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public int WaivedPlayerContractId { get; set; }

        public int Day { get; set; }
    }
}