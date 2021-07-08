using System.Collections.Generic;
using System.Linq;

namespace GSheets.Models
{
    public class QualificationResult
    {

        public string GroupName { get; set; }

        public string TeamName { get; set; }

        public bool Qualified { get; set; } = false; 

        public int TeamPoints { get; set; }
        
        public Bets Bet { get; set; }

        public BetsResult Points { get; set; }

        public bool FirstQuali { get; set; } = false; 

        public bool SecondQuali { get; set; } = false;


        
    }
}