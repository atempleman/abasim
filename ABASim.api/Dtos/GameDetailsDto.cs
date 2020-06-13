namespace ABASim.api.Dtos
{
    public class GameDetailsDto
    {
        public int GameId { get; set; }

        public string AwayTeam { get; set; }

        public int AwayTeamId { get; set; }

        public string HomeTeam { get; set; }

        public int HomeTeamId { get; set; }
    }
}