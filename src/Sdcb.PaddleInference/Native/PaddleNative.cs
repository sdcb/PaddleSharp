using System;
using System.Runtime.InteropServices;

namespace Sdcb.PaddleInference.Native
{
    public class PaddleNative
    {
        private unsafe struct PdStringArray
        {
#pragma warning disable CS0649
            public nint Size;
            public byte** Data;
#pragma warning restore CS0649

            public string[] ToArray()
            {
                var result = new string[Size];
                for (int i = 0; i < Size; ++i)
                {
                    result[i] = ((IntPtr)Data[i]).UTF8PtrToString()!;
                }
                return result;
            }
        }

        public unsafe ref struct PdStringArrayWrapper
        {
            public IntPtr ptr;

            public unsafe string[] ToArray()
            {
                return ((PdStringArray*)ptr)->ToArray();
            }

            public void Dispose()
            {
                PD_OneDimArrayCstrDestroy(ptr);
                ptr = IntPtr.Zero;
            }
        }

        private unsafe struct PdIntArray
        {
            public nint Size;
            public int* Data;

            public int[] ToArray()
            {
                var result = new int[Size];
                for (int i = 0; i < Size; ++i)
                {
                    result[i] = Data[i];
                }
                return result;
            }

            public unsafe void Dispose()
            {
                fixed (PdIntArray* ptr = &this)
                {
                    PD_OneDimArrayInt32Destroy((IntPtr)ptr);
                }
            }
        }

        public ref struct PdIntArrayWrapper
        {
            public IntPtr ptr;

            public unsafe int[] ToArray()
            {
                return ((PdIntArray*)ptr)->ToArray();
            }

            public void Dispose()
            {
                PD_OneDimArrayInt32Destroy(ptr);
                ptr = IntPtr.Zero;
            }
        }

        public const string PaddleInferenceCLib =
#if NET461_OR_GREATER
			@"dll\x64\paddle_inference_c.dll";
#elif NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
            @"paddle_inference_c";
#endif


        /// <summary>Create a paddle config</summary>
        /// <returns>new config.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_ConfigCreate();

        /// <summary>Destroy the paddle config</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigDestroy(IntPtr pd_config);

        /// <summary>Set the combined model with two specific pathes for program and parameters.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="prog_file_path">  <see cref="byte*" />model file path of the combined model.</param>
        /// <param name="params_file_path">  <see cref="byte*" />params file path of the combined model.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigSetModel(IntPtr pd_config, IntPtr prog_file_path, IntPtr params_file_path);

        /// <summary>Set the model file path of a combined model.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="prog_file_path">  <see cref="byte*" />model file path.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigSetProgFile(IntPtr pd_config, IntPtr prog_file_path);

        /// <summary>Set the params file path of a combined model.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="params_file_path">  <see cref="byte*" />params file path.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigSetParamsFile(IntPtr pd_config, IntPtr params_file_path);

        /// <summary>Set the path of optimization cache directory.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="opt_cache_dir">  <see cref="byte*" />the path of optimization cache directory.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigSetOptimCacheDir(IntPtr pd_config, IntPtr opt_cache_dir);

        /// <summary>Set the no-combined model dir path.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="model_dir">  <see cref="byte*" />model dir path.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigSetModelDir(IntPtr pd_config, IntPtr model_dir);

        /// <summary>Get the model directory path.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>The model directory path.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_ConfigGetModelDir(IntPtr pd_config);

        /// <summary>Get the program file path.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>The program file path.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_ConfigGetProgFile(IntPtr pd_config);

        /// <summary>Get the params file path.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>The params file path.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_ConfigGetParamsFile(IntPtr pd_config);

        /// <summary>Turn off FC Padding.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigDisableFCPadding(IntPtr pd_config);

        /// <summary>A boolean state telling whether fc padding is used.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>Whether fc padding is used.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_ConfigUseFcPadding(IntPtr pd_config);

