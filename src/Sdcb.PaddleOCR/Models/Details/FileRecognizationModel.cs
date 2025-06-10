using Sdcb.PaddleInference;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using YamlDotNet.RepresentationModel;

namespace Sdcb.PaddleOCR.Models.Details;

/// <summary>
/// Recognization model for file processing.
/// </summary>
/// <remarks>
/// Represents a model used for file processing.
/// </remarks>
public class FileRecognizationModel : RecognizationModel
{
    private readonly IReadOnlyList<string> _labels;

    /// <summary>
    /// Gets the directory path for the model.
    /// </summary>
    public string DirectoryPath { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileRecognizationModel"/> class.
    /// </summary>
    /// <param name="directoryPath">The directory path for the model.</param>
    /// <param name="labelFilePath">The path for the label file.</param>
    /// <param name="version">The version of the model.</param>
    public FileRecognizationModel(string directoryPath, string labelFilePath, ModelVersion version) : base(version)
    {
        DirectoryPath = directoryPath;
        if (version == ModelVersion.V5)
        {
            string ymlConfig = Path.Combine(directoryPath, "inference.yml");
            if (!File.Exists(ymlConfig))
                throw new FileNotFoundException("inference.yml not found in the model directory.", ymlConfig);

            using Stream stream = File.OpenRead(ymlConfig);
            YamlStream yaml = new();
            yaml.Load(new StreamReader(stream));
            _labels = ((YamlSequenceNode)yaml.Documents[0].RootNode["PostProcess"]["character_dict"])
                .Select(x => ((YamlScalarNode)x).Value!)
                .ToList();
        }
        else
        {
            _labels = File.ReadAllLines(labelFilePath);
        }
    }

    /// <summary>
    /// Overrides the function in <see cref="RecognizationModel"/> to create the configuration for the model.
    /// </summary>
    /// <returns>The configuration for the model.</returns>
    public override PaddleConfig CreateConfig()
    {
        return PaddleConfig.FromModelDir(DirectoryPath);
    }

    /// <summary>
    /// Returns the label string for the given index.
    /// </summary>
    /// <param name="i">The index of the label.</param>
    /// <returns>The label string for the given index.</returns>
    public override string GetLabelByIndex(int i) => GetLabelByIndex(i, _labels);
}
