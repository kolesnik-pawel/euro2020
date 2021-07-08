namespace GSheets.Models
{
    public class TeamAtGroup
    {
        public int drawn { get; set; }
        public int goalDifference { get; set; }
        public int goalsAgainst { get; set; }
        public int goalsFor { get; set; }
        public bool isLive { get; set; }
        public int lost { get; set; }
        public int played { get; set; }
        public int points { get; set; }
        public bool qualified { get; set; }
        public int rank { get; set; }
        public Team team { get; set; }
        public int won { get; set; }

    }
}