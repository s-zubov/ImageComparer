using System;

namespace ImageComparer
{
    public delegate void ImageProcessedEventHandler(object sender, ImageProcessedEventArgs e);
    
    public class ImageProcessedEventArgs : EventArgs
    {
        public ImageProcessedEventArgs(Guid guid)
        {
            Guid = guid;
        }

        public Guid Guid { get; set; }
    }
}