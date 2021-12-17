<Query Kind="Program">
  <NuGetReference>OpenCvSharp4</NuGetReference>
  <NuGetReference>OpenCvSharp4.runtime.win</NuGetReference>
  <NuGetReference Prerelease="true">Sdcb.PaddleInference</NuGetReference>
  <NuGetReference Prerelease="true">Sdcb.PaddleInference.runtime.win64.mkl</NuGetReference>
  <NuGetReference Prerelease="true">Sdcb.PaddleOCR</NuGetReference>
  <Namespace>OpenCvSharp</Namespace>
  <Namespace>Sdcb.PaddleInference</Namespace>
  <Namespace>Sdcb.PaddleOCR</Namespace>
</Query>

void Main()
{
	using PaddleOcrAll all = new (@"C:\_\3rd\paddle\models\ppocr-v2", @"C:\_\3rd\paddle\models\keys-cn.txt");
	using Mat src = Cv2.ImRead(@"C:\Users\ZhouJie\Pictures\xdr5480.jpg");
	Console.WriteLine(all.Run(src).Text);
}