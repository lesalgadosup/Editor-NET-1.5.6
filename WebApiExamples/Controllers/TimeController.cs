using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using DataTables;
using WebApiExamples.Models;

namespace WebApiExamples.Controllers
{
    public class TimeController : ApiController
    {
        [Route("api/time")]
        [HttpGet]
        [HttpPost]
        public IHttpActionResult Join()
        {
            var request = HttpContext.Current.Request;
            var settings = Properties.Settings.Default;

            using (var db = new Database(settings.DbType, settings.DbConnection))
            {
                var response = new Editor(db, "users")
                    .TryCatch(false)
                    .Model<TimeModel>()
                    .Field(new Field("shift_start")
                        .Validator(Validation.DateFormat(
                            "h:mm tt"
                        ))
                        .GetFormatter(Format.DateTime("hh:mm:ss", "h:mm tt"))
                        .SetFormatter(Format.DateTime("h:mm tt", "hh:mm:ss"))
                    )
                    .Field(new Field("shift_end")
                        .Validator(Validation.DateFormat(
                            "hh:mm:ss"
                        ))
                        .GetFormatter(Format.DateTime("hh:mm:ss"))
                    // No set formatter required, as already in IS0 format
                    )
                    .Process(request)
                    .Data();

                return Json(response);
            }
        }
    }
}