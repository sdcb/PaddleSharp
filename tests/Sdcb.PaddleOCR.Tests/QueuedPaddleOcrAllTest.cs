namespace Sdcb.PaddleOCR.Tests;

public class QueuedPaddleOcrAllTest
{
    [Fact]
    public void FailedConstructorErrorWillThrow()
    {
        QueuedPaddleOcrAll? queue = null;
        try
        {
            AggregateException ex = Assert.Throws<AggregateException>(() =>
            {
                queue = new QueuedPaddleOcrAll(() => throw new Exception("Construction failed!"));
            });
            Assert.Equal("Construction failed!", ex.InnerExceptions.Single().Message);
        }
        finally
        {
            queue?.Dispose();
        }
    }
}
