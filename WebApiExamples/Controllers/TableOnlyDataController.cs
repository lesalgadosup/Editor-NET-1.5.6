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
    /// This example is used to show data that is shown in the table only.
    /// Specifically, the `updated_date` field updated automatically by the
    /// database, so we don't need to update ourselves, but we still want to
    /// read that information. The `Set()` method is used to make this a read
    /// only field.
    /// </summary>
    public class TableOnlyDataController : ApiController
    {
        [Route("api/tableOnlyData")]
        [HttpGet]
        [HttpPost]
        public IHttpActionResult TableOnlyData()
        {
            var request = HttpContext.Current.Request;
            var settings = Properties.Settings.Default;

            using (var db = new Database(settings.DbType, settings.DbConnection))
            {
                var response = new Editor(db, "users")
                    .Model<TableOnlyDataModel>()
                    .Field(new Field("updated_date")
                        .Set(false)
                        .GetFormatter(Format.DateSqlToFormat(Format.DATE_ISO_822))
                    )
                    .Process(request)
                    .Data();

                return Json(response);
            }
        }
    }
}
