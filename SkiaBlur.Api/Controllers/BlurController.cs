using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

namespace SkiaBlur.Api.Controllers
{
    [Route("api/[controller]")]
    public class BlurController : Controller 
    {
        [HttpPost]
        [Consumes("multipart/form-data")]
        public FileStreamResult BlurImage([FromForm] BlurImageRequest request)
        {
            using (var s = request.Image.OpenReadStream())
            {
                using (var originalImageBitmap = SKBitmap.Decode(s))
                {
                    using (var paint = new SKPaint{ImageFilter = SKImageFilter.CreateBlur(15, 15)})
                    {
                        using (var canvas = new SKCanvas(originalImageBitmap))
                        {
                            canvas.DrawBitmap(originalImageBitmap, 0, 0, paint);
                            canvas.Flush();

                            var resultStream = new MemoryStream();
                            using (var data = SKImage.FromBitmap(originalImageBitmap)
                                .Encode(SKEncodedImageFormat.Jpeg, 100))
                            {
                                data.SaveTo(resultStream);
                            };
                            resultStream.Position = 0;
                            
                            return new FileStreamResult(resultStream, "image/jpeg")
                            {
                                FileDownloadName = "blured",
                                EnableRangeProcessing = true
                            };
                        }
                    }   
                }
            }
        }
        
        public class BlurImageRequest
        {
            public IFormFile Image { get; set; }
        }
    }
}