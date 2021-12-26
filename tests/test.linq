<Query Kind="Program">
  <NuGetReference>OpenCvSharp4</NuGetReference>
  <NuGetReference>OpenCvSharp4.runtime.win</NuGetReference>
  <NuGetReference>Sdcb.PaddleInference</NuGetReference>
  <NuGetReference>Sdcb.PaddleInference.runtime.win64.mkl</NuGetReference>
  <NuGetReference>Sdcb.PaddleOCR</NuGetReference>
  <NuGetReference>Sdcb.PaddleOCR.KnownModels</NuGetReference>
  <Namespace>OpenCvSharp</Namespace>
  <Namespace>Sdcb.PaddleInference</Namespace>
  <Namespace>Sdcb.PaddleOCR</Namespace>
  <Namespace>Sdcb.PaddleOCR.KnownModels</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Windows.Forms</Namespace>
</Query>

async Task Main()
{
	await KnownOCRModel.PPOcrV2.EnsureAll(QueryCancelToken);
	using PaddleOcrAll all = new (KnownOCRModel.PPOcrV2.RootDirectory, KnownOCRModel.PPOcrV2.KeyPath);
	using Mat src = Cv2.ImRead(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "xdr5480.jpg"));
	//using Mat src = Cv2.ImDecode(GetClipboardImage(), ImreadModes.Color);
	Console.WriteLine(all.Run(src).Text);
}

byte[] GetClipboardImage()
{
	using var ms = new MemoryStream();
	Clipboard.GetImage().Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
	return ms.ToArray();
}