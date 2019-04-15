using System.Drawing;

namespace ImageComparer
{
    public class BitmapAndLock
    {
        public Bitmap Bitmap { get; set; }
        
        public object SyncRoot { get; set; }

        public BitmapAndLock()
        {
        }

        public BitmapAndLock(Bitmap bitmap, object syncRoot)
        {
            Bitmap = bitmap;
            SyncRoot = syncRoot;
        }
    }
}