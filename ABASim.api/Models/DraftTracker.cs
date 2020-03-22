namespace ABASim.api.Models
{
    public class DraftTracker
    {
        public int Id { get; set; }

        public int Round { get; set; }

        public int Pick { get; set; }

        public string DateTimeOfLastPick { get; set; }
    }
}