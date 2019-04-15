using System;

namespace ImageComparer.Storage
{
    public interface IImageStorage
    {
        Guid Create(BitmapAndLock bitmapAndLock);
        
        BitmapAndLock Read(Guid guid);
        
        void Delete(Guid guid);
    }
}