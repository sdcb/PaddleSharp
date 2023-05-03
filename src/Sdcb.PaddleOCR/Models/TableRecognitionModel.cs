using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.Details;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdcb.PaddleOCR.Models
{
    public abstract class TableRecognitionModel
    {
        public IReadOnlyList<string> Labels { get; }

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

        public abstract PaddleConfig CreateConfig();

        public static TableRecognitionModel FromDirectory(string directoryPath, string labelPath) => new FileTableRecognizationModel(directoryPath, labelPath);

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

        protected virtual void ConfigPostProcess(PaddleConfig config) { }
    }

    public static class TableRecognitionModelConsts
    {
        public const string FirstLabel = "sos";
        public const string LastLabel = "eos";
    }
}
