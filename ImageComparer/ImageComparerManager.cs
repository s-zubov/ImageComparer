using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Threading.Tasks;
using ImageComparer.Algorithms;
using ImageComparer.Storage;

namespace ImageComparer
{
    public class ImageComparerManager : IImageComparerManager
    {
        private readonly IImageComparerAlgorithm _comparer;

        private readonly IImageStorage _imageStorage;

        private readonly IImagePainter _painter;

        private static readonly ConcurrentDictionary<Guid, ProcessingState> States;

        static ImageComparerManager()
        {
            States = new ConcurrentDictionary<Guid, ProcessingState>();
        }

        public ImageComparerManager(IImageComparerAlgorithm comparer, IImagePainter painter, IImageStorage imageStorage)
        {
            _comparer = comparer;

            _painter = painter;

            _imageStorage = imageStorage;
        }

        public Guid Process(Bitmap left, Bitmap right, int threshold)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            if (right == null)
                throw new ArgumentNullException(nameof(right));

            _painter.DrawDifferences(left, _comparer.GetDifferences(left, right, threshold));
            return _imageStorage.Create(new BitmapAndLock(left, new object()));
        }

        public Guid ProcessInBackground(Bitmap left, Bitmap right, int threshold)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            if (right == null)
                throw new ArgumentNullException(nameof(right));

            var copy = new Bitmap(left);
            var copyLock = new object();
            var guid = _imageStorage.Create(new BitmapAndLock(copy, copyLock));

            Task.Run(async () =>
            {
                States.TryAdd(guid, ProcessingState.InProgress);
                await _painter.DrawDifferencesAsync(copy,
                    _comparer.GetDifferencesAsync(left, right, threshold, new object()),
                    copyLock);
                States.TryUpdate(guid, ProcessingState.Completed, ProcessingState.InProgress);
                OnImageProcessed(new ImageProcessedEventArgs(guid));
            });

            return guid;
        }

        public Bitmap GetImage(Guid guid)
        {
            return _imageStorage.Read(guid).Bitmap;
        }

        public event ImageProcessedEventHandler ImageProcessed;

        public ProcessingState GetState(Guid guid)
        {
            if (!States.TryGetValue(guid, out var state))
                throw new InvalidOperationException($"Entity not found: '{guid}'");

            return state;
        }

        protected virtual void OnImageProcessed(ImageProcessedEventArgs e)
        {
            var handler = ImageProcessed;
            handler?.Invoke(this, e);
        }
    }
}