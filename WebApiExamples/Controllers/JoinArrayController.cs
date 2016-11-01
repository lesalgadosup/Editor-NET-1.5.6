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
    /// This example shows a very simple join using the `LeftJoin` method.
    /// Of particular note in this example is that the `JoinModel` defines two
    /// nested classes that obtain the data required from the two tables, where
    /// the nested class name defines the database table and the properties of
    /// the nested classes define the columns in each table.
    ///
    /// Note also the use of the `Options()` method for the `users.site` method
    /// which is used to retrieve the information for the `Site` drop down list
    /// on the client-side, which is automatically populated when the data is
    /// loaded.
    /// </summary>
    public class JoinArrayController : ApiController
    {
        [Route("api/joinArray")]
        [HttpGet]
        [HttpPost]
        public IHttpActionResult JoinArray()
        {
            var request = HttpContext.Current.Request;
            var settings = Properties.Settings.Default;

            using (var db = new Database(settings.DbType, settings.DbConnection))
            {
                var response = new Editor(db, "users")
                    .Model<JoinModel>()
                    .Field(new Field("users.site")
                        .Options("sites", "id", "name")
                    )
                    .LeftJoin("sites", "sites.id", "=", "users.site")
                    .MJoin(new MJoin("access")
                        .Link("users.id", "user_access.user_id")
                        .Link("access.id", "user_access.access_id")
                        .Model<JoinAccessModel>()
                        .Order("access.name")
                        .Field(new Field("id")
                            .Options("access", "id", "name")
                        )
                    )
                    .Process(request)
                    .Data();

                return Json(response);
            }
        }
    }
}
