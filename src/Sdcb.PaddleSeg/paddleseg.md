# Sdcb.PaddleSeg Usage Example

This example demonstrates how to use Sdcb.PaddleSeg for image segmentation.

## Basic Usage

### 1. Import Required Namespaces

```csharp
using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleSeg;
```

### 2. Load Model

First, load the segmentation model from the model directory:

```csharp
SegModel segModel = SegModel.FromDirectory(@"E:\gitproject\PaddleSeg\output\infer_model");
```

### 3. Create Predictor

Create a predictor using the loaded model, here we use MKLDNN acceleration:

```csharp
PaddleSegPredictor paddleSegPredictor = new PaddleSegPredictor(segModel, PaddleDevice.Mkldnn());
```

### 4. Load Image and Perform Prediction

```csharp
// Load input image
Mat img = new Mat(@"E:\gitproject\PaddleSeg\datasets\optic_disc_seg\JPEGImages\H0002.jpg");

// Perform prediction
var result = paddleSegPredictor.Run(img);
```

### 5. Process Prediction Results

Convert the prediction results to a binary mask image:

```csharp
// Create single-channel mask image
Mat mask = new Mat(img.Size(), MatType.CV_8UC1);

// Convert segmentation result to mask
int width = img.Width;
int height = img.Height;
for (int y = 0; y < height; y++)
{
    for (int x = 0; x < width; x++)
    {
        int index = y * width + x;
        if (index < result.Length)
        {
            // Convert 1 to 255, keep 0 as 0
            mask.Set(y, x, result[index] == 1 ? (byte)255 : (byte)0);
        }
    }
}

// Save mask image
Cv2.ImWrite("mask.png", mask);
```

## Notes

1. Ensure that `Sdcb.PaddleSeg`, `Sdcb.PaddleInference`, and `OpenCvSharp4` NuGet packages are properly installed
2. Model path should point to a valid PaddleSeg exported model directory
3. Input image path should be a valid image file path

## Results

The program will generate a binary mask image named `mask.png`, where:
- White regions (255) represent the target area
- Black regions (0) represent the background
