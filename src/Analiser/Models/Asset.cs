using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PUBG.Models
{
    public class Asset : PubgObject<AssetAttributes>
    {
    }

    public class AssetAttributes
    {
        public string Url { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
