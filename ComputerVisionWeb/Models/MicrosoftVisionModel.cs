using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerVisionWeb.Models
{
    public class Word
    {
        public IList<int> boundingBox { get; set; }
        public string text { get; set; }
        public string confidence { get; set; }
    }

    public class Line
    {
        public IList<int> boundingBox { get; set; }
        public string text { get; set; }
        public IList<Word> words { get; set; }
    }

    public class RecognitionResult
    {
        public int page { get; set; }
        public double clockwiseOrientation { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string unit { get; set; }
        public IList<Line> lines { get; set; }
    }

    public class MicrosoftVisionModel
    {
        public string status { get; set; }
        public IList<RecognitionResult> recognitionResults { get; set; }
    }


}
