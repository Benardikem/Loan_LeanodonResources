using Web.Attributes;
using Web.Extensions;
using Web.Models;
using System.Linq;
using System.Web.Mvc;
using API.Models;
using API.Utilities;

namespace Web.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {
        #region Roles
        [CustomAuthorize]
        public ActionResult Index()
        {
            RoleModel model = new RoleModel();
            try
            {
                using (MyDbContext _db = new MyDbContext())
                {
                    model.RoleItems = _db.Roles.Where(a => !a.Deleted)
                                                .ToList()
                                                .Select(a => new RoleItem
                                                {
                                                    Id = a.ID,
                                                    Name = a.Name,
                                                    Description = a.Description,
                                                    //UserCategory = a.UserCategory.CategoryName,
                                                    AddedBy = a.AddedBy,
                                                    AddedDate = a.AddedDate,
                                                    Active = a.Active ? "Yes" : "No",
                                                    ModifiedDate = a.ModifiedDate,
                                                    Approved = a.Approved ? "Yes" : "No"
                                                })
                                                .OrderBy(a => a.Name)
                                                .ToList();

                    //model.UserCategoryItems = _db.UserCategories.ToList()
                    //                          .Select(a => new UserCategoryItem
                    //                          {
                    //                              Id = a.Id,
                    //                              CategoryName = a.CategoryName
                    //                          })
                    //                          .OrderBy(a => a.CategoryName)
                    //                          .ToList();

                    model.SubMenuItems = (from s in _db.SubMenus
                                          where s.Parent == 0 && s.Active
                                          select new SubMenuItem
                                          {
                                              Id = s.ID,
                                              Name = s.Name,
                                              MenuName = s.Menu.Name,
                                              Order = s.Order,
                                              MenuOrder = s.Menu.Order
                                          }).OrderBy(a => a.MenuName)
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
            RoleModel model = new RoleModel();
            try
            {
                using (MyDbContext _db = new MyDbContext())
                {
                    model.RoleItems = _db.Roles.Where(a => !a.Deleted && !a.Approved)
                                                .ToList()
                                                .Select(a => new RoleItem
                                                {
                                                    Id = a.ID,
                                                    Name = a.Name,
                                                    Description = a.Description,
                                                    //UserCategory = a.UserCategory.CategoryName,
                                                    AddedBy = a.AddedBy,
                                                    AddedDate = a.AddedDate,
                                                    Active = a.Active ? "Yes" : "No",
                                                    ModifiedDate = a.ModifiedDate,
                                                    Approved = a.Approved ? "Yes" : "No"
                                                })
                                                .OrderBy(a => a.Name)
                                                .ToList();

                    model.SubMenuItems = (from s in _db.SubMenus
                                          where s.Parent == 0 && s.Active
                                          select new SubMenuItem
                                          {
                                              Id = s.ID,
                                              Name = s.Name,
                                              MenuName = s.Menu.Name,
                                              Order = s.Order,
                                              MenuOrder = s.Menu.Order
                                          }).OrderBy(a => a.MenuName)
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