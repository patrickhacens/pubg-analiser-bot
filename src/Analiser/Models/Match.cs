using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PUBG.Models
{
    public class Match : PubgObject<MatchAttributes>
    {
        public IEnumerable<PubgObject> Rosters => Relationships?["rosters"]?.Data;
        public IEnumerable<PubgObject> Assets => Relationships?["assets"]?.Data;
    }


    public class MatchAttributes
    {
        public int Duration { get; set; }
        public object Tags { get; set; }
        public string MatchType { get; set; }
        public DateTime CreatedAt { get; set; }
        public string GameMode { get; set; }
        public string TitleId { get; set; }
        public string ShardId { get; set; }
        public string MapName { get; set; }
        public bool IsCustomMatch { get; set; }
        public string SeasonState { get; set; }
        public object Stats { get; set; }
    }



}