        /// <summary>Turn on GPU.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="memory_pool_init_size_mb">  <see cref="ulong" />initial size of the GPU memory pool in MB.</param>
        /// <param name="device_id">  <see cref="int" />device_id the GPU card to use.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigEnableUseGpu(IntPtr pd_config, ulong memory_pool_init_size_mb, int device_id);

        /// <summary>Turn off GPU.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigDisableGpu(IntPtr pd_config);

        /// <summary>A boolean state telling whether the GPU is turned on.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>Whether the GPU is turned on.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_ConfigUseGpu(IntPtr pd_config);

        /// <summary>Turn on XPU.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="l3_workspace_size">  <see cref="int" />The size of the video memory allocated by the l3         cache, the maximum is 16M.</param>
        /// <param name="locked">  <see cref="sbyte" />Whether the allocated L3 cache can be locked. If false,       it means that the L3 cache is not locked, and the allocated L3       cache can be shared by multiple models, and multiple models       sharing the L3 cache will be executed sequentially on the card.</param>
        /// <param name="autotune">  <see cref="sbyte" />Whether to autotune the conv operator in the model. If       true, when the conv operator of a certain dimension is executed       for the first time, it will automatically search for a better       algorithm to improve the performance of subsequent conv operators       of the same dimension.</param>
        /// <param name="autotune_file">  <see cref="byte*" />Specify the path of the autotune file. If       autotune_file is specified, the algorithm specified in the       file will be used and autotune will not be performed again.</param>
        /// <param name="precision">  <see cref="byte*" />Calculation accuracy of multi_encoder</param>
        /// <param name="adaptive_seqlen">  <see cref="sbyte" />Is the input of multi_encoder variable length</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigEnableXpu(IntPtr pd_config, int l3_workspace_size, sbyte locked, sbyte autotune, IntPtr autotune_file, IntPtr precision, sbyte adaptive_seqlen);

        /// <summary>Turn on NPU.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="device_id">  <see cref="int" />device_id the NPU card to use.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigEnableNpu(IntPtr pd_config, int device_id);

        /// <summary>A boolean state telling whether the XPU is turned on.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>Whether the XPU is turned on.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_ConfigUseXpu(IntPtr pd_config);

        /// <summary>A boolean state telling whether the NPU is turned on.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>Whether the NPU is turned on.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_ConfigUseNpu(IntPtr pd_config);

        /// <summary>Get the GPU device id.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>The GPU device id.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern int PD_ConfigGpuDeviceId(IntPtr pd_config);

        /// <summary>Get the XPU device id.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>The XPU device id.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern int PD_ConfigXpuDeviceId(IntPtr pd_config);

        /// <summary>Get the NPU device id.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>The NPU device id.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern int PD_ConfigNpuDeviceId(IntPtr pd_config);

        /// <summary>Get the initial size in MB of the GPU memory pool.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>The initial size in MB of the GPU memory pool.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern int PD_ConfigMemoryPoolInitSizeMb(IntPtr pd_config);

        /// <summary>Get the proportion of the initial memory pool size compared to the device.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>The proportion of the initial memory pool size.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern float PD_ConfigFractionOfGpuMemoryForPool(IntPtr pd_config);

        /// <summary>Turn on CUDNN.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigEnableCudnn(IntPtr pd_config);

        /// <summary>A boolean state telling whether to use CUDNN.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>Whether to use CUDNN.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_ConfigCudnnEnabled(IntPtr pd_config);

        /// <summary>Control whether to perform IR graph optimization. If turned off, the AnalysisConfig will act just like a NativeConfig.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="x">  <see cref="sbyte" />Whether the ir graph optimization is actived.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigSwitchIrOptim(IntPtr pd_config, sbyte x);

        /// <summary>A boolean state telling whether the ir graph optimization is actived.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>Whether to use ir graph optimization.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_ConfigIrOptim(IntPtr pd_config);

