using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiExamples.Models
{
    public class CheckboxModel
    {
        public string first_name { get; set; }

        public string last_name { get; set; }

        public string phone { get; set; }

        public string city { get; set; }

        public string zip { get; set; }

        public int active { get; set; }
    }
}