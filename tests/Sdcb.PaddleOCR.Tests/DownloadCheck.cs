using OpenCvSharp;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Online;
using Xunit.Abstractions;

namespace Sdcb.PaddleOCR.Tests
{
    public class DownloadCheckTest
    {
        private readonly ITestOutputHelper _console;

        public DownloadCheckTest(ITestOutputHelper console)
        {
            _console = console;
        }

        [Fact]
        public async Task DownloadCheck()
        {
            await LocalDictOnlineRecognizationModel.ChineseMobileV2.DownloadAsync();
        }
    }
}