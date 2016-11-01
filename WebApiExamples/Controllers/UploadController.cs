using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using DataTables;
using WebApiExamples.Models;

namespace WebApiExamples.Controllers
{
    /// <summary>
    /// This is controller is used by the majority of Editor examples as it
    /// provides a nice rounded set of information for the client-side Editor
    /// Javascript library to show its capabilities.
    ///
    /// In the code here, note that the `StaffModel` is used as the model for
    /// the Editor, which automatically defines the database fields to be read.
    /// Additional instructions can be given for each field by creating a `Field`
    /// instance for it - many of the fields have validation methods applied here
    /// and the date field has a formatter to make it readable to users looking
    /// at the table!
    /// </summary>
    public class UploadController : ApiController
    {
        [HttpGet, HttpPost, Route("api/upload")]
        public IHttpActionResult Staff()
        {
            var request = HttpContext.Current.Request;
            var settings = Properties.Settings.Default;

            using (var db = new Database(settings.DbType, settings.DbConnection))
            {
                var response = new Editor(db, "users")
                    .Model<UploadModel>()
                    .Field(new Field("image")
                        .SetFormatter(Format.IfEmpty(null))
                        .Upload(new Upload(request.PhysicalApplicationPath + @"uploads\__ID____EXTN__")
                            .Db("files", "id", new Dictionary<string, object>
                            {
                                {"web_path", Upload.DbType.WebPath},
                                {"system_path", Upload.DbType.SystemPath},
                                {"filename", Upload.DbType.FileName},
                                {"filesize", Upload.DbType.FileSize}
                            })
                        )
                    )
                    .Process(request)
                    .Data();

                return Json(response);
            }
        }
    }
}
