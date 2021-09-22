using System.Drawing;
using System.IO;
using Composite.Core.WebClient.Media;
using ImageProcessor;
using ImageProcessor.Plugins.WebP.Imaging.Formats;

namespace Orckestra.Media.ImageFormats.WebP
{
    internal class WebPImageFileFormatProvider: IImageFileFormatProvider
    {
        public string MediaType => "image/webp";

        public string FileExtension => "webp";

        public bool CanSetImageQuality => true;
        public bool CanReadImageSize => false;

        public bool TryGetSize(Stream imageStream, out Size size)
        {
            size = Size.Empty;
            return false;
        }

        public Bitmap LoadImageFromStream(Stream stream)
        {
            using (var imageFactory = new ImageFactory(preserveExifData: false))
            {
                imageFactory.Load(stream);

                var bitmap = (Bitmap)imageFactory.Image;

                return (Bitmap)bitmap.Clone();
            }
        }

        public void SaveImageToFile(Bitmap image, string outputFilePath, int? quality = null)
        {
            using (var webPFileStream = new FileStream(outputFilePath, FileMode.Create))
            {
                using (var imageFactory = new ImageFactory(preserveExifData: false))
                {
                    imageFactory.Load(image).Format(new WebPFormat());

                    if (quality != null)
                    {
                        imageFactory.Quality(quality.Value);
                    }

                    imageFactory.Save(webPFileStream);
                }
            }
        }
    }
}
