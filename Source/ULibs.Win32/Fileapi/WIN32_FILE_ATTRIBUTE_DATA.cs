using System;
using System.IO;

namespace /***$rootnamespace$.***/ULibs.Win32.Fileapi
{
    [Serializable]
    internal struct WIN32_FILE_ATTRIBUTE_DATA
    {
        internal FileAttributes fileAttributes;

        internal uint ftCreationTimeLow;

        internal uint ftCreationTimeHigh;

        internal uint ftLastAccessTimeLow;

        internal uint ftLastAccessTimeHigh;

        internal uint ftLastWriteTimeLow;

        internal uint ftLastWriteTimeHigh;

        internal int fileSizeHigh;

        internal int fileSizeLow;
    }
}
