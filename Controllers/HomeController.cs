using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OTPLogin.Models;
 
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Collections.Specialized;
using System.Web;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Renci.SshNet;
using System.Net.WebSockets;

namespace OTPLogin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private otpContext _otpContext;
        private IConfiguration _configuration;
        private IHttpContextAccessor _contextAccessor;



        public HomeController(ILogger<HomeController> logger, otpContext otpContext, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _otpContext = otpContext;
            _configuration = configuration;
            _contextAccessor = contextAccessor;
        }

        public IActionResult Index()
        {
      
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public async Task<IActionResult> RegisterNewUser(User user)
        {
            user.Createddate = DateTime.Now;
            _otpContext.User.Add(user);
        await    _otpContext.SaveChangesAsync();
            return RedirectToAction("Welcome", "Home", user);
        }
        public IActionResult Welcome(User user)
        {
            return View(user);
        }
        public IActionResult Register(User user)
        {
            return View(user);
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public     IActionResult  ValidOtp( string mobileNo)
        {
           
            var User = _otpContext.User.Where(x => x.MobileNo == mobileNo).FirstOrDefault();
             
            if (User == null)
            {
                User = new User();
                User.MobileNo = (mobileNo);
                return RedirectToAction("Register", "Home",User);
            }
            return RedirectToAction("Welcome", "Home", User);

        }
        
                public JsonResult verifyotp(string otp)
        {

           bool otpValid= _contextAccessor.HttpContext.Session.GetString("otp") == otp;
            return Json(new { isSuccess = otpValid });




      


        }
        public JsonResult GenerateOtp(string value)
        {
            string ApiKey = _configuration["SMSApiKey"].ToString();
            int otp = new Random().Next(100000, 999999);

            

            string msg = HttpUtility.UrlEncode("Otp is " + otp);
            using (var client = new WebClient())
            {
                //byte[] res = client.UploadValues("https://api.textlocal.in/send/", new NameValueCollection()
                //{
                //    { "apiKey",ApiKey},
                //    {"numbers", value },
                //    {"message",msg },
                //    { "sender","TXTLCL"}
                //});
                //var json = JObject.Parse(Encoding.UTF8.GetString(res));
                //var status = json["status"].ToString();
                _contextAccessor.HttpContext.Session.SetString("otp", otp.ToString());
                return Json(new { isSuccess = true });
                    //return Json(new {isSuccess=( status == "success") });
              
         

              
            }

                
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
