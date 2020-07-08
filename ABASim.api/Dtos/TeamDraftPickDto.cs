namespace ABASim.api.Dtos
{
    public class TeamDraftPickDto
    {
        public int Year { get; set; }

        public int Round { get; set; }

        public int OriginalTeam { get; set; }

        public string OriginalTeamName { get; set; }

        public int CurrentTeam { get; set; }

        public string CurrentTeamName { get; set; }
    }
}