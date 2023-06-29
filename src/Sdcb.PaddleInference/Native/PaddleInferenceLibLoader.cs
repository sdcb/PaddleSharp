using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Sdcb.PaddleInference.Native;

#if LINQPad || NETCOREAPP3_1_OR_GREATER

/// <summary>
/// Loads PaddleInference native library and dependencies.
/// </summary>
public class PaddleInferenceLibLoader
{
    static PaddleInferenceLibLoader()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), PaddleImportResolver);
    }

    /// <summary>
    /// Initializes the PaddleInference library loader.
    /// </summary>
    public static void Init()
    {
        // stub to ensure static constructor executed at least once.
    }

    /// <summary>
    /// Resolves PaddleInference library and its dependencies.
    /// </summary>
    /// <param name="libraryName">Name of the library to resolve.</param>
    /// <param name="assembly">Assembly of the library to resolve.</param>
    /// <param name="searchPath">Search path for the library to resolve.</param>
    /// <returns>A handle to the library if found; otherwise, null.</returns>
    private static IntPtr PaddleImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName == PaddleNative.PaddleInferenceCLib)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                LoadPaddleDependencies(libraryName, assembly, searchPath, new[]
                {
                    "openblas.dll",

                    "libiomp5md.dll",
                    "mkldnn.dll",
                    "mklml.dll",

                    "onnxruntime.dll",
                    "paddle2onnx.dll",
                });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return NativeLibrary.Load(libraryName, assembly, searchPath);
            }
            else
            {
                /* linux or others */
                LoadPaddleDependencies(libraryName, assembly, searchPath, new[]
                {
                    /* openblas is static linked in linux */
                    "libgomp.so.1",

                    "libiomp5.so",
                    "libdnnl.so.2",
                    "libmklml_intel.so",

                    "libonnxruntime.so.1.11.1",
                    "libpaddle2onnx.so.1.0.0rc2",
                });
            }
        }
        return IntPtr.Zero;
    }

    /// <summary>
    /// Loads PaddleInference library and its dependencies.
    /// </summary>
    /// <param name="libraryName">Name of the library to load.</param>
    /// <param name="assembly">Assembly to load the library from.</param>
    /// <param name="searchPath">Search path for the library to load.</param>
    /// <param name="knownDependencies">List of known dependencies to load.</param>
    private static void LoadPaddleDependencies(string libraryName, Assembly assembly, DllImportSearchPath? searchPath, string[] knownDependencies)
    {
        string dependenciesLoadStatus = string.Join(Environment.NewLine, knownDependencies
            .Select(knownDll =>
            {
                bool loadOk = NativeLibrary.TryLoad(knownDll, assembly, searchPath, out IntPtr handle);
                string loadStatus = loadOk ? "OK" : "FAIL";
                return $"{knownDll}: {loadStatus}, handle={handle:x}";
            }));

        try
        {
            NativeLibrary.Load(libraryName, assembly, searchPath);
        }
        catch (DllNotFoundException ex)
        {
            throw new DllNotFoundException(
                $"Unable to load shared library '{libraryName}', dependencies load status:{Environment.NewLine}{dependenciesLoadStatus}", ex);
        }
    }
}

#endif