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
    public class DatesController : ApiController
    {
        [Route("api/dates")]
        [HttpGet]
        [HttpPost]
        public IHttpActionResult Dates()
        {
            var request = HttpContext.Current.Request;
            var settings = Properties.Settings.Default;

            using (var db = new Database(settings.DbType, settings.DbConnection))
            {
                // Allow a number of different formats to be submitted for the various demos
                var update = Format.DATE_ISO_8601;
                var registered = Format.DATE_ISO_8601;
                var format = request.QueryString["format"]; // null if not given

                if (format == "custom")
                {
                    update = "M/d/yyyy";
                    registered = "dddd d MMMM yyyy";
                }

                var response = new Editor(db, "users")
                    .Model<DatesModel>()
                    .Field(new Field("updated_date")
                        .Validator(Validation.DateFormat(
                            update,
                            new ValidationOpts { Message = "Please enter a date in the format yyyy-mm-dd" }
                            ))
                        .GetFormatter(Format.DateSqlToFormat(update))
                        .SetFormatter(Format.DateFormatToSql(update))
                    )
                    .Field(new Field("registered_date")
                        .Validator(Validation.DateFormat(
                            registered,
                            new ValidationOpts { Message = "Please enter a date in the format ddd, d MMM yy" }
                            ))
                        .GetFormatter(Format.DateSqlToFormat(registered))
                        .SetFormatter(Format.DateFormatToSql(registered))
                    )
                    .Process(request)
                    .Data();

                return Json(response);
            }
        }
    }
}
