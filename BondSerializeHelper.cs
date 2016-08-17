using Microsoft.Bond;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SerializaType = System.Byte;

namespace Microsoft.Spark.CSharp.Common
{
    class BondSerializeHelper
    {
    }

    public static class Extend
    {
        public static SerializaType[] SerializeBondObject(this IBondSerializable value)
        {
            byte[] result;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CompactBinaryProtocolWriter compactBinaryProtocolWriter = new CompactBinaryProtocolWriter(memoryStream))
                {
                    ((IBondSerializable)value).Write(compactBinaryProtocolWriter);
                    result = memoryStream.ToArray();
                }
            }
            return result;
        }

        public static T DeserializeBondObject<T>(this SerializaType[] line) where T : IBondSerializable, new()
        {
            T a = new T();
            var payload = line;
            using (var ms = new MemoryStream(payload))
            {
                using (var protocolReader = new CompactBinaryProtocolReader(ms))
                {
                    a.Read(protocolReader);
                }
            }
            return a;
        }
    }
}
