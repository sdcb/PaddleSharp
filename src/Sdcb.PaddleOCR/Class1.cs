using Sdcb.PaddleInference;

namespace Sdcb.PaddleOCR
{
    public abstract class OcrDetectionModel
    {
        public abstract (int channel, int height, int width) SuggestedSize { get; }

        public abstract PaddleConfig CreateConfig();
    }

    public abstract class OcrRecognizationModel
    {
        public abstract OcrModelVersion OcrModelVersion { get; }

        public abstract string GetLabelByIndex(int i);

        public abstract PaddleConfig CreateConfig();
    }

    public abstract class OcrClassificationModel
    {
        public abstract (int channel, int height, int width) SuggestedSize { get; }
        public abstract PaddleConfig CreateConfig();
    }

    public class FullOcrModel
    {
        OcrDetectionModel DetectionModel { get; }

        OcrClassificationModel? ClassificationModel { get; }

        OcrRecognizationModel RecognizationModel { get; }
    }

    public enum OcrModelVersion
    {
        V2, V3, 
    }
}