        /// <summary>Turn on the TensorRT engine. The TensorRT engine will accelerate some subgraphes in the original Fluid computation graph. In some models such as resnet50, GoogleNet and so on, it gains significant performance acceleration.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="workspace_size">  <see cref="int" />The memory size(in byte) used for TensorRT workspace.</param>
        /// <param name="max_batch_size">  <see cref="int" />The maximum batch size of this prediction task, better set as small as possible for less performance loss.</param>
        /// <param name="min_subgraph_size">  <see cref="int" /></param>
        /// <param name="precision">  <see cref="int" />The precision used in TensorRT.</param>
        /// <param name="use_static">  <see cref="sbyte" />Serialize optimization information to disk for reusing.</param>
        /// <param name="use_calib_mode">  <see cref="sbyte" />Use TRT int8 calibration(post training quantization).</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigEnableTensorRtEngine(IntPtr pd_config, int workspace_size, int max_batch_size, int min_subgraph_size, int precision, sbyte use_static, sbyte use_calib_mode);

        /// <summary>A boolean state telling whether the TensorRT engine is used.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>Whether the TensorRT engine is used.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_ConfigTensorRtEngineEnabled(IntPtr pd_config);

        /// <summary>Set min, max, opt shape for TensorRT Dynamic shape mode.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="tensor_num">The number of the subgraph input.</param>
        /// <param name="tensor_name">  <see cref="IntPtr*" />The name of every subgraph input.</param>
        /// <param name="shapes_num">  <see cref="int*" />The shape size of every subgraph input.</param>
        /// <param name="min_shape">  <see cref="IntPtr*" />The min input shape of every subgraph input.</param>
        /// <param name="max_shape">  <see cref="IntPtr*" />The max input shape of every subgraph input.</param>
        /// <param name="optim_shape">  <see cref="IntPtr*" />The opt input shape of every subgraph input.</param>
        /// <param name="disable_trt_plugin_fp16">  <see cref="sbyte" />Setting this parameter to true means that TRT plugin will not run fp16.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigSetTrtDynamicShapeInfo(IntPtr pd_config, int tensor_num, IntPtr tensor_name, IntPtr shapes_num, IntPtr min_shape, IntPtr max_shape, IntPtr optim_shape, sbyte disable_trt_plugin_fp16);

        /// <summary>A boolean state telling whether the trt dynamic_shape is used.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_ConfigTensorRtDynamicShapeEnabled(IntPtr pd_config);

        /// <summary>Enable tuned tensorrt dynamic shape.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="shape_range_info_path">  <see cref="byte*" />the path to shape_info file got in CollectShapeInfo mode.</param>
        /// <param name="allow_build_at_runtime">  <see cref="sbyte" />allow build trt engine at runtime.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigEnableTunedTensorRtDynamicShape(IntPtr pd_config, IntPtr shape_range_info_path, sbyte allow_build_at_runtime);

        /// <summary>A boolean state telling whether to use tuned tensorrt dynamic shape.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_ConfigTunedTensorRtDynamicShape(IntPtr pd_config);

        /// <summary>A boolean state telling whether to allow building trt engine at runtime.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_ConfigTrtAllowBuildAtRuntime(IntPtr pd_config);

        /// <summary>Collect shape info of all tensors in compute graph.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="shape_range_info_path">  <see cref="byte*" />the path to save shape info.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigCollectShapeRangeInfo(IntPtr pd_config, IntPtr shape_range_info_path);

        /// <summary>the shape info path in CollectShapeInfo mode. Attention, Please release the string manually.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_ConfigShapeRangeInfoPath(IntPtr pd_config);

        /// <summary>A boolean state telling whether to collect shape info.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_ConfigShapeRangeInfoCollected(IntPtr pd_config);

        /// <summary>Prevent ops running in Paddle-TRT NOTE: just experimental, not an official stable API, easy to be broken.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="ops_num">ops number</param>
        /// <param name="ops_name">  <see cref="IntPtr*" />ops name</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigDisableTensorRtOPs(IntPtr pd_config, int ops_num, IntPtr ops_name);

        /// <summary>Replace some TensorRT plugins to TensorRT OSS( https://github.com/NVIDIA/TensorRT), with which some models's inference may be more high-performance. Libnvinfer_plugin.so greater than V7.2.1 is needed.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigEnableTensorRtOSS(IntPtr pd_config);

