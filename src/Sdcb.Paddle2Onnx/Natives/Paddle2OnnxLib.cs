using System;
using System.Runtime.InteropServices;

namespace Sdcb.Paddle2Onnx.Natives;

internal static class Paddle2OnnxLib
{
    const string Dll = @"C:\_\3rd\paddle\inference_cpp\third_party\install\paddle2onnx\lib\paddle2onnx.dll";

    // public: __cdecl paddle2onnx::PaddleReader::PaddleReader(char const * __ptr64,int) __ptr64
    [DllImport(Dll, EntryPoint = "??0PaddleReader@paddle2onnx@@QEAA@PEBDH@Z")]
    public static extern void PaddleReaderInit(ref CPaddleReader pthis, [MarshalAs(UnmanagedType.LPArray)] byte[] modelBuffer, int bufferSize);

    // public: struct paddle2onnx::PaddleReader & __ptr64 __cdecl paddle2onnx::PaddleReader::operator=(struct paddle2onnx::PaddleReader && __ptr64) __ptr64
    [DllImport(Dll, EntryPoint = "??4PaddleReader@paddle2onnx@@QEAAAEAU01@$$QEAU01@@Z")]
    public static extern IntPtr PaddleReaderAssign(ref CPaddleReader pthis, ref CPaddleReader src);

    // public: struct paddle2onnx::PaddleReader & __ptr64 __cdecl paddle2onnx::PaddleReader::operator=(struct paddle2onnx::PaddleReader const & __ptr64) __ptr64
    [DllImport(Dll, EntryPoint = "??4PaddleReader@paddle2onnx@@QEAAAEAU01@AEBU01@@Z")]
    public static extern IntPtr PaddleReaderAssignConst(ref CPaddleReader pthis, in CPaddleReader src);

    // public: int __cdecl paddle2onnx::PaddleReader::GetInputIndex(char const * __ptr64)const __ptr64
    [DllImport(Dll, EntryPoint = "?GetInputIndex@PaddleReader@paddle2onnx@@QEBAHPEBD@Z")]
    public static extern int PaddleReaderGetInputIndex(ref CPaddleReader pthis, [MarshalAs(UnmanagedType.LPStr)] string name);

    // public: int __cdecl paddle2onnx::PaddleReader::NumOutputs(void)const __ptr64
    [DllImport(Dll, EntryPoint = "?NumOutputs@PaddleReader@paddle2onnx@@QEBAHXZ")]
    public static extern int PaddleReaderNumOutputs(ref CPaddleReader pthis);

    // public: int __cdecl paddle2onnx::PaddleReader::NumInputs(void)const __ptr64
    [DllImport(Dll, EntryPoint = "?NumInputs@PaddleReader@paddle2onnx@@QEBAHXZ")]
    public static extern int PaddleReaderNumInputs(ref CPaddleReader pthis);

    // public: int __cdecl paddle2onnx::PaddleReader::GetOutputIndex(char const * __ptr64)const __ptr64
    [DllImport(Dll, EntryPoint = "?GetOutputIndex@PaddleReader@paddle2onnx@@QEBAHPEBD@Z")]
    public static extern int PaddleReaderGetOutputIndex(ref CPaddleReader pthis, [MarshalAs(UnmanagedType.LPStr)] string name);





    // public: __cdecl paddle2onnx::CustomOp::CustomOp(void) __ptr64
    [DllImport(Dll, EntryPoint = "??0CustomOp@paddle2onnx@@QEAA@XZ")]
    public static extern void CustomOpInit(ref CCustomOp pthis);

    // public: struct paddle2onnx::CustomOp & __ptr64 __cdecl paddle2onnx::CustomOp::operator=(struct paddle2onnx::CustomOp && __ptr64) __ptr64
    [DllImport(Dll, EntryPoint = "??4CustomOp@paddle2onnx@@QEAAAEAU01@$$QEAU01@@Z")]
    public static extern IntPtr CustomOpAssign(ref CCustomOp op, ref CCustomOp src);

    // public: struct paddle2onnx::CustomOp & __ptr64 __cdecl paddle2onnx::CustomOp::operator=(struct paddle2onnx::CustomOp const & __ptr64) __ptr64
    [DllImport(Dll, EntryPoint = "??4CustomOp@paddle2onnx@@QEAAAEAU01@AEBU01@@Z")]
    public static extern IntPtr CustomOpAssignConst(ref CCustomOp op, in CCustomOp src);





