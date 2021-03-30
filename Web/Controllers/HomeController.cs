using API;
using API.Models;
using API.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Web.Application;
using Web.Codes;
using Web.Extensions;
using Web.Interfaces;
using Web.Models;

namespace Web.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private IAppManager _appMgr;
        public HomeController()
        {
            _appMgr = new AppManager();
        }

        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                Func<ActionResponse> function = new Func<ActionResponse>(() => _appMgr.IsLogin(model.CustomerID, model.Password));
                ActionResponse result = await Task.Run(function);
                if (result.ResponseCode == "00")
                {
                    CustomAuthentication.CreateAuthenticationTicket(model.CustomerID, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                    //return RedirectToLocal(returnUrl, model.UserName);
                }
                else
                {
                    ModelState.AddModelError("", result.ResponseMsg);
                    System.Web.Helpers.AntiForgery.Validate();
                }
            }
            catch (Exception ex)
            {
                General.LOGGER.Error(ex.Source, ex);
                ModelState.AddModelError("", "Internal Server Error");
            }
            return View(model);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Dashboard");
        }

        [AllowAnonymous]
        public ActionResult Error(string msg)
        {
            ErrorModel model = new ErrorModel { Message = "Error" };
            if (!string.IsNullOrEmpty(msg))
                model.Message = msg;
            Exception exception = Server.GetLastError();
            if (exception != null)
            {
                General.LOGGER.Error(exception.Source, exception);
            }
            return View(model);
        }

        
        [AllowAnonymous]
        public ActionResult TestEmail()
        {
            EmailItem _mail = new EmailItem
            {
                Title = "Test Email",
                Body = "We are testing",
                To = new List<string> { "wintopeo@windraysystems.com", "dwintope@yahoo.com" },
            };
            Messaging.LogMail(_mail);
            return View();
        }

        public ActionResult Aboutus()
        {
            return View();
        }

        public ActionResult FAQ()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Newcustomer(string returnUrl)
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Newcustomer(SignUpViewModel model, string returnUrl)
        {
            model.Message = "";
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                Func<ActionResponse> function = new Func<ActionResponse>(() => _appMgr.IsSignUp(model.DisplayName, model.EmailAddress, model.PhoneNumber, model.ResidentialAddress, model.ZipCode, model.Password, model.SecurityQ, model.SecurityA));
                ActionResponse result = await Task.Run(function);
                if (result.ResponseCode == "00")
                {
                    model.Message = $"Welcome {model.DisplayName}, Your Registration was Successful.";
                }
                else
                {
                    ModelState.AddModelError("", result.ResponseMsg);
                }
            }
            catch (Exception ex)
            {
                General.LOGGER.Error(ex.Source, ex);
                ModelState.AddModelError("", "Internal Server Error");
            }
            return View(model);
        }
    }
}