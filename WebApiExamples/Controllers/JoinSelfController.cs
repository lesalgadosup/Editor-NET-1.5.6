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
    /// As with SQL, Editor joins can be self referencing using the `as` option
    /// to alias a table name. This example shows the `users` database table
    /// being used to get a list of all users, but each user also has a manager
    /// who is also a user! The `LeftJoin()` is used to create this relationship.
    ///
    /// Note also the `Options()` method for the `users.manager` field which
    /// uses a delegate to get the list of options for the manager drop down
    /// list on the client-side.
    /// </summary>
    public class JoinSelfController : ApiController
    {
        [Route("api/joinSelf")]
        [HttpGet]
        [HttpPost]
        public IHttpActionResult JoinSelf()
        {
            var request = HttpContext.Current.Request;
            var settings = Properties.Settings.Default;

            using (var db = new Database(settings.DbType, settings.DbConnection))
            {
                var response = new Editor(db, "users")
                    .Model<JoinSelfModel>()
                    .Field(new Field("users.manager")
                        .Options("users", "id", new[] { "first_name", "last_name" })
                    )
                    .LeftJoin("users as manager", "users.manager", "=", "manager.id")
                    .Process(request)
                    .Data();

                return Json(response);
            }
        }
    }
}
