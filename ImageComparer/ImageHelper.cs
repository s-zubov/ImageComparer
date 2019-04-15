using System.Drawing;
using System.IO;

namespace ImageComparer
{
    public class ImageHelper
    {
        public static byte[] ImageToByteArray(Image imageIn)  
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }  
    }
}