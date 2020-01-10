using Composite.Core;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.IO.Compression;

namespace Orckestra.Web.BundlingAndMinification
{
    public class ResponseFilterStream : Stream, IDisposable
    {
        private readonly Stream _responseStream;
        private readonly MemoryStream _ms = new MemoryStream();
        private readonly Encoding _encoding;

        public ResponseFilterStream(
            Stream responseStream, 
            Encoding encoding, 
            bool bundleMinifyScripts, 
            bool bundleMinifyStyles, 
            bool removeComments)
        {
            _responseStream = responseStream ?? throw new ArgumentNullException(nameof(responseStream));
            _encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            BundleMinifyScripts = bundleMinifyScripts;
            BundleMinifyStyles = bundleMinifyStyles;
            RemoveComments = removeComments;
        }

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override void Flush() { }

        public override long Length => throw new NotSupportedException();

        public bool BundleMinifyScripts { get; }
        public bool BundleMinifyStyles { get; }

        public bool RemoveComments { get; }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Close()
        {
            if (!_ms.CanSeek || _ms.Length.Equals(0))
            {
                _ms.Close();
                _responseStream.Close();
                return;
            }
            /* If server uses compression, received via Response.Filter bytes will be compressed and to modify data we have 
             * to decompress them first. From productivity point of view it looks better to turn off server compression at all 
             * and to provide compression by this module. But it seems to be too specific and complicated for end user, so I 
             * kept simplier solution */
            bool decompressStreamSuccess = DecompressStream(out DecompressionMethods decompressionMethod, out byte[] bytes);
            if (decompressStreamSuccess)
            {
                bytes = OptimizationManager.GetOptimizedBytes(bytes, _encoding, BundleMinifyScripts, BundleMinifyStyles, RemoveComments);
                bytes = OptimizationManager.GetCompressedBytes(bytes, decompressionMethod);
            }
            _responseStream.Write(bytes, 0, bytes.Length);
            _responseStream.Close();
        }

        private bool DecompressStream(out DecompressionMethods decompressionMethod, out byte[] decompressedBytes)
        {
            _ms.Seek(0, SeekOrigin.Begin);
            decompressionMethod = DecompressionMethods.None;
            decompressedBytes = _ms.ToArray();

            string encoding = HttpContext.Current.Response.Headers["Content-Encoding"];

            if (encoding is null) 
            {
                _ms.Close();
                return true;
            }
            else if (encoding.Equals(DecompressionMethods.GZip.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                decompressionMethod = DecompressionMethods.GZip;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (GZipStream gzipStream = new GZipStream(_ms, CompressionMode.Decompress))
                    {
                        gzipStream.CopyTo(ms);
                    }
                    decompressedBytes = ms.ToArray();
                }
            }
            else if (encoding.Equals(DecompressionMethods.Deflate.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                decompressionMethod = DecompressionMethods.Deflate;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (DeflateStream deflateStream = new DeflateStream(_ms, CompressionMode.Decompress))
                    {
                        deflateStream.CopyTo(ms);
                    }
                    decompressedBytes = ms.ToArray();
                }  
            }
            else
            {
                Log.LogError(nameof(ResponseFilterStream), $"Encoding {encoding} cannot be recognized");
                _ms.Close();
                HttpContext.Current.Response.Headers.Remove("Content-Encoding");
                return false;
            }
            _ms.Close();
            return true;
        }

        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => _ms.Write(buffer, offset, count);
    }
}