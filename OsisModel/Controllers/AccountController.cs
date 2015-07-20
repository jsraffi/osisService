using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using OsisModel.Models;
using System.Configuration;
using PagedList;
using System.Data.Entity;

namespace OsisModel.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public JsonResult GetAcademicYearBySchoolID(int id)
        {
            var db = new ApplicationDbContext();
            
            var acyear = db.AcademicYears.Where(s => s.SchoolRefID == id).Select(a => new { Text = a.DisplayYear, Value = a.AcademicYearID }).ToList();

            return Json(acyear, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult doesUserNameExist(string UserName)
        {
            var db = new ApplicationDbContext();
            var user = db.Users.Where(c => c.UserName == UserName).FirstOrDefault();

            return Json(user == null);
        }





        [Authorize(Roles = "Admin")]
        public ActionResult Index(int? page)
        {
            var Db = new ApplicationDbContext();
            var users = Db.Users.OrderBy(u => u.UserName).ToList();
            var pageNumber = page ?? 1;

            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize"]);
            
            return View(users.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id, ManageMessageId? Message = null)
        {
            var Db = new ApplicationDbContext();
            var user = Db.Users.First(u => u.UserName == id);
            var model = new EditUserViewModel(user);
            ViewBag.MessageId = Message;
            
            var userprefer = Db.UserPreferences.Where(a => a.UserName == id).Select(x=> new {x.SchoolRefID,x.AcademicYearRefID}).FirstOrDefault();
            
            ViewBag.SchoolID = new SelectList(Db.Schools.AsNoTracking().Select(x => new { x.SchoolID, x.SchoolName }), "SchoolID", "SchoolName",userprefer.SchoolRefID);
            ViewBag.AcademicYearID = new SelectList(Db.AcademicYears.AsNoTracking().Select(x => new { x.AcademicYearID, x.DisplayYear }), "AcademicYearID", "DisplayYear",userprefer.AcademicYearRefID);

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
            var dbmgtm = new ApplicationDbContext();
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.UserName);

                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;

                using (var securitytrans = dbmgtm.Database.BeginTransaction())
                {

                    //try
                    //{
                    
                        IdentityResult result;
                        result = await UserManager.UpdateAsync(user);

                        if (result.Succeeded)
                        {
                            
                            if(model.AcademicYearID > 0 && model.SchoolID > 0 )
                            { 
                                var userid = dbmgtm.UserPreferences.Where(a => a.UserName == model.UserName).Select(x => new { x.UserID }).FirstOrDefault();
                                UserPreference up = new UserPreference();
                                up.UserID = userid.UserID;
                                up.SchoolRefID = model.SchoolID;
                                up.AcademicYearRefID = model.AcademicYearID;

                                dbmgtm.UserPreferences.Attach(up);
                                dbmgtm.Entry(up).Property("SchoolRefID").IsModified = true;
                                dbmgtm.Entry(up).Property("AcademicYearRefID").IsModified = true;
                                //dbmgtm.Entry(up).State = EntityState.Modified;
                            
                                await dbmgtm.SaveChangesAsync();
                            
                                securitytrans.Commit();
                                return RedirectToAction("Index", "Account");
                              }
                            else
                            {
                                securitytrans.Rollback();
                                return RedirectToAction("Index", "Account");

                            }
                            //await SignInAsync(user, isPersistent: false);
                            
                        }

                //        else
                //        {
                //            ModelState.AddModelErrsor(string.Empty, "There are error in the form");
                //        }
                //    }
                //    catch (Exception e)
                //    {
                //        securitytrans.Rollback();
                //        ModelState.AddModelError(e.Message, "There are error in the form");
                //    }
                        //return View(model);
                }
                  
            }
            ViewBag.SchoolID = new SelectList(dbmgtm.Schools.AsNoTracking().Select(x => new { x.SchoolID, x.SchoolName }), "SchoolID", "SchoolName",model.SchoolID);
            ViewBag.AcademicYearID = new SelectList(dbmgtm.AcademicYears.AsNoTracking().Select(x => new { x.AcademicYearID, x.DisplayYear }), "AcademicYearID", "DisplayYear",model.AcademicYearID);

            return View(model);
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
                var usermgmt = new ApplicationDbContext();

                ViewBag.SchoolID = new SelectList(usermgmt.Schools.AsNoTracking().Select(x => new { x.SchoolID, x.SchoolName }), "SchoolID", "SchoolName");
                ViewBag.AcademicYearID = new SelectList(usermgmt.AcademicYears.AsNoTracking().Select(x => new { x.AcademicYearID, x.DisplayYear }), "AcademicYearID", "DisplayYear");

            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            var dbmgtm = new ApplicationDbContext();

            IdentityResult result;
            
            using (var securitytrans = dbmgtm.Database.BeginTransaction())
            {
                        try 
                        {
                                if (ModelState.IsValid)
                                {
                                    var user = new ApplicationUser()
                                    {
                                        UserName = model.UserName,
                                        FirstName = model.FirstName,
                                        LastName = model.LastName,
                                        Email = model.Email
                                    };
                

                                    var um = new UserManager<ApplicationUser>(
                                    new UserStore<ApplicationUser>(dbmgtm));

                                    result = await um.CreateAsync(user, model.Password);
                                    

                                    if (result.Succeeded)
                                    {   
                                        
                                        UserPreference userpref = new UserPreference()
                                        {
                                            UserID = user.Id,
                                            UserName = model.UserName,
                                            SchoolRefID = model.SchoolID,
                                            AcademicYearRefID = model.AcademicYearID
                                        };
                                        dbmgtm.UserPreferences.Add(userpref);

                                        await dbmgtm.SaveChangesAsync();
                                        securitytrans.Commit();
                                        //await SignInAsync(user, isPersistent: false);
                                        return RedirectToAction("Index", "Account");
                                    }
                            }
                          
                    }
                    catch (Exception e)
                    {
                                                                         
                        securitytrans.Rollback();
                        ModelState.AddModelError("", e.Message);
                        ViewBag.SchoolID = new SelectList(dbmgtm.Schools.AsNoTracking().Select(x => new { x.SchoolID, x.SchoolName }), "SchoolID", "SchoolName", model.SchoolID);
                        ViewBag.AcademicYearID = new SelectList(dbmgtm.AcademicYears.AsNoTracking().Select(x => new { x.AcademicYearID, x.DisplayYear }), "AcademicYearID", "DisplayYear", model.AcademicYearID);
                       return View(model);
                    }
                }
            
                ViewBag.SchoolID = new SelectList(dbmgtm.Schools.AsNoTracking().Select(x => new { x.SchoolID, x.SchoolName }), "SchoolID", "SchoolName", model.SchoolID);
                ViewBag.AcademicYearID = new SelectList(dbmgtm.AcademicYears.AsNoTracking().Select(x => new { x.AcademicYearID, x.DisplayYear }), "AcademicYearID", "DisplayYear", model.AcademicYearID);
            
            return View(model);                
                      
            } 
            
                    
            

            // If we got this far, something failed, redisplay form
           
    

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}