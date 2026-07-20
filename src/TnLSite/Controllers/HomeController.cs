using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Plisky.Diagnostics;
using TnLSite.Models;

namespace TnLSite.Controllers {

    [Route("[controller]")]
    public class HomeController : Controller {
        protected Bilge b = new Bilge();

        public IActionResult Index() {
            return View();
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("webhook")]
        public ActionResult WebHookGet() {
            return Ok();
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> WebHook(string payload) {
            Request.EnableBuffering();

            string bodyJson = string.Empty;
            if (Request.Body.CanRead) {
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true)) {
                    bodyJson = await reader.ReadToEndAsync();
                }

                Request.Body.Position = 0;
            }

            string fullUrl = Request.GetDisplayUrl();
            string key = Request.Query["key"].ToString();
            string urlPayload = string.Empty;

            foreach (string l in Request.Query.Keys) {
                urlPayload += $"{l}={Request.Query[l]},";
            }

            string logContent = "URL: " + fullUrl + Environment.NewLine
                + "Parameter: " + urlPayload + Environment.NewLine;

            b.Info.Log(logContent, bodyJson);

            IActionResult result;
            if (!string.Equals(key, "mykey", StringComparison.Ordinal)) {
                result = Unauthorized();
            } else {
                result = Ok();
            }

            return result;
        }
    }
}
