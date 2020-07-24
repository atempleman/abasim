namespace ABASim.api.Dtos
{
    public class PlayoffSummaryDto
    {
        public string HomeTeam { get; set; }

        public int HomeTeamId { get; set; }

        public string AwayTeam { get; set; }

        public int AwayTeamId { get; set; }

        public int HomeWins { get; set; }

        public int AwayWins { get; set; }
    }
}