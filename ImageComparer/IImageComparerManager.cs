using System;
using System.Drawing;
using System.Threading.Tasks;

namespace ImageComparer
{
    public interface IImageComparerManager
    {
        Guid Process(Bitmap left, Bitmap right);
        Guid ProcessInBackground(Bitmap left, Bitmap right);
        Bitmap GetImage(Guid guid);
        event ImageProcessedEventHandler ImageProcessed;
    }
}