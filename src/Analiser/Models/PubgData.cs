using System.Collections.Generic;

namespace PUBG.Models
{
    public class PubgData<T>
    {
        public T Data { get; set; }
        public Links Links { get; set; }
        public Metas Meta { get; set; }

        public IEnumerable<PubgObject> Included { get; set; }
    }

    public class PubgObject
    {
        public ModelType Type { get; set; }

        public string Id { get; set; }
    }


    public class PubgObject<TAttributes> : PubgObject
    {
        public TAttributes Attributes { get; set; }

        public Links Links { get; set; }

        public Dictionary<string, PubgData<IEnumerable<PubgObject>>> Relationships { get; set; }

        public Metas Meta { get; set; }

        
    }
}
