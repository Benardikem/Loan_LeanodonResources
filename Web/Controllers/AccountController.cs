
using Web.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Web.Application;
using API.Utilities;
using Web.Interfaces;
using Web.Codes;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private IAppManager _appMgr;
        public AccountController()
        {
            _appMgr = new AppManager();
        }
        [AllowAnonymous]
        //public ActionResult Login(string returnUrl)
        //{
        //    Session.Abandon();
        //    FormsAuthentication.SignOut();
        //    ViewBag.ReturnUrl = returnUrl;
        //    return View();
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    try
        //    {
        //        Func<ActionResponse> function = new Func<ActionResponse>(() => _appMgr.IsLogin(model.UserName, model.Password));
        //        ActionResponse result = await Task.Run(function);
        //        if (result.ResponseCode == "00")
        //        {
        //            CustomAuthentication.CreateAuthenticationTicket(model.UserName, model.RememberMe);
        //            return RedirectToLocal(returnUrl);
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", result.ResponseMsg);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        General.LOGGER.Error(ex.Source, ex);
        //        ModelState.AddModelError("", "Internal Server Error");
        //    }
        //    return View(model);
        //}
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult LogOff()
        {
            this.Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult AccessDenied(string returnUrl)
        {
            return View();
        }
        public ActionResult ChangePassword()
        {
            var model = new ChangePasswordViewModel { Username = LoggedInUser.UserName };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            model.Message = "";
            if (ModelState.IsValid)
            {
                var response = await _appMgr.ChangePasswordAsync(model.Username, model.OldPassword, model.NewPassword);
                if (response.ResponseCode == "00")
                {
                    model.Message = "Password Change Successful";
                }
                else
                {
                    ModelState.AddModelError("", response.ResponseMsg);
                }
            }
            return View(model);
        }
                
        [AllowAnonymous]
        public ActionResult ForgotPassword(string returnUrl)
        {            
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ForgotPassword(ResetPasswordModel model, string returnUrl)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    try
        //    {
        //        Func<ActionResponse> function = new Func<ActionResponse>(() => _appMgr.ResetPass(model.UserID));
        //        ActionResponse result = await Task.Run(function);
        //        if (result.ResponseCode == "00")
        //        {
        //            CustomAuthentication.CreateAuthenticationTicket(model.UserName, model.RememberMe);
        //            return RedirectToLocal(returnUrl);
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", result.ResponseMsg);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        General.LOGGER.Error(ex.Source, ex);
        //        ModelState.AddModelError("", "Internal Server Error");
        //    }
        //    return View(model);
        //}
    }
}