    // public: __cdecl paddle2onnx::ModelTensorInfo::ModelTensorInfo(void) __ptr64
    [DllImport(Dll, EntryPoint = "??0ModelTensorInfo@paddle2onnx@@QEAA@XZ")]
    public static extern void ModelTensorInfoInit(ref CModelTensorInfo info);

    // public: __cdecl paddle2onnx::ModelTensorInfo::~ModelTensorInfo(void) __ptr64
    [DllImport(Dll, EntryPoint = "??1ModelTensorInfo@paddle2onnx@@QEAA@XZ")]
    public static extern void ModelTensorInfoRelease(ref CModelTensorInfo info);

    // public: struct paddle2onnx::ModelTensorInfo & __ptr64 __cdecl paddle2onnx::ModelTensorInfo::operator=(struct paddle2onnx::ModelTensorInfo const & __ptr64) __ptr64
    [DllImport(Dll, EntryPoint = "??4ModelTensorInfo@paddle2onnx@@QEAAAEAU01@AEBU01@@Z")]
    public static extern IntPtr ModelTensorInfoAssign(ref CModelTensorInfo info, in CModelTensorInfo src);




    // public: __cdecl paddle2onnx::OnnxReader::OnnxReader(char const * __ptr64,int) __ptr64
    [DllImport(Dll, EntryPoint = "??0OnnxReader@paddle2onnx@@QEAA@PEBDH@Z")]
    public static extern void OnnxReaderInit(ref COnnxReader pthis, [MarshalAs(UnmanagedType.LPArray)] byte[] modelBuffer, int bufferSize);

    // public: struct paddle2onnx::OnnxReader & __ptr64 __cdecl paddle2onnx::OnnxReader::operator=(struct paddle2onnx::OnnxReader && __ptr64) __ptr64
    [DllImport(Dll, EntryPoint = "??4OnnxReader@paddle2onnx@@QEAAAEAU01@$$QEAU01@@Z")]
    public static extern IntPtr OnnxReaderAssign(ref COnnxReader dest, ref COnnxReader src);

    // public: struct paddle2onnx::OnnxReader & __ptr64 __cdecl paddle2onnx::OnnxReader::operator=(struct paddle2onnx::OnnxReader const & __ptr64) __ptr64
    [DllImport(Dll, EntryPoint = "??4OnnxReader@paddle2onnx@@QEAAAEAU01@AEBU01@@Z")]
    public static extern IntPtr OnnxReaderAssignConst(ref COnnxReader dest, in COnnxReader src);

    // public: int __cdecl paddle2onnx::OnnxReader::GetInputIndex(char const * __ptr64)const __ptr64
    [DllImport(Dll, EntryPoint = "?GetInputIndex@OnnxReader@paddle2onnx@@QEBAHPEBD@Z")]
    public static extern int OnnxReaderGetInputIndex(ref COnnxReader pthis, [MarshalAs(UnmanagedType.LPStr)] string name);

    // public: void __cdecl paddle2onnx::OnnxReader::GetInputInfo(int,struct paddle2onnx::ModelTensorInfo * __ptr64)const __ptr64
    [DllImport(Dll, EntryPoint = "?GetInputInfo@OnnxReader@paddle2onnx@@QEBAXHPEAUModelTensorInfo@2@@Z")]
    public static extern void OnnxReaderGetInputInfo(ref COnnxReader pthis, int index, ref CModelTensorInfo info);

    // public: int __cdecl paddle2onnx::OnnxReader::GetOutputIndex(char const * __ptr64)const __ptr64
    [DllImport(Dll, EntryPoint = "?GetOutputIndex@OnnxReader@paddle2onnx@@QEBAHPEBD@Z")]
    public static extern int OnnxReaderGetOutputIndex(ref COnnxReader pthis, [MarshalAs(UnmanagedType.LPStr)] string name);

    // public: void __cdecl paddle2onnx::OnnxReader::GetOutputInfo(int,struct paddle2onnx::ModelTensorInfo * __ptr64)const __ptr64
    [DllImport(Dll, EntryPoint = "?GetOutputInfo@OnnxReader@paddle2onnx@@QEBAXHPEAUModelTensorInfo@2@@Z")]
    public static extern void OnnxReaderGetOutputInfo(ref COnnxReader pthis, int index, ref CModelTensorInfo info);

    // public: int __cdecl paddle2onnx::OnnxReader::NumInputs(void)const __ptr64
    [DllImport(Dll, EntryPoint = "?NumInputs@OnnxReader@paddle2onnx@@QEBAHXZ")]
    public static extern int OnnxReaderNumInputs(ref COnnxReader pthis);

