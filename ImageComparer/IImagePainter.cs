using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace ImageComparer
{
    public interface IImagePainter
    {
        void DrawDifferences(Bitmap image, IEnumerable<RectangleF> differences);

        Task DrawDifferencesAsync(Bitmap image, IAsyncEnumerable<RectangleF> differences,
            object lockObject);
    }
}