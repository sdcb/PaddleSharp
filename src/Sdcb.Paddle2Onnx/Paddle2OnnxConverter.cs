using Sdcb.Paddle2Onnx.Natives;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Sdcb.Paddle2Onnx;

public static class Paddle2OnnxConverter
{
    /// <summary>
    /// Determines whether a PaddlePaddle model can be converted to ONNX format from file paths.
    /// </summary>
    /// <param name="modelFile">The path to the PaddlePaddle model file.</param>
    /// <param name="paramsFile">The path to the PaddlePaddle parameter file.</param>
    /// <param name="opsetVersion">The version of ONNX Operator Set to use. Default is 11.</param>
    /// <param name="autoUpgradeOpset">If true, automatically upgrades the Operator Set if the exported model does not support the required version. Default is true.</param>
    /// <param name="verbose">If true, additional logging is printed. Default is false.</param>
    /// <param name="enableOnnxChecker">If true, validates the exported ONNX model. Default is true.</param>
    /// <param name="enableExperimentalOp">If true, experimental ONNX Operators are enabled. Default is false.</param>
    /// <param name="enableOptimize">If true, optimizes the exported ONNX graph. Default is true.</param>
    /// <param name="customOps">An array of CustomOp objects to use. Default is null.</param>
    /// <param name="deployBackend">The backend to deploy the ONNX model. Default is "onnxruntime".</param>
    /// <returns>true if the model can be exported to an ONNX file, otherwise false.</returns>
    public static unsafe bool CanConvert(
        string modelFile, 
        string paramsFile,
        int opsetVersion = 11, 
        bool autoUpgradeOpset = true, 
        bool verbose = false, 
        bool enableOnnxChecker = true, 
        bool enableExperimentalOp = false, 
        bool enableOptimize = true,
        CustomOp[]? customOps = null, 
        string deployBackend = "onnxruntime")
    {
        if (!File.Exists(modelFile)) throw new FileNotFoundException("Model file not found.", modelFile);
        if (!File.Exists(paramsFile)) throw new FileNotFoundException("Params file not found.", paramsFile);

        CCustomOp[]? customOpsNative = new CCustomOp[customOps?.Length ?? 0];
        GCHandle handle = CustomOpsToHandle(customOps, customOpsNative);

        try
        {
            return Paddle2OnnxLib.IsExportable(
                modelFile, paramsFile,
                opsetVersion,
                autoUpgradeOpset,
                verbose,
                enableOnnxChecker,
                enableExperimentalOp,
                enableOptimize,
                handle.AddrOfPinnedObject(),
                customOpsNative.Length,
                deployBackend);
        }
        finally
        {
            handle.Free();
        }
    }

