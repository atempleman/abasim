namespace ABASim.api.Dtos
{
    public class DetailedRetiredPlayerDto
    {
        public int PlayerId { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public int PGPosition { get; set; }

        public int SGPosition { get; set; }

        public int SFPosition { get; set; }

        public int PFPosition { get; set; }

        public int CPosition { get; set; }
    }
}