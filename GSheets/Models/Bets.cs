using GSheets.Enums;

namespace GSheets.Models
{
    public class Bets
    {
        public TeamsValues RegularTimeWin { get; set; }

        public WinsEnum winTeam { get; set; }

        public int PenatlyWin { get; set; }

        public bool? RedCard { get; set; }

        public int YellowCards { get; set; }

        public string GroupQualification { get; set; }

        public string Winner { get; set; }

        public string  SecondPlace { get; set; }

        public string ThirdPlace { get; set; }

    }
}