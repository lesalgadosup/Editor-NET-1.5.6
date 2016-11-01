using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using DataTables;
using WebApiExamples.Models;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;

namespace WebApiExamples.Controllers
{
    public class CheckboxController : ApiController
    {
        [HttpGet, HttpPost, Route("api/checkbox")]
        public IHttpActionResult Checkbox()
        {
            var request = HttpContext.Current.Request;
            var settings = Properties.Settings.Default;

            using (var db = new Database(settings.DbType, settings.DbConnection))
            {
                var response = new Editor(db, "users")
                    .Model<CheckboxModel>()
                    .Field(new Field("image")
                        .SetFormatter((val, data) => (string)val == "" ? 0 : 1)
                    )
                    .Process(request)
                    .Data();

                return Json(response);
            }
        }
    }
}