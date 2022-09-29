using OpenCvSharp;

namespace Sdcb.RotationDetector.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void NonRotatedImageShouldNoRotate()
        {
            using Mat dog = Cv2.ImRead("dog.jpg");
            using PaddleRotationDetector rd = new(RotationDetectionModel.EmbeddedDefault);
            RotationResult r = rd.Run(dog);
            Assert.Equal(RotationDegree._0, r.Rotation);
        }

        [Fact]
        public void Rotated90()
        {
            using Mat dog = Cv2.ImRead("dog.jpg");
            Cv2.Rotate(dog, dog, RotateFlags.Rotate90Clockwise);
            using PaddleRotationDetector rd = new(RotationDetectionModel.EmbeddedDefault);
            RotationResult r = rd.Run(dog);
            Assert.Equal(RotationDegree._90, r.Rotation);
        }

        [Fact]
        public void Rotated180()
        {
            using Mat dog = Cv2.ImRead("dog.jpg");
            Cv2.Rotate(dog, dog, RotateFlags.Rotate180);
            using PaddleRotationDetector rd = new(RotationDetectionModel.EmbeddedDefault);
            RotationResult r = rd.Run(dog);
            Assert.Equal(RotationDegree._180, r.Rotation);
        }

        [Fact]
        public void Rotated270()
        {
            using Mat dog = Cv2.ImRead("dog.jpg");
            Cv2.Rotate(dog, dog, RotateFlags.Rotate90Counterclockwise);
            using PaddleRotationDetector rd = new(RotationDetectionModel.EmbeddedDefault);
            RotationResult r = rd.Run(dog);
            Assert.Equal(RotationDegree._270, r.Rotation);
        }
    }
}