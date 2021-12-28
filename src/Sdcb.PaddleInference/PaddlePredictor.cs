using Sdcb.PaddleInference.Native;
using System;

namespace Sdcb.PaddleInference
{
    public class PaddlePredictor : IDisposable
    {
        private IntPtr _ptr;

        public PaddlePredictor(IntPtr predictorPointer)
        {
            if (predictorPointer == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(predictorPointer));
            }
            _ptr = predictorPointer;
        }

        public PaddlePredictor Clone() => new PaddlePredictor(PaddleNative.PD_PredictorClone(_ptr));

        public string[] InputNames
        {
            get
            {
                using PaddleNative.PdStringArrayWrapper wrapper = new() { ptr = PaddleNative.PD_PredictorGetInputNames(_ptr) };
                return wrapper.ToArray();
            }
        }

        public string[] OutputNames
        {
            get
            {
                using PaddleNative.PdStringArrayWrapper wrapper = new() { ptr = PaddleNative.PD_PredictorGetOutputNames(_ptr) };
                return wrapper.ToArray();
            }
        }

        public PaddleTensor GetInputTensor(string name) => new PaddleTensor(PaddleNative.PD_PredictorGetInputHandle(_ptr, name));
        public PaddleTensor GetOutputTensor(string name) => new PaddleTensor(PaddleNative.PD_PredictorGetOutputHandle(_ptr, name));

        public int InputSize => (int)PaddleNative.PD_PredictorGetInputNum(_ptr);
        public int OutputSize => (int)PaddleNative.PD_PredictorGetOutputNum(_ptr);

        public bool Run() => PaddleNative.PD_PredictorRun(_ptr) != 0;

        public void Dispose()
        {
            if (_ptr != IntPtr.Zero)
            {
                PaddleNative.PD_PredictorDestroy(_ptr);
                _ptr = IntPtr.Zero;
            }
        }
    }
}
