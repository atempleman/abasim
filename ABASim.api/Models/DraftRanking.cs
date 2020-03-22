
namespace ABASim.api.Models
{
    public class DraftRanking
    {
        public int Id { get; set; }

        public int TeamId { get; set; }

        public int PlayerId { get; set; }

        public int Rank { get; set; }
    }
}