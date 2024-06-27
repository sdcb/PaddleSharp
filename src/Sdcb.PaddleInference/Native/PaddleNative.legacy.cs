using System;
using System.Runtime.InteropServices;

namespace Sdcb.PaddleInference.Native;

public partial class PaddleNative
{
    /// <summary>Turn on GPU, note: this api is only available when version &lt; 2.5.0</summary>
    /// <param name="pd_config">(C API type: PD_Config*) </param>
    /// <param name="memory_pool_init_size_mb">initial size of the GPU memory pool in MB.</param>
    /// <param name="device_id">device_id the GPU card to use.</param>
    [DllImport(PaddleInferenceCLib)]
    public static extern void PD_ConfigEnableUseGpu(IntPtr pd_config, ulong memory_pool_init_size_mb, int device_id);
}