        /// <summary>A boolean state telling whether to use the TensorRT OSS.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>Whether to use the TensorRT OSS.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_ConfigTensorRtOssEnabled(IntPtr pd_config);

        /// <summary>Enable TensorRT DLA</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="dla_core">  <see cref="int" />ID of DLACore, which should be 0, 1,        ..., IBuilder.getNbDLACores() - 1</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigEnableTensorRtDla(IntPtr pd_config, int dla_core);

        /// <summary>A boolean state telling whether to use the TensorRT DLA.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>Whether to use the TensorRT DLA.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_ConfigTensorRtDlaEnabled(IntPtr pd_config);

        /// <summary>Turn on the usage of Lite sub-graph engine.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="precision">  <see cref="int" />Precion used in Lite sub-graph engine.</param>
        /// <param name="zero_copy">  <see cref="sbyte" />whether use zero copy.</param>
        /// <param name="passes_filter_num">The number of passes used in Lite sub-graph engine.</param>
        /// <param name="passes_filter">  <see cref="IntPtr*" />The name of passes used in Lite sub-graph engine.</param>
        /// <param name="ops_filter_num">The number of operators not supported by Lite.</param>
        /// <param name="ops_filter">  <see cref="IntPtr*" />The name of operators not supported by Lite.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigEnableLiteEngine(IntPtr pd_config, int precision, sbyte zero_copy, int passes_filter_num, IntPtr passes_filter, int ops_filter_num, IntPtr ops_filter);

        /// <summary>A boolean state indicating whether the Lite sub-graph engine is used.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>Whether the Lite sub-graph engine is used.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_ConfigLiteEngineEnabled(IntPtr pd_config);

        /// <summary>Control whether to debug IR graph analysis phase. This will generate DOT files for visualizing the computation graph after each analysis pass applied.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="x">  <see cref="sbyte" />whether to debug IR graph analysis phase.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigSwitchIrDebug(IntPtr pd_config, sbyte x);

        /// <summary>Turn on MKLDNN.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigEnableMKLDNN(IntPtr pd_config);

        /// <summary>Set the cache capacity of different input shapes for MKLDNN. Default value 0 means not caching any shape. Please see MKL-DNN Data Caching Design Document: https://github.com/PaddlePaddle/FluidDoc/blob/develop/doc/fluid/design/mkldnn/caching/caching.md</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="capacity">  <see cref="int" />The cache capacity.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigSetMkldnnCacheCapacity(IntPtr pd_config, int capacity);

        /// <summary>A boolean state telling whether to use the MKLDNN.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>Whether to use the MKLDNN.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_ConfigMkldnnEnabled(IntPtr pd_config);

        /// <summary>Set the number of cpu math library threads.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="cpu_math_library_num_threads">  <see cref="int" />The number of cpu math library threads.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigSetCpuMathLibraryNumThreads(IntPtr pd_config, int cpu_math_library_num_threads);

        /// <summary>An int state telling how many threads are used in the CPU math library.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>The number of threads used in the CPU math library.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern int PD_ConfigGetCpuMathLibraryNumThreads(IntPtr pd_config);

        /// <summary>Specify the operator type list to use MKLDNN acceleration.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="ops_num">The number of operator type list.</param>
        /// <param name="op_list">  <see cref="IntPtr*" />The name of operator type list.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigSetMkldnnOp(IntPtr pd_config, int ops_num, IntPtr op_list);

        /// <summary>Turn on MKLDNN quantization.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigEnableMkldnnQuantizer(IntPtr pd_config);

        /// <summary>A boolean state telling whether the MKLDNN quantization is enabled.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>Whether the MKLDNN quantization is enabled.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_ConfigMkldnnQuantizerEnabled(IntPtr pd_config);

        /// <summary>Turn on MKLDNN bfloat16.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigEnableMkldnnBfloat16(IntPtr pd_config);

        /// <summary>A boolean state telling whether to use the MKLDNN Bfloat16.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>Whether to use the MKLDNN Bfloat16.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_ConfigMkldnnBfloat16Enabled(IntPtr pd_config);

