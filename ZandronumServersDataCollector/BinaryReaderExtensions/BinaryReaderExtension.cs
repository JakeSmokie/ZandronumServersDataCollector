using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZandronumServersDataCollector.BinaryReaderExtensions {
    public static class BinaryReaderExtension {
        public static string ReadNullTerminatedString(this BinaryReader reader) {
            var buffer = new List<byte>();
            byte b;

            do {
                b = reader.ReadByte();
                buffer.Add(b);
            } while (b != '\0');

            buffer.RemoveAt(buffer.Count - 1);

            return Encoding.UTF8.GetString(buffer.ToArray());
        }
    }
}