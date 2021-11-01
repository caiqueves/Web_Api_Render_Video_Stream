using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApi_Render_Video_Stream.Service;

namespace WebApi_Render_Video_Stream.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RenderingVideoController : Controller
    {
        private readonly IRenderingVideoService _renderingVideoService;

        string filepath = @"C:\Temp\Video\teste-grande-tempo.mp4"; //video file address

        public RenderingVideoController(IRenderingVideoService renderingVideoService) 
            => this._renderingVideoService = renderingVideoService;


        // Return format of blob using arrayBuffer
        [HttpGet("blob")]
        public async Task GetVideoAndBlob()
        {
            Stream iStream = null;
            byte[] buffer = new Byte[4096];
            int length;
            long dataToRead;

            try
            {
                // Open the file.
                iStream = new FileStream(filepath, FileMode.Open,
                            FileAccess.Read, FileShare.Read);

                // Total bytes to read:
                dataToRead = iStream.Length;

                Response.Headers["Accept-Ranges"] = "bytes";
                Response.ContentType = "application/octet-stream";

                int startbyte = 0;

                if (!String.IsNullOrEmpty(Request.Headers["Range"]))
                {
                    string[] range = Request.Headers["Range"].ToString().Split(new char[] { '=', '-' });
                    startbyte = Int32.Parse(range[1]);
                    iStream.Seek(startbyte, SeekOrigin.Begin);

                    Response.StatusCode = 206;
                    Response.Headers["Content-Range"] = String.Format(" bytes {0}-{1}/{2}", startbyte, dataToRead - 1, dataToRead);
                }
                var outputStream = this.Response.Body;
                while (dataToRead > 0)
                {
                    // Verify that the client is connected.
                    if (HttpContext.RequestAborted.IsCancellationRequested == false)
                    {
                        // Read the data in buffer.
                        length = await iStream.ReadAsync(buffer, 0, buffer.Length);

                        // Write the data to the current output stream.
                        await outputStream.WriteAsync(buffer, 0, buffer.Length);
                        // Flush the data to the HTML output.
                        outputStream.Flush();

                        buffer = new Byte[buffer.Length];
                        dataToRead = dataToRead - buffer.Length;
                    }
                    else
                    {
                        //prevent infinite loop if user disconnects
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception)
            {
                // Trap the error, if any.

            }
            finally
            {
                if (iStream != null)
                {
                    //Close the file.
                    iStream.Close();
                }
            }
        }
    
        // Return format of mp4 
        [HttpGet("mp4")]
        public IActionResult GetVideoAndMP4()
        {
            var arrayOfVideo = _renderingVideoService.ByteFormatVideo(filepath);

            return File(arrayOfVideo.ToArray(), "video/mp4", "video");
        }

        // Return format of stream
        [HttpGet("stream")]
        public IActionResult GetVideoAndStream()
            => PhysicalFile(filepath, "application/octet-stream", enableRangeProcessing: true);
        
    }
}
