using OpenCvSharp;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace Sdcb.PaddleOCR;

/// <summary>
/// A class for queuing multiple OCR requests using PaddleOCR.
/// </summary>
public class QueuedPaddleOcrAll : IDisposable
{
    private readonly Func<PaddleOcrAll> _factory;
    private readonly BlockingCollection<ThreadedQueueItem> _queue;
    private readonly Task[] _workers;
    private readonly CountdownEvent _countdownEvent;
    private readonly ConcurrentBag<Exception> _constructExceptions = new ConcurrentBag<Exception>();
    private bool _disposed;

    /// <summary>
    /// Constructs an instance of <see cref="QueuedPaddleOcrAll"/> class.
    /// </summary>
    /// <param name="factory">The function that constructs each individual instance of <see cref="PaddleOcrAll"/>.</param>
    /// <param name="consumerCount">The number of consumers that process the OCR requests.</param>
    /// <param name="boundedCapacity">The maximum number of queued OCR requests.</param>
    public QueuedPaddleOcrAll(Func<PaddleOcrAll> factory, int consumerCount = 1, int boundedCapacity = 64)
    {
        _factory = factory;
        _queue = new BlockingCollection<ThreadedQueueItem>(boundedCapacity);
        _workers = new Task[consumerCount];
        _countdownEvent = new CountdownEvent(consumerCount);

        for (int i = 0; i < consumerCount; i++)
        {
            _workers[i] = Task.Run(ProcessQueue);
        }

        try
        {
            WaitFactoryReady();
        }
        catch (AggregateException)
        {
            Dispose();
            throw;
        }
    }

    /// <summary>
    /// Waits for the factory to become ready before processing OCR requests.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The instance of <see cref="QueuedPaddleOcrAll"/> is disposed.</exception>
    private void WaitFactoryReady()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(QueuedPaddleOcrAll));

        _countdownEvent.Wait();
        if (_constructExceptions.Any())
        {
            throw new AggregateException(_constructExceptions);
        }
    }

    /// <summary>
    /// Queues an OCR request to be processed.
    /// </summary>
    /// <param name="src">The image to be recognized.</param>
    /// <param name="recognizeBatchSize">
    /// The number of images recognized with one call. 
    /// Zero means single recognition only. Maximum value is limited by the model and hardware.
    /// </param>
    /// <param name="configure">
    /// Configuration action to customize the <see cref="PaddleOcrAll"/> instance before running the OCR.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> that represents the queued OCR operation.</returns>
    /// <exception cref="ObjectDisposedException">The instance of <see cref="QueuedPaddleOcrAll"/> is disposed.</exception>
    public Task<PaddleOcrResult> Run(Mat src, int recognizeBatchSize = 0, Action<PaddleOcrAll>? configure = null, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(QueuedPaddleOcrAll));

        TaskCompletionSource<PaddleOcrResult> tcs = new();
        cancellationToken.ThrowIfCancellationRequested();

        _queue.Add(new ThreadedQueueItem(src, recognizeBatchSize, configure, cancellationToken, tcs), cancellationToken);

        return tcs.Task;
    }

    private void ProcessQueue()
    {
        PaddleOcrAll paddleOcr = null!;
        try
        {
            paddleOcr = _factory();
        }
        catch (Exception e)
        {
            _constructExceptions.Add(e);
        }
        finally
        {
            _countdownEvent.Signal();
        }

        using PaddleOcrAll _ = paddleOcr;
        foreach (ThreadedQueueItem item in _queue.GetConsumingEnumerable())
        {
            if (item.CancellationToken.IsCancellationRequested || _disposed)
            {
                item.TaskCompletionSource.SetCanceled();
                continue;
            }

            try
            {
                item.Configure?.Invoke(paddleOcr);
                PaddleOcrResult result = paddleOcr.Run(item.Source, item.RecognizeBatchSize);
                item.TaskCompletionSource.SetResult(result);
            }
            catch (Exception ex)
            {
                item.TaskCompletionSource.SetException(ex);
            }
        }
    }

    /// <summary>
    /// Disposes this instance of <see cref="QueuedPaddleOcrAll"/> and releases associated resources.
    /// </summary>
    public void Dispose()
    {
        _disposed = true;
        _queue.CompleteAdding();
        Task.WaitAll(_workers);
        _countdownEvent.Dispose();
    }
}

internal record ThreadedQueueItem(
    Mat Source, 
    int RecognizeBatchSize, 
    Action<PaddleOcrAll>? Configure,
    CancellationToken CancellationToken, 
    TaskCompletionSource<PaddleOcrResult> TaskCompletionSource);
