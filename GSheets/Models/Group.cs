using System.Collections.Generic;

namespace GSheets.Models
{
    public class Group
    {
        public GroupInformation  group { get; set; }
        public List<TeamAtGroup>  items { get; set; }
        public string  status { get; set; }
    }
}