        /// <summary>Specify the operator type list to use Bfloat16 acceleration.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="ops_num">The number of operator type list.</param>
        /// <param name="op_list">  <see cref="IntPtr*" />The name of operator type list.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigSetBfloat16Op(IntPtr pd_config, int ops_num, IntPtr op_list);

        /// <summary>Enable the GPU multi-computing stream feature. NOTE: The current behavior of this interface is to bind the computation stream to the thread, and this behavior may be changed in the future.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigEnableGpuMultiStream(IntPtr pd_config);

        /// <summary>A boolean state telling whether the thread local CUDA stream is enabled.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>Whether the thread local CUDA stream is enabled.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_ConfigThreadLocalStreamEnabled(IntPtr pd_config);

        /// <summary>Specify the memory buffer of program and parameter. Used when model and params are loaded directly from memory.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="prog_buffer">  <see cref="byte*" />The memory buffer of program.</param>
        /// <param name="prog_buffer_size">The size of the model data.</param>
        /// <param name="params_buffer">  <see cref="byte*" />The memory buffer of the combined parameters file.</param>
        /// <param name="params_buffer_size">The size of the combined parameters data.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigSetModelBuffer(IntPtr pd_config, IntPtr prog_buffer, int prog_buffer_size, IntPtr params_buffer, int params_buffer_size);

        /// <summary>A boolean state telling whether the model is set from the CPU memory.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>Whether model and params are loaded directly from memory.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_ConfigModelFromMemory(IntPtr pd_config);

        /// <summary>Turn on memory optimize NOTE still in development.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="x">  <see cref="sbyte" /></param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigEnableMemoryOptim(IntPtr pd_config, sbyte x);

        /// <summary>A boolean state telling whether the memory optimization is activated.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>Whether the memory optimization is activated.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_ConfigMemoryOptimEnabled(IntPtr pd_config);

        /// <summary>Turn on profiling report. If not turned on, no profiling report will be generated.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigEnableProfile(IntPtr pd_config);

        /// <summary>A boolean state telling whether the profiler is activated.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>bool Whether the profiler is activated.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_ConfigProfileEnabled(IntPtr pd_config);

        /// <summary>Mute all logs in Paddle inference.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigDisableGlogInfo(IntPtr pd_config);

        /// <summary>A boolean state telling whether logs in Paddle inference are muted.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>Whether logs in Paddle inference are muted.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_ConfigGlogInfoDisabled(IntPtr pd_config);

        /// <summary>Set the Config to be invalid. This is to ensure that an Config can only be used in one Predictor.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigSetInvalid(IntPtr pd_config);

        /// <summary>A boolean state telling whether the Config is valid.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>Whether the Config is valid.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_ConfigIsValid(IntPtr pd_config);

        /// <summary>Partially release the memory</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigPartiallyRelease(IntPtr pd_config);

        /// <summary>Delete all passes that has a certain type 'pass'.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="pass">  <see cref="byte*" />the certain pass type to be deleted.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigDeletePass(IntPtr pd_config, IntPtr pass);

        /// <summary>Insert a pass to a specific position</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="idx">the position to insert.</param>
        /// <param name="pass">  <see cref="byte*" />the new pass.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigInsertPass(IntPtr pd_config, int idx, IntPtr pass);

        /// <summary>Append a pass to the end of the passes</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <param name="pass">  <see cref="byte*" />the new pass.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_ConfigAppendPass(IntPtr pd_config, IntPtr pass);

        /// <summary>Get information of passes.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>Return list of the passes.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_ConfigAllPasses(IntPtr pd_config);

        /// <summary>Get information of config. Attention, Please release the string manually.</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>Return config info.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_ConfigSummary(IntPtr pd_config);

        /// <summary>Create a new Predictor</summary>
        /// <param name="pd_config">  <see cref="PD_Config*" /></param>
        /// <returns>new predicor.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_PredictorCreate(IntPtr pd_config);