    // public: int __cdecl paddle2onnx::OnnxReader::NumOutputs(void)const __ptr64
    [DllImport(Dll, EntryPoint = "?NumOutputs@OnnxReader@paddle2onnx@@QEBAHXZ")]
    public static extern int OnnxReaderNumOutputs(ref COnnxReader pthis);



    // bool __cdecl paddle2onnx::Export(char const * __ptr64,char const * __ptr64,char * __ptr64 * __ptr64,int * __ptr64,int,bool,bool,bool,bool,bool,struct paddle2onnx::CustomOp * __ptr64,int,char const * __ptr64)
    [DllImport(Dll, EntryPoint = "?Export@paddle2onnx@@YA_NPEBD0PEAPEADPEAHH_N3333PEAUCustomOp@1@H0@Z")]
    public static extern bool Export([MarshalAs(UnmanagedType.LPStr)] string model, [MarshalAs(UnmanagedType.LPStr)] string @params, ref IntPtr output,
        ref int outputSize, int opsetVersion = 11, bool autoUpgradeOpset = true,
        bool verbose = false, bool enableOnnxChecker = true,
        bool enableExperimentalOp = false, bool enableOptimize = true,
        IntPtr customeOp = default, int opCount = 0,
        [MarshalAs(UnmanagedType.LPStr)] string deployBackend = "onnxruntime");

    // bool __cdecl paddle2onnx::Export(void const * __ptr64,int,void const * __ptr64,int,char * __ptr64 * __ptr64,int * __ptr64,int,bool,bool,bool,bool,bool,struct paddle2onnx::CustomOp * __ptr64,int,char const * __ptr64)
    [DllImport(Dll, EntryPoint = "?Export@paddle2onnx@@YA_NPEBXH0HPEAPEADPEAHH_N3333PEAUCustomOp@1@HPEBD@Z")]
    public static extern bool Export(IntPtr modelBuffer, int modelSize, IntPtr paramsBuffer,
        int paramsSize, ref IntPtr output, ref int outputSize,
        int opsetVersion = 11, bool autoUpgradeOpset = true,
        bool verbose = false, bool enableOnnxChecker = true,
        bool enableExperimentalOp = false, bool enableOptimize = true,
        IntPtr customeOp = default, int opCount = 0,
        [MarshalAs(UnmanagedType.LPStr)] string deployBackend = "onnxruntime");

    // bool __cdecl paddle2onnx::IsExportable(char const * __ptr64,char const * __ptr64,int,bool,bool,bool,bool,bool,struct paddle2onnx::CustomOp * __ptr64,int,char const * __ptr64)
    [DllImport(Dll, EntryPoint = "?IsExportable@paddle2onnx@@YA_NPEBD0H_N1111PEAUCustomOp@1@H0@Z")]
    public static extern bool IsExportable([MarshalAs(UnmanagedType.LPStr)] string model, [MarshalAs(UnmanagedType.LPStr)] string @params,
        int opsetVersion = 11, bool autoUpgradeOpset = true,
        bool verbose = false, bool enableOnnxChecker = true,
        bool enableExperimentalOp = false, bool enableOptimize = true,
        IntPtr customeOp = default, int opCount = 0,
        [MarshalAs(UnmanagedType.LPStr)] string deployBackend = "onnxruntime");

    // bool __cdecl paddle2onnx::IsExportable(void const * __ptr64,int,void const * __ptr64,int,int,bool,bool,bool,bool,bool,struct paddle2onnx::CustomOp * __ptr64,int,char const * __ptr64)
    [DllImport(Dll, EntryPoint = "?IsExportable@paddle2onnx@@YA_NPEBXH0HH_N1111PEAUCustomOp@1@HPEBD@Z")]
    public static extern bool IsExportable(IntPtr modelBuffer, int modelSize, IntPtr paramsBuffer,
        int paramsSize, int opsetVersion = 11, bool autoUpgradeOpset = true,
        bool verbose = false, bool enableOnnxChecker = true,
        bool enableExperimentalOp = false, bool enableOptimize = true,
        IntPtr customeOp = default, int opCount = 0,
        [MarshalAs(UnmanagedType.LPStr)] string deployBackend = "onnxruntime");

    // bool __cdecl paddle2onnx::RemoveMultiClassNMS(char const * __ptr64,int,char * __ptr64 * __ptr64,int * __ptr64)
    [DllImport(Dll, EntryPoint = "?RemoveMultiClassNMS@paddle2onnx@@YA_NPEBDHPEAPEADPEAH@Z")]
    public static extern bool RemoveMultiClassNMS([MarshalAs(UnmanagedType.LPArray)] byte[] onnxModel, int modelSize, out IntPtr outModel, out int outModelSize);
}
