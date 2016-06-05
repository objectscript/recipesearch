using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace RecipesSearch.WebApplication.Controllers
{
    public class ProxyController : Controller
    {
        public FileResult Index(string url)
        {
            url = HttpUtility.UrlDecode(url);
            byte[] content;
		
            var req = HttpWebRequest.Create(url);
            req.Method = HttpContext.Request.HttpMethod;

            using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
            {
                // Get the stream containing content returned by the server.
                using (Stream dataStream = response.GetResponseStream())
                {
                    content = ReadFully(dataStream);
                }

                return new FileContentResult(content, response.ContentType);
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
