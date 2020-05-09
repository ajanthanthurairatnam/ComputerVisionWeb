using ComputerVisionWeb.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerVisionWeb.Models
{
    public class ImageProcessModel
    {
        public Status Status { get; set; }
        public string Message { get; set; }
        public string OutputText { get; set; }

    }
}