    /// <summary>
    /// Determines whether a PaddlePaddle model can be converted to ONNX format from in-memory buffers.
    /// </summary>
    /// <param name="modelBuffer">The bytes of the PaddlePaddle model file. modelFile must not be null.</param>
    /// <param name="paramsBuffer">The bytes of the PaddlePaddle parameter file. paramsFile must not be null.</param>
    /// <param name="opsetVersion">The version of ONNX Operator Set to use. Default is 11.</param>
    /// <param name="autoUpgradeOpset">If true, automatically upgrades the Operator Set if the exported model does not support the required version. Default is true.</param>
    /// <param name="verbose">If true, additional logging is printed. Default is false.</param>
    /// <param name="enableOnnxChecker">If true, validates the exported ONNX model. Default is true.</param>
    /// <param name="enableExperimentalOp">If true, experimental ONNX Operators are enabled. Default is false.</param>
    /// <param name="enableOptimize">If true, optimizes the exported ONNX graph. Default is true.</param>
    /// <param name="customOps">An array of CustomOp objects to use. Default is null.</param>
    /// <param name="deployBackend">The backend to deploy the ONNX model. Default is "onnxruntime".</param>
    /// <returns>true if the model can be exported to an ONNX file format, otherwise false.</returns>
    public static unsafe bool CanConvert(
        byte[] modelBuffer,
        byte[] paramsBuffer,
        int opsetVersion = 11,
        bool autoUpgradeOpset = true,
        bool verbose = false,
        bool enableOnnxChecker = true,
        bool enableExperimentalOp = false,
        bool enableOptimize = true,
        CustomOp[]? customOps = null,
        string deployBackend = "onnxruntime")
    {
        if (modelBuffer == null) throw new ArgumentNullException(nameof(modelBuffer));
        if (paramsBuffer == null) throw new ArgumentNullException(nameof(paramsBuffer));

        CCustomOp[]? customOpsNative = new CCustomOp[customOps?.Length ?? 0];
        GCHandle handle = CustomOpsToHandle(customOps, customOpsNative);
        GCHandle modelHandle = GCHandle.Alloc(modelBuffer, GCHandleType.Pinned);
        GCHandle paramsHandle = GCHandle.Alloc(paramsBuffer, GCHandleType.Pinned);

        try
        {
            return Paddle2OnnxLib.IsExportable(
                modelHandle.AddrOfPinnedObject(), modelBuffer.Length,
                paramsHandle.AddrOfPinnedObject(), paramsBuffer.Length,
                opsetVersion,
                autoUpgradeOpset,
                verbose,
                enableOnnxChecker,
                enableExperimentalOp,
                enableOptimize,
                handle.AddrOfPinnedObject(),
                customOpsNative.Length,
                deployBackend);
        }
        finally
        {
            handle.Free();
            modelHandle.Free();
            paramsHandle.Free();
        }
    }

    /// <summary>
    /// Convert PaddlePaddle models to ONNX format.
    /// </summary>
    /// <param name="modelFile">Path to the PaddlePaddle model file.</param>
    /// <param name="paramsFile">Path to the parameter file associated with the PaddlePaddle model file.</param>
    /// <param name="opsetVersion">The ONNX opset version.</param>
    /// <param name="autoUpgradeOpset">Whether or not to auto-upgrade the opset to the latest supported version.</param>
    /// <param name="verbose">Whether to enable verbose logging information.</param>
    /// <param name="enableOnnxChecker">Whether or not to perform model optimization.</param>
    /// <param name="enableExperimentalOp">Whether or not to enable the use of experimental ops, which may not be available for all ONNX inference runtime implementations.</param>
    /// <param name="enableOptimize">Whether or not to perform model checks.</param>
    /// <param name="customOps">Custom operator definitions to be used for model conversion.</param>
    /// <param name="deployBackend">Backend for deployment, defaults to ONNXRuntime.</param>
    /// <returns>The ONNX model.</returns>
    public static unsafe byte[] ConvertToOnnx(
        string modelFile,
        string paramsFile,
        int opsetVersion = 11, 
        bool autoUpgradeOpset = true, 
        bool verbose = false, 
        bool enableOnnxChecker = true, 
        bool enableExperimentalOp = false, 
        bool enableOptimize = true, 
        CustomOp[]? customOps = null, 
        string deployBackend = "onnxruntime")
    {
        if (!File.Exists(modelFile)) throw new FileNotFoundException("Model file not found.", modelFile);
        if (!File.Exists(paramsFile)) throw new FileNotFoundException("Params file not found.", paramsFile);

        CCustomOp[]? customOpsNative = new CCustomOp[customOps?.Length ?? 0];
        GCHandle handle = CustomOpsToHandle(customOps, customOpsNative);

        try
        {
            bool ok = Paddle2OnnxLib.Export(
                modelFile, paramsFile,
                out IntPtr output, out int outputSize,
                opsetVersion,
                autoUpgradeOpset,
                verbose,
                enableOnnxChecker,
                enableExperimentalOp,
                enableOptimize,
                handle.AddrOfPinnedObject(),
                customOpsNative.Length,
                deployBackend);
            if (!ok || output == IntPtr.Zero) throw new Exception("Failed to export model.");

            byte[] result = new ReadOnlySpan<byte>((void*)output, outputSize).ToArray();
            MsvcLib.free(output);
            return result;
        }
        finally
        {
            handle.Free();
        }
    }

