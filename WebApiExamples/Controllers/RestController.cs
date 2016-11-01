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
    /// This example shows how the Editor libraries can be used in a REST like
    /// environment, where different actions are sent to different URLs and / or
    /// use different HTTP verbs.
    /// 
    /// Note that the Editor instance will automatically detect which request
    /// type is being made, so all of the routes can be passed through to a
    /// single controller method.
    /// </summary>
    public class RestController : ApiController
    {
        public IHttpActionResult Rest(HttpRequest request)
        {
            var settings = Properties.Settings.Default;

            using (var db = new Database(settings.DbType, settings.DbConnection))
            {
                var response = new Editor(db, "datatables_demo")
                    .Model<StaffModel>()
                    .Field(new Field("first_name").Validator(Validation.NotEmpty()))
                    .Field(new Field("last_name").Validator(Validation.NotEmpty()))
                    .Field(new Field("extn").Validator(Validation.Numeric()))
                    .Field(new Field("age").Validator(Validation.Numeric()))
                    .Field(new Field("salary").Validator(Validation.Numeric()))
                    .Field(new Field("start_date")
                        .Validator(Validation.DateFormat(
                            Format.DATE_ISO_8601,
                            new ValidationOpts { Message = "Please enter a date in the format yyyy-mm-dd" }
                            ))
                        .GetFormatter(Format.DateSqlToFormat(Format.DATE_ISO_8601))
                        .SetFormatter(Format.DateFormatToSql(Format.DATE_ISO_8601))
                    )
                    .Process(request)
                    .Data();

                return Json(response);
            }
        }

        [Route("api/rest/get")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            var request = HttpContext.Current.Request;
            return Rest(request);
        }

        [Route("api/rest/create")]
        [HttpPost]
        public IHttpActionResult Create()
        {
            var request = HttpContext.Current.Request;
            return Rest(request);
        }

        [Route("api/rest/edit/{id}")]
        [HttpPut]
        public IHttpActionResult Edit()
        {
            var request = HttpContext.Current.Request;
            return Rest(request);
        }

        [Route("api/rest/remove/{id}")]
        [HttpDelete]
        public IHttpActionResult Remove([FromBody] FormDataCollection formData)
        {
            var request = HttpContext.Current.Request;
            return Rest(request);
        }
    }
}
