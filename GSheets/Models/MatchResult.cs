using GSheets.Enums;

namespace GSheets.Models
{
    public class MatchResult
    {
        public string homeTeam { get; set; }

        public string awayTeam { get; set; }

        public WinsEnum winTeam { get; set; }

        public int ReadCards { get; set; }

        public int YellowCards { get; set; }

        public Score score { get; set; }

        public int penalty { get; set; }

        public Bets bets { get; set; }

        public BetsResult Points { get; set; }

    }


    public class BetsResult
    {
        public int RedCardBets { get; set; }

        public int WinTeamBets { get; set; }

        public int MachScoreBets { get; set; }

        public int GroupQualification { get; set; }

        public int TourmentClassification {get; set;}
    }
}