        /// <summary>Clone a new Predictor</summary>
        /// <param name="pd_predictor">  <see cref="PD_Predictor*" />predictor</param>
        /// <returns>new predictor.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_PredictorClone(IntPtr pd_predictor);

        /// <summary>Get the input names</summary>
        /// <param name="pd_predictor">  <see cref="PD_Predictor*" />predictor</param>
        /// <returns>input names</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_PredictorGetInputNames(IntPtr pd_predictor);

        /// <summary>Get the output names</summary>
        /// <param name="pd_predictor">  <see cref="PD_Predictor*" />predictor</param>
        /// <returns>output names</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_PredictorGetOutputNames(IntPtr pd_predictor);

        /// <summary>Get the input number</summary>
        /// <param name="pd_predictor">  <see cref="PD_Predictor*" />predictor</param>
        /// <returns>input number</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern int PD_PredictorGetInputNum(IntPtr pd_predictor);

        /// <summary>Get the output number</summary>
        /// <param name="pd_predictor">  <see cref="PD_Predictor*" />predictor</param>
        /// <returns>output number</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern int PD_PredictorGetOutputNum(IntPtr pd_predictor);

        /// <summary>Get the Input Tensor object</summary>
        /// <param name="pd_predictor">  <see cref="PD_Predictor*" />predictor</param>
        /// <param name="name">  <see cref="byte*" />input name</param>
        /// <returns>input tensor</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_PredictorGetInputHandle(IntPtr pd_predictor, IntPtr name);

        /// <summary>Get the Output Tensor object</summary>
        /// <param name="pd_predictor">  <see cref="PD_Predictor*" />predictor</param>
        /// <param name="name">  <see cref="byte*" />output name</param>
        /// <returns>output tensor</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_PredictorGetOutputHandle(IntPtr pd_predictor, IntPtr name);

        /// <summary>Run the prediction engine</summary>
        /// <param name="pd_predictor">  <see cref="PD_Predictor*" />predictor</param>
        /// <returns>Whether the function executed successfully</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern sbyte PD_PredictorRun(IntPtr pd_predictor);

        /// <summary>Clear the intermediate tensors of the predictor</summary>
        /// <param name="pd_predictor">  <see cref="PD_Predictor*" />predictor</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_PredictorClearIntermediateTensor(IntPtr pd_predictor);

        /// <summary>Release all tmp tensor to compress the size of the memory pool. The memory pool is considered to be composed of a list of chunks, if the chunk is not occupied, it can be released.</summary>
        /// <param name="pd_predictor">  <see cref="PD_Predictor*" />predictor</param>
        /// <returns>Number of bytes released. It may be smaller than the actual released memory, because part of the memory is not managed by the MemoryPool.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern ulong PD_PredictorTryShrinkMemory(IntPtr pd_predictor);

        /// <summary>Destroy a predictor object</summary>
        /// <param name="pd_predictor">  <see cref="PD_Predictor*" />predictor</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_PredictorDestroy(IntPtr pd_predictor);

        /// <summary>Get version info.</summary>
        /// <returns>version</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_GetVersion();

        /// <summary>Destroy the paddle tensor</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_TensorDestroy(IntPtr pd_tensor);

        /// <summary>Reset the shape of the tensor. Generally it's only used for the input tensor. Reshape must be called before calling PD_TensorMutableData*() or PD_TensorCopyFromCpu*()</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <param name="shape_size">The size of shape.</param>
        /// <param name="shape">  <see cref="int*" />The shape to set.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_TensorReshape(IntPtr pd_tensor, int shape_size, IntPtr shape);

        /// <summary>Get the memory pointer in CPU or GPU with 'float' data type. Please Reshape the tensor first before call this. It's usually used to get input data pointer.</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <param name="place">  <see cref="int" />The place of the tensor.</param>
        /// <returns>Memory pointer of pd_tensor</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_TensorMutableDataFloat(IntPtr pd_tensor, int place);

