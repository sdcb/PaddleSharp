using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.Details;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdcb.PaddleOCR.Models;

/// <summary>
/// Abstract class for table recognition models.
/// </summary>
public abstract class TableRecognitionModel
{
    /// <summary>
    /// Read-only list that contains the recognized text labels.
    /// </summary>
    public IReadOnlyList<string> Labels { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableRecognitionModel"/> class with labels and mergeNoSpanStructure.
    /// </summary>
    /// <param name="labels">The list of recognized text labels.</param>
    public TableRecognitionModel(IReadOnlyList<string> labels) : this(labels, mergeNoSpanStructure: true)
    {
    }

    internal TableRecognitionModel(IReadOnlyList<string> labels, bool mergeNoSpanStructure)
    {
        if (mergeNoSpanStructure)
        {
            List<string> newLabels = labels.ToList();
            newLabels.Add("<td></td>");
            for (int i = 0; i < newLabels.Count;)
            {
                if (newLabels[i] == "<td>")
                {
                    newLabels.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            Labels = newLabels;
        }
        else
        {
            Labels = labels;
        }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="PaddleConfig"/> class.
    /// </summary>
    /// <returns>Returns an instance of the <see cref="PaddleConfig"/> class.</returns>
    public abstract PaddleConfig CreateConfig();

    /// <summary>
    /// Creates a new instance of the <see cref="FileTableRecognizationModel"/> class from a specified directory path and label path.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    /// <param name="labelPath">The label path.</param>
    /// <returns>Returns an instance of the <see cref="FileTableRecognizationModel"/> class.</returns>
    public static TableRecognitionModel FromDirectory(string directoryPath, string labelPath) => new FileTableRecognizationModel(directoryPath, labelPath);

    /// <summary>
    /// Gets the recognized label based on the specified index.
    /// </summary>
    /// <param name="i">The specified index.</param>
    /// <returns>Returns the recognized label based on the specified index.</returns>
    public virtual string GetLabelByIndex(int i)
    {
        return i switch
        {
            0 => TableRecognitionModelConsts.FirstLabel,
            var x when x > 0 && x <= Labels.Count => Labels[i - 1],
            var x when x == Labels.Count + 1 => TableRecognitionModelConsts.LastLabel,
            _ => throw new ArgumentOutOfRangeException(nameof(i), i, $"Index(given i={i}) must be in range [0, {Labels.Count + 1}], labelFilePath not matching table recognition model?"),
        };
    }

    /// <summary>
    /// Configures the <see cref="PaddleConfig"/> instance and its properties.
    /// </summary>
    /// <param name="config">The instance of the <see cref="PaddleConfig"/> class to configure.</param>
    protected virtual void ConfigPostProcess(PaddleConfig config) { }
}
