using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using DataTables;
using WebApiExamples.Models;

namespace WebApiExamples.Controllers
{
    public class SequenceController : ApiController
    {
        [Route("api/sequence")]
        [HttpGet]
        [HttpPost]
        public IHttpActionResult Staff()
        {
            var request = HttpContext.Current.Request;
            var settings = Properties.Settings.Default;

            using (var db = new Database(settings.DbType, settings.DbConnection))
            {
                var editor = new Editor(db, "audiobooks")
                    .Model<SequenceModel>()
                    .Field(new Field("title").Validator(Validation.NotEmpty()))
                    .Field(new Field("author").Validator(Validation.NotEmpty()))
                    .Field(new Field("duration").Validator(Validation.Numeric()))
                    .Field(new Field("readingOrder").Validator(Validation.Numeric()));

                editor.PreCreate += (sender, e) => e.Editor.Db()
                    .Query("update", "audiobooks")
                    .Set("readingOrder", "readingOrder+1", false)
                    .Where("readingOrder", e.Values["readingOrder"], ">=")
                    .Exec();

                editor.PostRemove += (sender, e) => e.Editor.Db()
                    .Query("update", "audiobooks")
                    .Set("readingOrder", "readingOrder-1", false)
                    .Where("readingOrder", e.Values["readingOrder"], ">")
                    .Exec();

                var response = editor
                    .Process(request)
                    .Data();

                return Json(response);
            }
        }
    }
}