        /// <summary>Get the memory pointer in CPU or GPU with 'int64_t' data type. Please Reshape the tensor first before call this. It's usually used to get input data pointer.</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <param name="place">  <see cref="int" />The place of the tensor.</param>
        /// <returns>Memory pointer of pd_tensor</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_TensorMutableDataInt64(IntPtr pd_tensor, int place);

        /// <summary>Get the memory pointer in CPU or GPU with 'int32_t' data type. Please Reshape the tensor first before call this. It's usually used to get input data pointer.</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <param name="place">  <see cref="int" />The place of the tensor.</param>
        /// <returns>Memory pointer of pd_tensor</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_TensorMutableDataInt32(IntPtr pd_tensor, int place);

        /// <summary>Get the memory pointer in CPU or GPU with 'uint8_t' data type. Please Reshape the tensor first before call this. It's usually used to get input data pointer.</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <param name="place">  <see cref="int" />The place of the tensor.</param>
        /// <returns>Memory pointer of pd_tensor</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_TensorMutableDataUint8(IntPtr pd_tensor, int place);

        /// <summary>Get the memory pointer in CPU or GPU with 'int8_t' data type. Please Reshape the tensor first before call this. It's usually used to get input data pointer.</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <param name="place">  <see cref="int" />The place of the tensor.</param>
        /// <returns>Memory pointer of pd_tensor</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_TensorMutableDataInt8(IntPtr pd_tensor, int place);

        /// <summary>Get the memory pointer directly. It's usually used to get the output data pointer.</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <param name="place">  <see cref="int*" />To get the device type of the tensor.</param>
        /// <param name="size">  <see cref="int*" />To get the data size of the tensor.</param>
        /// <returns>The tensor data buffer pointer.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_TensorDataFloat(IntPtr pd_tensor, IntPtr place, IntPtr size);

        /// <summary>Get the memory pointer directly. It's usually used to get the output data pointer.</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <param name="place">  <see cref="int*" />To get the device type of the tensor.</param>
        /// <param name="size">  <see cref="int*" />To get the data size of the tensor.</param>
        /// <returns>The tensor data buffer pointer.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_TensorDataInt64(IntPtr pd_tensor, IntPtr place, IntPtr size);

        /// <summary>Get the memory pointer directly. It's usually used to get the output data pointer.</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <param name="place">  <see cref="int*" />To get the device type of the tensor.</param>
        /// <param name="size">  <see cref="int*" />To get the data size of the tensor.</param>
        /// <returns>The tensor data buffer pointer.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_TensorDataInt32(IntPtr pd_tensor, IntPtr place, IntPtr size);

        /// <summary>Get the memory pointer directly. It's usually used to get the output data pointer.</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <param name="place">  <see cref="int*" />To get the device type of the tensor.</param>
        /// <param name="size">  <see cref="int*" />To get the data size of the tensor.</param>
        /// <returns>The tensor data buffer pointer.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_TensorDataUint8(IntPtr pd_tensor, IntPtr place, IntPtr size);

        /// <summary>Get the memory pointer directly. It's usually used to get the output data pointer.</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <param name="place">  <see cref="int*" />To get the device type of the tensor.</param>
        /// <param name="size">  <see cref="int*" />To get the data size of the tensor.</param>
        /// <returns>The tensor data buffer pointer.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_TensorDataInt8(IntPtr pd_tensor, IntPtr place, IntPtr size);

        /// <summary>Copy the host memory to tensor data. It's usually used to set the input tensor data.</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <param name="data">  <see cref="float*" />The pointer of the data, from which the tensor will copy.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_TensorCopyFromCpuFloat(IntPtr pd_tensor, IntPtr data);

        /// <summary>Copy the host memory to tensor data. It's usually used to set the input tensor data.</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <param name="data">  <see cref="long*" />The pointer of the data, from which the tensor will copy.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_TensorCopyFromCpuInt64(IntPtr pd_tensor, IntPtr data);

        /// <summary>Copy the host memory to tensor data. It's usually used to set the input tensor data.</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <param name="data">  <see cref="int*" />The pointer of the data, from which the tensor will copy.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_TensorCopyFromCpuInt32(IntPtr pd_tensor, IntPtr data);

