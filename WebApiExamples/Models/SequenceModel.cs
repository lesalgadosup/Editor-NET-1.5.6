using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiExamples.Models
{
    public class SequenceModel
    {
        public string title { get; set; }

        public string author { get; set; }

        public int duration { get; set; }

        public int readingOrder { get; set; }
    }
}