
using System.Collections.Generic;

namespace WebApi_Render_Video_Stream.Service
{
    public interface IRenderingVideoService
    {
        IEnumerable<byte> ByteFormatVideo(string path);
    }
}
