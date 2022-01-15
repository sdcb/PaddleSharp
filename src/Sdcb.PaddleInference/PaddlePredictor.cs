using Sdcb.PaddleInference.Native;
using System;
using System.Text;

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

        public PaddlePredictor Clone() => new(PaddleNative.PD_PredictorClone(_ptr));

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

        public unsafe PaddleTensor GetInputTensor(string name)
        {
            byte[] nameBytes = Encoding.UTF8.GetBytes(name);
            fixed(byte* ptr = nameBytes)
            {
                return new PaddleTensor(PaddleNative.PD_PredictorGetInputHandle(_ptr, (IntPtr)ptr));
            }
        }

        public unsafe PaddleTensor GetOutputTensor(string name)
        {
            byte[] nameBytes = Encoding.UTF8.GetBytes(name);
            fixed (byte* ptr = nameBytes)
            {
                return new PaddleTensor(PaddleNative.PD_PredictorGetOutputHandle(_ptr, (IntPtr)ptr));
            }
        }

        public int InputSize => PaddleNative.PD_PredictorGetInputNum(_ptr);
        public int OutputSize => PaddleNative.PD_PredictorGetOutputNum(_ptr);

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