        /// <summary>Copy the host memory to tensor data. It's usually used to set the input tensor data.</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <param name="data">  <see cref="byte*" />The pointer of the data, from which the tensor will copy.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_TensorCopyFromCpuUint8(IntPtr pd_tensor, IntPtr data);

        /// <summary>Copy the host memory to tensor data. It's usually used to set the input tensor data.</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <param name="data">  <see cref="sbyte*" />The pointer of the data, from which the tensor will copy.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_TensorCopyFromCpuInt8(IntPtr pd_tensor, IntPtr data);

        /// <summary>Copy the tensor data to the host memory. It's usually used to get the output tensor data.</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <param name="data">  <see cref="float*" />The tensor will copy the data to the address.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_TensorCopyToCpuFloat(IntPtr pd_tensor, IntPtr data);

        /// <summary>Copy the tensor data to the host memory. It's usually used to get the output tensor data.</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <param name="data">  <see cref="long*" />The tensor will copy the data to the address.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_TensorCopyToCpuInt64(IntPtr pd_tensor, IntPtr data);

        /// <summary>Copy the tensor data to the host memory. It's usually used to get the output tensor data.</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <param name="data">  <see cref="int*" />The tensor will copy the data to the address.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_TensorCopyToCpuInt32(IntPtr pd_tensor, IntPtr data);

        /// <summary>Copy the tensor data to the host memory. It's usually used to get the output tensor data.</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <param name="data">  <see cref="byte*" />The tensor will copy the data to the address.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_TensorCopyToCpuUint8(IntPtr pd_tensor, IntPtr data);

        /// <summary>Copy the tensor data to the host memory. It's usually used to get the output tensor data.</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <param name="data">  <see cref="sbyte*" />The tensor will copy the data to the address.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_TensorCopyToCpuInt8(IntPtr pd_tensor, IntPtr data);

        /// <summary>Get the tensor shape</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <returns>The tensor shape.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_TensorGetShape(IntPtr pd_tensor);

        /// <summary>Set the tensor lod information</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <param name="lod">  <see cref="PD_TwoDimArraySize*" />lod information.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_TensorSetLod(IntPtr pd_tensor, IntPtr lod);

        /// <summary>Get the tensor lod information</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <returns>the lod information.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_TensorGetLod(IntPtr pd_tensor);

        /// <summary>Get the tensor name</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <returns>the tensor name.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern IntPtr PD_TensorGetName(IntPtr pd_tensor);

        /// <summary>Get the tensor data type</summary>
        /// <param name="pd_tensor">  <see cref="PD_Tensor*" />tensor.</param>
        /// <returns>the tensor data type.</returns>
        [DllImport(PaddleInferenceCLib)]
        public static extern int PD_TensorGetDataType(IntPtr pd_tensor);

        /// <summary>Destroy the PD_OneDimArrayInt32 object pointed to by the pointer.</summary>
        /// <param name="array">  <see cref="PD_OneDimArrayInt32*" />pointer to the PD_OneDimArrayInt32 object.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_OneDimArrayInt32Destroy(IntPtr array);

        /// <summary>Destroy the PD_OneDimArrayCstr object pointed to by the pointer.</summary>
        /// <param name="array">  <see cref="PD_OneDimArrayCstr*" />pointer to the PD_OneDimArrayCstr object.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_OneDimArrayCstrDestroy(IntPtr array);

        /// <summary>Destroy the PD_OneDimArraySize object pointed to by the pointer.</summary>
        /// <param name="array">  <see cref="PD_OneDimArraySize*" />pointer to the PD_OneDimArraySize object.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_OneDimArraySizeDestroy(IntPtr array);

        /// <summary>Destroy the PD_TwoDimArraySize object pointed to by the pointer.</summary>
        /// <param name="array">  <see cref="PD_TwoDimArraySize*" />pointer to the PD_TwoDimArraySize object.</param>
        [DllImport(PaddleInferenceCLib)]
        public static extern void PD_TwoDimArraySizeDestroy(IntPtr array);
    }
}
