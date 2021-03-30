using Web.Attributes;
using Web.Extensions;
using Web.Models;
using System.Linq;
using System.Web.Mvc;
using API.Models;
using API.Utilities;
using Web.Codes.Constants;

namespace Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        #region Admin
        [CustomAuthorize]
        public ActionResult Index()
        {
            UserModel model = new UserModel();
            try
            {
                using (MyDbContext _db = new MyDbContext())
                {
                    model.UserItems = _db.Users.Where(a => !a.Deleted && a.UserCategory == UserCategory.ADMIN)
                                                .ToList()
                                                .Select(a => new UserItem
                                                {
                                                    UserID = a.UserID,
                                                    DisplayName = a.DisplayName,
                                                    Number = a.Phone,
                                                    AddedBy = a.AddedBy,
                                                    EmailAddress = a.Email,
                                                    UserCategory = a.UserCategory,
                                                    AddedDate = a.DateAdded,
                                                    Active = a.Active ? "Yes" : "No",
                                                    ModifiedDate = a.DateModified,
                                                    Approved = a.Approved ? "Yes" : "No",
                                                    Roles = a.UserRoles.Select(r => r.Role.Name).ToList()
                                                })
                                                .OrderBy(a => a.DisplayName)
                                                .ToList();

                    model.RoleItems = (from s in _db.Roles
                                       where s.Approved && s.Active
                                       select new RoleItem
                                       {
                                           Id = s.ID,
                                           Name = s.Name
                                       }).OrderBy(a => a.Name)
                                        .ToList();
                }
            }
            catch (System.Exception ex)
            {
                General.LogExceptions("", ex);
                ViewBag.MessageText = ex.Message.ToString();
                ViewBag.MessageType = MessageLabelControl.MessageType.danger;
            }
            return View(model);
        }


        [CustomAuthorize]
        public ActionResult Approval()
        {
            UserModel model = new UserModel();
            try
            {
                using (MyDbContext _db = new MyDbContext())
                {
                    model.UserItems = _db.Users.Where(a => !a.Deleted && !a.Approved && a.UserCategory == UserCategory.ADMIN)
                                                .ToList()
                                                .Select(a => new UserItem
                                                {
                                                    UserID = a.UserID,
                                                    DisplayName = a.DisplayName,
                                                    UserCategory = a.UserCategory,
                                                    EmailAddress = a.Email,
                                                    Number = a.Phone,
                                                    AddedBy = a.AddedBy,
                                                    AddedDate = a.DateAdded,
                                                    Active = a.Active ? "Yes" : "No",
                                                    ModifiedDate = a.DateModified,
                                                    Approved = a.Approved ? "Yes" : "No"
                                                })
                                                .OrderBy(a => a.DisplayName)
                                                .ToList();

                    model.RoleItems = (from s in _db.Roles
                                       where s.Approved && s.Active
                                       select new RoleItem
                                       {
                                           Id = s.ID,
                                           Name = s.Name
                                       }).OrderBy(a => a.Name)
                                        .ToList();
                }
            }
            catch (System.Exception ex)
            {
                General.LogExceptions("", ex);
                ViewBag.MessageText = ex.Message.ToString();
                ViewBag.MessageType = MessageLabelControl.MessageType.danger;
            }
            return View(model);
        }
        #endregion
    }
}