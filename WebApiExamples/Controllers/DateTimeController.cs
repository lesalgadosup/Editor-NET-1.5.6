using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using DataTables;
using WebApiExamples.Models;

namespace WebApiExamples.Controllers
{
    /// <summary>
    /// This example shows how a field can use formatters and validators to
    /// work with date fields - traditionally a fairly complex task, made easy
    /// here.
    ///
    /// Note how the <code>ValidationOpts</code> class is used to specify a
    /// custom error message for the validation methods.
    /// </summary>
    public class DateTimeController : ApiController
    {
        [Route("api/datetime")]
        [HttpGet]
        [HttpPost]
        public IHttpActionResult Dates()
        {
            var request = HttpContext.Current.Request;
            var settings = Properties.Settings.Default;

            using (var db = new Database(settings.DbType, settings.DbConnection))
            {
                var response = new Editor(db, "users")
                    .Model<DatesModel>()
                    .Field(new Field("updated_date")
                        .Validator(Validation.DateFormat(
                            "MM-dd-yyyy h:mm tt",
                            new ValidationOpts { Message = "Please enter a date and time in the format MM-dd-yyyy h:mm A" }
                            ))
                        .GetFormatter(Format.DateTime("yyyy-MM-dd hh:mm", "MM-dd-yyyy h:mm tt"))
                        .SetFormatter(Format.DateTime("MM-dd-yyyy h:mm tt", "yyyy-MM-dd hh:mm"))
                    )
                    .Field(new Field("registered_date")
                        .Validator(Validation.DateFormat(
                            "d MMM yyyy HH:mm",
                            new ValidationOpts { Message = "Please enter a date and time in the format d MMMM yyyy HH:mm" }
                            ))
                        .GetFormatter(Format.DateTime("yyyy-MM-dd hh:mm", "d MMM yyyy HH:mm"))
                        .SetFormatter(Format.DateTime("d MMM yyyy HH:mm", "yyyy-MM-dd hh:mm"))
                    )
                    .Process(request)
                    .Data();

                return Json(response);
            }
        }
    }
}