using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;

namespace ImageComparer
{
    public class ImageStorage : IImageStorage
    {
        static ImageStorage()
        {
            Images = new ConcurrentDictionary<Guid, BitmapAndLock>();
        }

        private static readonly ConcurrentDictionary<Guid, BitmapAndLock> Images;

        public Guid Create(BitmapAndLock bitmapAndLock)
        {
            var guid = Guid.NewGuid();
            while (!Images.TryAdd(guid, bitmapAndLock))
            {
                // In case of guid collision
            }
            return guid;
        }

        public BitmapAndLock Read(Guid guid)
        {
            Images.TryGetValue(guid, out var image);
            return image;
        }

        public void Delete(Guid guid)
        {
            if(!Images.ContainsKey(guid))
                throw new InvalidOperationException($"Entity not found: '{guid}'");

            Images.TryRemove(guid, out _);
        }
    }
}