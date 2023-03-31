using OpenCvSharp;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace Sdcb.PaddleOCR
{
    public class QueuedPaddleOcrAll : IDisposable
    {
        private readonly Func<PaddleOcrAll> _factory;
        private readonly BlockingCollection<ThreadedQueueItem> _queue;
        private readonly Task[] _workers;
        private readonly CountdownEvent _countdownEvent;
        private bool _disposed;

        public QueuedPaddleOcrAll(Func<PaddleOcrAll> factory, int consumerCount = 4, int boundedCapacity = 64)
        {
            _factory = factory;
            _queue = new BlockingCollection<ThreadedQueueItem>(boundedCapacity);
            _workers = new Task[consumerCount];
            _countdownEvent = new CountdownEvent(consumerCount);

            for (int i = 0; i < consumerCount; i++)
            {
                _workers[i] = Task.Run(ProcessQueue);
            }
        }

        public void WaitFactoryReady()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(QueuedPaddleOcrAll));

            _countdownEvent.Wait();
        }

        public Task<PaddleOcrResult> Run(Mat src, int recognizeBatchSize = 0, CancellationToken cancellationToken = default)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(QueuedPaddleOcrAll));

            TaskCompletionSource<PaddleOcrResult> tcs = new();
            cancellationToken.ThrowIfCancellationRequested();

            _queue.Add(new ThreadedQueueItem(src, recognizeBatchSize, cancellationToken, tcs), cancellationToken);

            return tcs.Task;
        }

        private void ProcessQueue()
        {
            using PaddleOcrAll paddleOcr = _factory();
            _countdownEvent.Signal();

            foreach (ThreadedQueueItem item in _queue.GetConsumingEnumerable())
            {
                if (item.CancellationToken.IsCancellationRequested || _disposed)
                {
                    item.TaskCompletionSource.SetCanceled();
                    continue;
                }

                try
                {
                    PaddleOcrResult result = paddleOcr.Run(item.Source, item.RecognizeBatchSize);
                    item.TaskCompletionSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    item.TaskCompletionSource.SetException(ex);
                }
            }
        }

        public void Dispose()
        {
            _disposed = true;
            _countdownEvent.Dispose();
            _queue.CompleteAdding();
            Task.WaitAll(_workers);
        }
    }

    internal record ThreadedQueueItem(Mat Source, int RecognizeBatchSize, CancellationToken CancellationToken, TaskCompletionSource<PaddleOcrResult> TaskCompletionSource);
}
