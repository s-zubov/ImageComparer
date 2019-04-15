using System;
using System.Collections.Concurrent;

namespace ImageComparer.Storage
{
    public class InnerImageStorage : IImageStorage
    {
        private static readonly ConcurrentDictionary<Guid, BitmapAndLock> Images;

        static InnerImageStorage()
        {
            Images = new ConcurrentDictionary<Guid, BitmapAndLock>();
        }

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
            if (!Images.ContainsKey(guid))
                throw new InvalidOperationException($"Entity not found: '{guid}'");

            Images.TryRemove(guid, out _);
        }
    }
}