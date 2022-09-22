#nullable enable
using System.IO;

/***using System.Diagnostics.CodeAnalysis;***/

namespace /***$rootnamespace$.***/ULibs.SerializedEntryStore
{
    /***[ExcludeFromCodeCoverage]***/
    public interface ISerializedEntryStore
    {
        Stream Write(string entryName);

        Stream Read(string entryName);
    }
}