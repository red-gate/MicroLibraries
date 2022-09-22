using System.Collections.Generic;
using System.IO;

namespace ULibs.SerializedEntryStore
{
    public class InMemorySerializedEntryStore: ISerializedEntryStore
    {
        private readonly Dictionary<string, MemoryStream> m_Entries = new();

        public Stream Write(string entryName)
        {
            var entry = new MemoryStream();
            m_Entries.Add(entryName, entry);
            return entry;
        }

        public Stream Read(string entryName) => new MemoryStream(m_Entries[entryName].ToArray());
    }
}