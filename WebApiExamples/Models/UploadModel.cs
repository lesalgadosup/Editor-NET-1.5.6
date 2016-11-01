using System.ComponentModel;
using DataTables;

namespace WebApiExamples.Models
{
    public class UploadModel
    {
        public string first_name { get; set; }

        public string last_name { get; set; }

        public string phone { get; set; }

        public string city { get; set; }

        public int image { get; set; }
    }
}