    /// <summary>
    /// Exports a PaddlePaddle model to an ONNX model
    /// </summary>
    /// <param name="modelBuffer">The buffer containing the model data. Must not be null.</param>
    /// <param name="paramsBuffer">The buffer containing the parameters. Must not be null.</param>
    /// <param name="opsetVersion">The version of the operator set to use. Default is 11.</param>
    /// <param name="autoUpgradeOpset">Whether to automatically upgrade opset. Default is true.</param>
    /// <param name="verbose">Whether to write verbose output. Default is false.</param>
    /// <param name="enableOnnxChecker">Whether to enable checking of the generated ONNX model. Default is true.</param>
    /// <param name="enableExperimentalOp">Whether to enable experimental operators. Default is false.</param>
    /// <param name="enableOptimize">Whether to enable model optimization. Default is true.</param>
    /// <param name="customOps">An optional array of custom operators. Default is null.</param>
    /// <param name="deployBackend">The backend to use. Default is "onnxruntime".</param>
    /// <returns>The ONNX model as a byte array.</returns>
    public static unsafe byte[] ConvertToOnnx(
        byte[] modelBuffer,
        byte[] paramsBuffer,
        int opsetVersion = 11,
        bool autoUpgradeOpset = true,
        bool verbose = false,
        bool enableOnnxChecker = true,
        bool enableExperimentalOp = false,
        bool enableOptimize = true,
        CustomOp[]? customOps = null,
        string deployBackend = "onnxruntime")
    {
        if (modelBuffer == null) throw new ArgumentNullException(nameof(modelBuffer));
        if (paramsBuffer == null) throw new ArgumentNullException(nameof(paramsBuffer));

        CCustomOp[]? customOpsNative = new CCustomOp[customOps?.Length ?? 0];
        GCHandle handle = CustomOpsToHandle(customOps, customOpsNative);
        GCHandle modelHandle = GCHandle.Alloc(modelBuffer, GCHandleType.Pinned);
        GCHandle paramsHandle = GCHandle.Alloc(paramsBuffer, GCHandleType.Pinned);

        try
        {
            bool ok = Paddle2OnnxLib.Export(
                modelHandle.AddrOfPinnedObject(), modelBuffer.Length, 
                paramsHandle.AddrOfPinnedObject(), paramsBuffer.Length,
                out IntPtr output, out int outputSize,
                opsetVersion,
                autoUpgradeOpset,
                verbose,
                enableOnnxChecker,
                enableExperimentalOp,
                enableOptimize,
                handle.AddrOfPinnedObject(),
                customOpsNative.Length,
                deployBackend);
            if (!ok || output == IntPtr.Zero) throw new Exception("Failed to export model.");

            byte[] result = new ReadOnlySpan<byte>((void*)output, outputSize).ToArray();
            MsvcLib.free(output);
            return result;
        }
        finally
        {
            handle.Free();
        }
    }

    public static unsafe byte[] RemoveOnnxMultiClassNMS(byte[] onnxModel)
    {
        GCHandle handle = GCHandle.Alloc(onnxModel, GCHandleType.Pinned);
        try
        {
            bool ok = Paddle2OnnxLib.RemoveMultiClassNMS(handle.AddrOfPinnedObject(), onnxModel.Length, out IntPtr outModel, out int outSize);
            if (!ok || outModel == IntPtr.Zero) throw new Exception("Failed to remove MultiClassNMS.");

            byte[] result = new ReadOnlySpan<byte>((void*)outModel, outSize).ToArray();
            MsvcLib.free(outModel);
            return result;
        }
        finally
        {
            handle.Free();
        }
    }

    private static unsafe GCHandle CustomOpsToHandle(CustomOp[]? customOps, CCustomOp[] customOpsNative)
    {
        if (customOps != null)
        {
            for (int i = 0; i < customOps.Length; ++i)
            {
                Paddle2OnnxLib.CustomOpInit(ref customOpsNative[i]);
                customOpsNative[i].Assign(customOps[i]);
            }
        }

        return GCHandle.Alloc(customOpsNative, GCHandleType.Pinned);
    }
}
