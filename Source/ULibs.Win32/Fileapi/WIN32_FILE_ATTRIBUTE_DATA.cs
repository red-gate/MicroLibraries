using System;
using System.IO;

namespace /***$rootnamespace$.***/ULibs.Win32.Fileapi
{
    [Serializable]
    public struct WIN32_FILE_ATTRIBUTE_DATA
    {
        public FileAttributes fileAttributes;

        public uint ftCreationTimeLow;

        public uint ftCreationTimeHigh;

        public uint ftLastAccessTimeLow;

        public uint ftLastAccessTimeHigh;

        public uint ftLastWriteTimeLow;

        public uint ftLastWriteTimeHigh;

        public int fileSizeHigh;

        public int fileSizeLow;
    }
}
