using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Orckestra.Search.IFilterParser
{
    [Flags]
    public enum IFILTER_INIT : uint
    {
        NONE = 0,
        CANON_PARAGRAPHS = 1,
        HARD_LINE_BREAKS = 2,
        CANON_HYPHENS = 4,
        CANON_SPACES = 8,
        APPLY_INDEX_ATTRIBUTES = 16,
        APPLY_CRAWL_ATTRIBUTES = 256,
        APPLY_OTHER_ATTRIBUTES = 32,
        INDEXING_ONLY = 64,
        SEARCH_LINKS = 128,
        FILTER_OWNED_VALUE_OK = 512
    }

    public enum CHUNK_BREAKTYPE
    {
        CHUNK_NO_BREAK = 0,
        CHUNK_EOW = 1,
        CHUNK_EOS = 2,
        CHUNK_EOP = 3,
        CHUNK_EOC = 4
    }

    [Flags]
    public enum CHUNKSTATE
    {
        CHUNK_TEXT = 0x1,
        CHUNK_VALUE = 0x2,
        CHUNK_FILTER_OWNED_VALUE = 0x4
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PROPSPEC
    {
        public uint ulKind;
        public uint propid;
        public IntPtr lpwstr;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FULLPROPSPEC
    {
        public Guid guidPropSet;
        public PROPSPEC psProperty;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct STAT_CHUNK
    {
        public uint idChunk;
        [MarshalAs(UnmanagedType.U4)]
        public CHUNK_BREAKTYPE breakType;
        [MarshalAs(UnmanagedType.U4)]
        public CHUNKSTATE flags;
        public uint locale;
        [MarshalAs(UnmanagedType.Struct)]
        public FULLPROPSPEC attribute;
        public uint idChunkSource;
        public uint cwcStartSource;
        public uint cwcLenSource;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FILTERREGION
    {
        public uint idChunk;
        public uint cwcStart;
        public uint cwcExtent;
    }

    [ComImport]
    [Guid("89BCB740-6119-101A-BCB7-00DD010655AF")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFilter
    {
        [PreserveSig]
        int Init([MarshalAs(UnmanagedType.U4)] IFILTER_INIT grfFlags, uint cAttributes, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] FULLPROPSPEC[] aAttributes, ref uint pdwFlags);

        [PreserveSig]
        int GetChunk(out STAT_CHUNK pStat);

        [PreserveSig]
        int GetText(ref uint pcwcBuffer, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder buffer);

        void GetValue(ref UIntPtr ppPropValue);
        void BindRegion([MarshalAs(UnmanagedType.Struct)] FILTERREGION origPos, ref Guid riid, ref UIntPtr ppunk);
    }

    [ComImport]
    [Guid("f07f3920-7b8c-11cf-9be8-00aa004b9986")]
    public class CFilter
    {
    }

    public class IFilterConstants
    {
        public const uint PID_STG_DIRECTORY = 0x00000002;
        public const uint PID_STG_CLASSID = 0x00000003;
        public const uint PID_STG_STORAGETYPE = 0x00000004;
        public const uint PID_STG_VOLUME_ID = 0x00000005;
        public const uint PID_STG_PARENT_WORKID = 0x00000006;
        public const uint PID_STG_SECONDARYSTORE = 0x00000007;
        public const uint PID_STG_FILEINDEX = 0x00000008;
        public const uint PID_STG_LASTCHANGEUSN = 0x00000009;
        public const uint PID_STG_NAME = 0x0000000a;
        public const uint PID_STG_PATH = 0x0000000b;
        public const uint PID_STG_SIZE = 0x0000000c;
        public const uint PID_STG_ATTRIBUTES = 0x0000000d;
        public const uint PID_STG_WRITETIME = 0x0000000e;
        public const uint PID_STG_CREATETIME = 0x0000000f;
        public const uint PID_STG_ACCESSTIME = 0x00000010;
        public const uint PID_STG_CHANGETIME = 0x00000011;
        public const uint PID_STG_CONTENTS = 0x00000013;
        public const uint PID_STG_SHORTNAME = 0x00000014;
        public const int FILTER_E_END_OF_CHUNKS = (unchecked((int)0x80041700));
        public const int FILTER_E_NO_MORE_TEXT = (unchecked((int)0x80041701));
        public const int FILTER_E_NO_MORE_VALUES = (unchecked((int)0x80041702));
        public const int FILTER_E_NO_TEXT = (unchecked((int)0x80041705));
        public const int FILTER_E_NO_VALUES = (unchecked((int)0x80041706));
        public const int FILTER_S_LAST_TEXT = (unchecked((int)0x00041709));
    }

    /// 
    /// IFilter return codes
    /// 
    public enum IFilterReturnCodes : uint
    {
        /// 
        /// Success
        /// 
        S_OK = 0,
        /// 
        /// The function was denied access to the filter file. 
        /// 
        E_ACCESSDENIED = 0x80070005,
        /// 
        /// The function encountered an invalid handle, probably due to a low-memory situation. 
        /// 
        E_HANDLE = 0x80070006,
        /// 
        /// The function received an invalid parameter.
        /// 
        E_INVALIDARG = 0x80070057,
        /// 
        /// Out of memory
        /// 
        E_OUTOFMEMORY = 0x8007000E,
        /// 
        /// Not implemented
        /// 
        E_NOTIMPL = 0x80004001,
        /// 
        /// Unknown error
        /// 
        E_FAIL = 0x80000008,
        /// 
        /// File not filtered due to password protection
        /// 
        FILTER_E_PASSWORD = 0x8004170B,
        /// 
        /// The document format is not recognized by the filter
        /// 
        FILTER_E_UNKNOWNFORMAT = 0x8004170C,
        /// 
        /// No text in current chunk
        /// 
        FILTER_E_NO_TEXT = 0x80041705,
        /// 
        /// No more chunks of text available in object
        /// 
        FILTER_E_END_OF_CHUNKS = 0x80041700,
        /// 
        /// No more text available in chunk
        /// 
        FILTER_E_NO_MORE_TEXT = 0x80041701,
        /// 
        /// No more property values available in chunk
        /// 
        FILTER_E_NO_MORE_VALUES = 0x80041702,
        /// 
        /// Unable to access object
        /// 
        FILTER_E_ACCESS = 0x80041703,
        /// 
        /// Moniker doesn't cover entire region
        /// 
        FILTER_W_MONIKER_CLIPPED = 0x00041704,
        /// 
        /// Unable to bind IFilter for embedded object
        /// 
        FILTER_E_EMBEDDING_UNAVAILABLE = 0x80041707,
        /// 
        /// Unable to bind IFilter for linked object
        /// 
        FILTER_E_LINK_UNAVAILABLE = 0x80041708,
        /// 
        /// This is the last text in the current chunk
        /// 
        FILTER_S_LAST_TEXT = 0x00041709,
        /// 
        /// This is the last value in the current chunk
        /// 
        FILTER_S_LAST_VALUES = 0x0004170A
    }

    /// 
    /// Convenience class which provides static methods to extract text from files using installed IFilters
    /// 
    public class FilterFacade
    {
        const int BufferLength = 65536;

        [DllImport("query.dll", CharSet = CharSet.Unicode)]
        private static extern int LoadIFilter(string pwcsPath, [MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter, ref IFilter ppIUnk);

        private static IFilter loadIFilter(string filename, out int resultCode)
        {
            object outer = null;
            IFilter filter = null;

            // Try to load the corresponding IFilter
            resultCode = LoadIFilter(filename, outer, ref filter);
            if (resultCode != (int)IFilterReturnCodes.S_OK)
            {
                return null;
            }
            return filter;
        }


        public static bool IsParseable(string filename)
        {
            var filter = loadIFilter(filename, out int errorCode);
            if (filter != null)
            {
                Marshal.ReleaseComObject(filter);
            }

            return filter != null;
        }


        public static int Extract(string path, StreamWriter streamWriter)
        {
            //var sb = new StringBuilder();
            IFilter filter = null;

            try
            {
                int responseCode;
                filter = loadIFilter(path, out responseCode);

                if (filter == null) return responseCode;

                uint i = 0;

                var iflags =
                    IFILTER_INIT.CANON_HYPHENS |
                    IFILTER_INIT.CANON_PARAGRAPHS |
                    IFILTER_INIT.CANON_SPACES |
                    IFILTER_INIT.APPLY_CRAWL_ATTRIBUTES |
                    IFILTER_INIT.APPLY_INDEX_ATTRIBUTES |
                    IFILTER_INIT.APPLY_OTHER_ATTRIBUTES |
                    IFILTER_INIT.HARD_LINE_BREAKS |
                    IFILTER_INIT.SEARCH_LINKS |
                    IFILTER_INIT.FILTER_OWNED_VALUE_OK;

                if (filter.Init(iflags, 0, null, ref i) != (int)IFilterReturnCodes.S_OK)
                    throw new Exception($"Problem initializing an IFilter for:\n{path}\n\n");

                var sbBuffer = new StringBuilder(BufferLength);

                while (filter.GetChunk(out STAT_CHUNK ps) == (int)IFilterReturnCodes.S_OK)
                {
                    if (ps.flags == CHUNKSTATE.CHUNK_TEXT)
                    {
                        IFilterReturnCodes scode = 0;
                        while (scode == IFilterReturnCodes.S_OK || scode == IFilterReturnCodes.FILTER_S_LAST_TEXT)
                        {
                            uint pcwcBuffer = BufferLength;
                            scode = (IFilterReturnCodes)filter.GetText(ref pcwcBuffer, sbBuffer);

                            if (pcwcBuffer > 0 && sbBuffer.Length > 0)
                            {
                                if (sbBuffer.Length < pcwcBuffer) // Should never happen, but it happens !
                                    pcwcBuffer = (uint)sbBuffer.Length;

                                string fragment = sbBuffer.ToString(0, (int)pcwcBuffer);

                                streamWriter.Write(fragment);
                                // Adding a space so the last word of the previous paragraph will not be merged with the
                                // first word of the next paragraph
                                streamWriter.Write(" ");

                            }

                            sbBuffer.Clear();
                        }
                    }
                }
            }
            finally
            {
                if (filter != null)
                {
                    Marshal.ReleaseComObject(filter);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }

            return 0;
        }

        public static string GetResposeMessage(int resultCode)
        {
            return Enum.GetName(typeof(IFilterReturnCodes), (uint)resultCode)
                ?? ("0x" + ((uint)resultCode).ToString("x"));
        }
    }
}