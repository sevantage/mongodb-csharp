using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Base.Serialization
{
    internal class YearSerializer : IBsonSerializer
    {
        public Type ValueType => typeof(int);

        public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context.Reader.CurrentBsonType == MongoDB.Bson.BsonType.Int32)
                return context.Reader.ReadInt32();
            else if (context.Reader.CurrentBsonType == MongoDB.Bson.BsonType.String)
            {
                var str = context.Reader.ReadString();
                return int.Parse(str.Substring(0, 4));
            }
            throw new InvalidOperationException("Encountered invalid BSON type.");
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            throw new NotImplementedException();
        }
    }
}
