using System.Collections.Generic;
using System.Diagnostics;

namespace PUBG.Models
{
    [DebuggerDisplay("Player {Name}")]
    public class Player : PubgObject<PlayerAttributes>
    {
        public string Name => Attributes?.Name;
        public IEnumerable<PubgObject> Matches => Relationships?["matches"]?.Data;

        public IEnumerable<PubgObject> Assets => Relationships?["assets"]?.Data;
    }

    public class PlayerAttributes
    {
        public string Name { get; set; }
        public object Stats { get; set; }
        public string TitleId { get; set; }
        public string ShardId { get; set; }
        public string PatchVersion { get; set; }
    }
}
