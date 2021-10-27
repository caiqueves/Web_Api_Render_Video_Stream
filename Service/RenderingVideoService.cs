using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi_Render_Video_Stream.Service
{
    public class RenderingVideoService : IRenderingVideoService
    {
        public IEnumerable<byte> ByteFormatVideo()
        {
            string filepath = @"C:\Users\caique_neves\Downloads\earth.mp4";
            var x = File.ReadAllBytes(filepath);

            return x.AsEnumerable();

        }
    }
}
