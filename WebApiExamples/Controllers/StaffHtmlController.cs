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
    /// This example is used only for the DOM sourced table example where the
    /// initial table is read from HTML rather than by Ajax. A `GetFormatter`
    /// is used for the `salary` field to convert the data into the currency
    /// format for the end user to view.
    /// </summary>
    public class StaffHtmlController : ApiController
    {
        [Route("api/staff-html")]
        [HttpGet]
        [HttpPost]
        public IHttpActionResult StaffHtml()
        {
            var request = HttpContext.Current.Request;
            var settings = Properties.Settings.Default;

            using (var db = new Database(settings.DbType, settings.DbConnection))
            {
                var response = new Editor(db, "datatables_demo")
                    .Model<StaffModel>()
                    .Field(new Field("first_name").Validator(Validation.NotEmpty()))
                    .Field(new Field("last_name").Validator(Validation.NotEmpty()))
                    .Field(new Field("salary")
                        .Validator(Validation.Numeric())
                        .GetFormatter((val, row) => "$" + ((Int32)val).ToString("N0"))
                    )
                    .Process(request)
                    .Data();

                return Json(response);
            }
        }
    }
}
