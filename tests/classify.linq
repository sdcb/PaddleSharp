<Query Kind="Program">
  <NuGetReference>OpenCvSharp4</NuGetReference>
  <NuGetReference>OpenCvSharp4.runtime.win</NuGetReference>
  <NuGetReference Prerelease="true">Sdcb.PaddleInference</NuGetReference>
  <NuGetReference>Sdcb.PaddleInference.runtime.win64.mkl</NuGetReference>
  <NuGetReference Prerelease="true">Sdcb.PaddleOCR</NuGetReference>
  <NuGetReference Prerelease="true">Sdcb.PaddleOCR.KnownModels</NuGetReference>
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
	IOCRModel model = await KnownOCRModel.PPOcrV2.EnsureAll(QueryCancelToken);
	//PaddleConfig.EnableMkldnnByDefault = false;
	//PaddleConfig.UseGpuByDefault = true;
	PaddleConfig.Defaults.UseGpu = false;
	PaddleConfig.Defaults.UseMkldnn = true;
	
	using PaddleOcrAll all = new(model.DetectionDirectory, model.ClassifierDirectory, model.RecognitionDirectory, model.KeyPath)
	{
	   Enable180Classification = true,
	   AllowRotateDetection = true,
	};
	var images = new[]
	{
		"rotate0.png", 
		"rotate180.png", 
		"rotate0b.png", 
	};
	images
		.Select(x => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), x))
		.Select(x => new
		{
			Image = Util.Image(x, Util.ScaleMode.Unscaled), 
			ShouldRotate180 = all.Classifier.ShouldRotate180(Cv2.ImRead(x))
		})
		.Dump();
}