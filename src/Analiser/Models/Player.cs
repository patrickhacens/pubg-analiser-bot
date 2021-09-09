using System.Collections.Generic;

namespace PUBG.Models
{
    public class Player : PubgObject<PlayerAttributes>
    {
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
