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
    /// This controller is used by the "To Do" examples on the client-side to
    /// show how Editor can display fields of different types.
    /// </summary>
    public class ToDoController : ApiController
    {
        [Route("api/todo")]
        [HttpGet]
        [HttpPost]
        public IHttpActionResult ToDo()
        {
            var request = HttpContext.Current.Request;
            var settings = Properties.Settings.Default;

            using (var db = new Database(settings.DbType, settings.DbConnection))
            {
                var response = new Editor(db, "todo")
                    .Model<ToDoModel>()
                    .Field(new Field("done").Validator(Validation.Boolean()))
                    .Field(new Field("priority").Validator(Validation.Numeric()))
                    .Process(request)
                    .Data();

                return Json(response);
            }
        }
    }
}
