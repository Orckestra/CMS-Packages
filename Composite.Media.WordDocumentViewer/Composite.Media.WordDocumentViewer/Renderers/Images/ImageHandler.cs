using System.Web;
using DocumentFormat.OpenXml.Packaging;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Composite.Media.WordDocumentViewer.Renderers.Images
{
    public class ImageHandler : IHttpHandler
    {
        public bool IsReusable => true;

        public void ProcessRequest(HttpContext context)
        {
            var image = context.Request["image"];
            var mediaFile = context.Request["file"];

            using (WordprocessingDocument wpdocument = DocumentFacade.GetWordprocessingDocument(mediaFile))
            {
                OpenXmlPart imagePart = wpdocument.MainDocumentPart.GetPartById(image);
                if (imagePart == null)
                {
                    throw new FileNotFoundException("Part not found in document.", image);
                }

                // Set response code
                context.Response.StatusCode = 200;
                // Set content type
                context.Response.ContentType = imagePart.ContentType;
                // Render to output stream
                using (Bitmap bitmap = new Bitmap(imagePart.GetStream()))
                {
                    Size imageSize = new Size {Width = bitmap.Width, Height = bitmap.Height};
                    int newWidth, newHeight;
                    bool needToResize = CalculateSize(imageSize.Width, imageSize.Height,
                        out newWidth, out newHeight, context);

                    if (needToResize)
                    {
                        using (var resizedImage = new Bitmap(newWidth, newHeight))
                        {
                            resizedImage.SetResolution(72, 72);

                            using (var newGraphic = Graphics.FromImage(resizedImage))
                            {
                                newGraphic.Clear(Color.White);
                                newGraphic.SmoothingMode = SmoothingMode.AntiAlias;
                                newGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                newGraphic.DrawImage(bitmap, 0, 0, newWidth, newHeight);
                            }

                            using (var stream = new MemoryStream())
                            {
                                resizedImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                                stream.WriteTo(context.Response.OutputStream);
                            }
                        }
                    }
                    else
                    {
                        using (var ms = new MemoryStream())
                        {
                            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            ms.WriteTo(context.Response.OutputStream);
                        }
                    }
                }
                //	imagePart.GetStream().Read(outputBytes, 0, outputBytes.Length);
                //	context.Response.BinaryWrite(outputBytes);
            }
        }


        private bool CalculateSize(int width, int height, out int newWidth, out int newHeight, HttpContext context)
        {
            newWidth = width;
            newHeight = height;

            if (context.Request["w"] != null && context.Request["h"] != null)
            {
                var maxWidth = int.Parse(context.Request["w"]);
                var maxHeight = int.Parse(context.Request["h"]);
                // we do not allow scalling to a size, bigger than original one
                if (newHeight > maxHeight)
                {
                    newHeight = maxHeight;
                    newWidth = (int)(width * (double)maxHeight / height);
                }
                if (newWidth > maxWidth)
                {
                    newHeight = (int)(height * (double)maxWidth / width);
                    newWidth = maxWidth;
                }
                return newWidth != width || newHeight != height;
            }
            return false;
        }
    }
}
