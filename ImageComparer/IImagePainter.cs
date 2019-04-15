using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace ImageComparer
{
    public interface IImagePainter
    {
        Color Color { get; set; }
        int Width { get; set; }
        void DrawDifferences(Bitmap image, IEnumerable<RectangleF> differences);

        Task DrawDifferencesAsync(Bitmap image, IAsyncEnumerable<RectangleF> differences,
            object lockObject);
    }
}