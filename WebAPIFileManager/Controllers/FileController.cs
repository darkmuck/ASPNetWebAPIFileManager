using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace WebAPIFileManager.Controllers
{
    public class FileController : ApiController
    {
        private string DefaultHomeDirectory = WebAPIFileManager.Properties.Resources.DefaultHomeDirectory;
        private struct FileInformation
        {
            public string name { get; set; }
            [JsonProperty(PropertyName = "type")]
            public string type { get; set; }
            public long size { get; set; }
            public int? count { get; set; }
            public string path { get; set; }
        }

        [System.Web.Http.HttpGet]
        public System.Web.Mvc.JsonResult GetFilesJSON(string dir)
        {
            var lstFileInformation = new List<FileInformation>();

            // Get directory info
            dir = String.IsNullOrEmpty(dir) ? DefaultHomeDirectory : dir;
            dir = (dir[0] == '/') ? dir : "/" + dir;
            DirectoryInfo dinfo = new DirectoryInfo(HttpContext.Current.Server.MapPath("~"+@dir));

            // Get list of files and folders
            var lstFiles = dinfo.GetFiles().ToList();
            var lstFolders = dinfo.GetDirectories().ToList();

            // Add files and folders to returnList
            GetFoldersInList(lstFolders, dir, ref lstFileInformation);
            GetFilesInList(lstFiles, dir, ref lstFileInformation);

            // Return returnList as JsonResult
            return new System.Web.Mvc.JsonResult()
            {
                Data = lstFileInformation,
                JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<HttpResponseMessage> Upload(string dir)
        {
            try
            {
                var provider = new MultipartMemoryStreamProvider();
                // Read MIME message and store in a MultipartMemoryStreamProvider
                await Request.Content.ReadAsMultipartAsync(provider);
                // Process each item in the HttpContent collection
                foreach (var file in provider.Contents)
                {
                    // Serialize the data in to a stream
                    var dataStream = await file.ReadAsStreamAsync();
                    // Copy Stream to a MemoryStream
                    var ms = new MemoryStream();
                    dataStream.CopyTo(ms);
                    // Convert MemoryStream to an Array
                    byte[] bytes = ms.ToArray();
                    // Get filename
                    string name = file.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                    // Save file
                    SaveFile(name, bytes, dir);
                    // Build and return response
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent("Upload Success!", System.Text.Encoding.UTF8, "text/plain");
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(@"text/html");
                    return response;
                }
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }

            return Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, HttpStatusCode.UnsupportedMediaType.ToString());
        }

        private static void SaveFile(string name, byte[] bytes, string dir)
        {
            // Get the full path
            string path = HttpContext.Current.Server.MapPath("~" + @dir);
            if (!path.EndsWith(@"\")) path += @"\";

            // Delete the file if it already exists
            if (File.Exists(Path.Combine(path, name))) File.Delete(Path.Combine(path, name));

            // Save the file
            using (FileStream fs = new FileStream(Path.Combine(path, name), FileMode.CreateNew, FileAccess.Write))
            {
                fs.Write(bytes, 0, (int)bytes.Length);
                fs.Close();
            }
        }

        private void GetFoldersInList(List<DirectoryInfo> lstFolders, string dir, ref List<FileInformation> lstFileInformation)
        {
            foreach (DirectoryInfo d in lstFolders)
            {
                string dirName = d.Name;
                long size = d.GetFiles("*.*", SearchOption.AllDirectories).Sum(file => file.Length);
                int count = d.GetFiles().Count();
                lstFileInformation.Add(new FileInformation()
                {
                    name = dirName,
                    type = null,
                    size = size,
                    count = count,
                    path = dir + "/"
                });
            }
        }

        private void GetFilesInList(List<FileInfo> lstFiles, string dir, ref List<FileInformation> lstFileInformation)
        {
            foreach (FileInfo file in lstFiles)
            {
                string fileName = file.Name;
                string type = MimeMapping.GetMimeMapping(file.Name);
                long size = file.Length;
                lstFileInformation.Add(new FileInformation()
                {
                    name = fileName,
                    type = type,
                    size = size,
                    count = null,
                    path = dir + "/"
                });
            }
        }
    }
}