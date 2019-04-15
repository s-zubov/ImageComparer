using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace ImageComparer
{
    public class ImagePainter : IImagePainter
    {
        public ImagePainter(Color color, int width)
        {
            Color = color;
            Width = width;
        }

        public Color Color { get; set; }

        public int Width { get; set; }

        public void DrawDifferences(Bitmap image, IEnumerable<RectangleF> differences)
        {
            if(image == null)
                throw new ArgumentNullException(nameof(Image));
            
            if(differences == null)
                throw new ArgumentNullException(nameof(differences));
            
            var differencesArray = differences.ToArray();
            if(differencesArray.Length == 0)
                return;
            
            using (var graphics = Graphics.FromImage(image))
            using (var pen = new Pen(Color, Width))
                graphics.DrawRectangles(pen, differencesArray);
        }

        public async Task DrawDifferencesAsync(Bitmap image, IAsyncEnumerable<RectangleF> differences,
            object lockObject)
        {
            if(image == null)
                throw new ArgumentNullException(nameof(Image));
            
            if(differences == null)
                throw new ArgumentNullException(nameof(differences));
            
            using (var graphics = Graphics.FromImage(image))
            using (var pen = new Pen(Color, Width))
                await foreach (var difference in differences)
                    lock (lockObject)
                        graphics.DrawRectangles(pen, new[] {difference});
        }
    }
}