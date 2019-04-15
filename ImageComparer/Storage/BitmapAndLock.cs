using System.Drawing;

namespace ImageComparer.Storage
{
    public class BitmapAndLock
    {
        public BitmapAndLock()
        {
        }

        public BitmapAndLock(Bitmap bitmap, object syncRoot)
        {
            Bitmap = bitmap;
            SyncRoot = syncRoot;
        }

        public Bitmap Bitmap { get; set; }

        public object SyncRoot { get; set; }
    }
}