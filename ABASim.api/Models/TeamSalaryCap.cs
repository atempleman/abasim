namespace ABASim.api.Models
{
    public class TeamSalaryCap
    {
        public int Id { get; set; }

        public int CapId { get; set; }

        public int TeamId { get; set; }

        public int CurrentCapAmount { get; set; }
    }
}