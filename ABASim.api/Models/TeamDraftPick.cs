namespace ABASim.api.Models
{
    public class TeamDraftPick
    {
        public int Id { get; set; }

        public int Year { get; set; }

        public int Round { get; set; }

        public int OriginalTeam { get; set; }

        public int CurrentTeam { get; set; }
    }
}