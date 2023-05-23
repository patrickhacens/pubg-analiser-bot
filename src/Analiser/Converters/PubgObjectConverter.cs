using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PUBG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PUBG.Converters
{
    public class PubgObjectConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return typeof(PubgObject).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            

            var jobj = JObject.Load(reader);

            if (Enum.TryParse(typeof(ModelType), jobj["type"].ToString(), true, out var type))
            {
                return type switch
                {
                    ModelType.Asset => jobj.ToObject<Asset>(),
                    ModelType.Match => jobj.ToObject<Match>(),
                    ModelType.Participant => jobj.ToObject<MatchParticipant>(),
                    ModelType.Player => jobj.ToObject<Player>(),
                    ModelType.Roster => jobj.ToObject<MatchRoster>(),
                    _ => jobj.ToObject<PubgObject>()
                };
            }


            return jobj.ToObject<PubgObject>(serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
