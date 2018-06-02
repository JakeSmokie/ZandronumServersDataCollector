using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZandronumServersDataCollector.Extensions {
    public static class BinaryReaderExtension {
        public static string ReadNullTerminatedString(this BinaryReader reader) {
            var buffer = new MemoryStream();

            while (true) {
                var b = reader.ReadByte();

                if (b == '\0')
                    break;

                buffer.WriteByte(b);
            }

            return Encoding.UTF8.GetString(buffer.ToArray());
        }
    }
}