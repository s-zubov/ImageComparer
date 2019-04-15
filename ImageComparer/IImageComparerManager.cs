using System;
using System.Drawing;

namespace ImageComparer
{
    public interface IImageComparerManager
    {
        Guid Process(Bitmap left, Bitmap right, int threshold);
        
        Guid ProcessInBackground(Bitmap left, Bitmap right, int threshold);
        
        Bitmap GetImage(Guid guid);
        
        event ImageProcessedEventHandler ImageProcessed;
    }
}