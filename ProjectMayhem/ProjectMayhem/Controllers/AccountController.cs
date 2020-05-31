using System;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ProjectMayhem.Models;

namespace ProjectMayhem.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

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
            if (model.Username.IndexOf('@') > -1)
            {
                //Validate email format
                string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                       @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                          @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                Regex re = new Regex(emailRegex);
                if (!re.IsMatch(model.Username))
                {
                    ModelState.AddModelError("", "Email is not valid");
                }
            }
            else
            {
                //validate Username format
                string emailRegex = @"^[a-zA-Z0-9]*$";
                Regex re = new Regex(emailRegex);
                if (!re.IsMatch(model.Username))
                {
                    ModelState.AddModelError("", "Username is not valid");
                }
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var UserName = model.Username;
            ApplicationUser user;
            if (UserName.IndexOf('@') > -1)
            {
                user = await UserManager.FindByEmailAsync(model.Username);
            }
            else
                user = await UserManager.FindByNameAsync(model.Username);
            if (user != null)
            {
                UserName = user.UserName;
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    ModelState.AddModelError("", "You must have a confirmed email to log on.");
                    return View(model);
                }
            }

            var result = await SignInManager.PasswordSignInAsync(UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> AcceptInvite(string userId, string InviteToken, string EmailConf)
        {

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(InviteToken))
                return View("Error");
            var result = await UserManager.VerifyUserTokenAsync(userId, "Invite", InviteToken);
            if (result == true)
            {
                var EmailRes = await UserManager.ConfirmEmailAsync(userId, EmailConf);
                return View(EmailRes.Succeeded ? "AcceptInvite" : "Error");
            }
            else return View("Error");
            
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AcceptInvite(InvitationViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(Request.QueryString["userId"]);
                var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                await UserManager.ResetPasswordAsync(user.Id, token, model.Password);
                user.UserName = model.Username;
                var request = await UserManager.UpdateAsync(user);
                if (request.Succeeded)
                    return RedirectToAction("Login", "Account");
                AddErrors(request);
            }
            return View(model);
        }

        //
        // GET: /Account/Register
        [Authorize]
        public ActionResult Register()
        {
            AccountManagementViewModel mymodel = new AccountManagementViewModel();
            var error = (String)TempData["DeleteError"];
            var confirmation = (string)TempData["DelConfirmation"];
            if (!String.IsNullOrEmpty(error))
            {
                ModelState.AddModelError("Deletion", error);
                TempData.Remove("DeleteError");
            }
            else if(!string.IsNullOrEmpty(confirmation))
            {
                ViewBag.DelConfirmation = confirmation;
                confirmation = "";
            }
            var currentUser = User.Identity.GetUserId();
            mymodel.TeamMembers = UserManager.Users.Where(x => x.teamLead.Id == currentUser).ToList();
            return View(mymodel);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Register(AccountManagementViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (UserManager.FindByEmail(model.Email) == null)
                {
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email, teamLead = UserManager.FindById(User.Identity.GetUserId()) };
                    var symbolStr = "@$.,!%*?&";
                    var RandPassword = Membership.GeneratePassword(20, 5) + new Random().Next(9).ToString() + symbolStr[new Random().Next(8)];
                    var result = await UserManager.CreateAsync(user, RandPassword);
                    if (result.Succeeded)
                    {
                        string EmailCode = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        string InvCode = await UserManager.GenerateUserTokenAsync("Invite", user.Id);
                        var InviteUrl = Url.Action("AcceptInvite", "Account", new { userId = user.Id, InviteToken = InvCode, EmailConf = EmailCode }, Request.Url.Scheme);
                        await UserManager.SendEmailAsync(user.Id,
                           "Invitation", "Hello,<br><br>You can accept the invitation by clicking <a href=\""
                           + InviteUrl + "\">here</a><br><br>Best Regards,<br> ProjectMayhem Team");

                        return RedirectToAction("Members", "Team");
                    }
                    AddErrors(result);
                }
                else
                    ModelState.AddModelError("", "The User already exist");
            }
            var currentUser = User.Identity.GetUserId();
            model.TeamMembers = UserManager.Users.Where(x => x.teamLead.Id == currentUser).ToList();
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> DeleteAccount(AccountManagementViewModel model)
        {
            var SelectedId = model.EmpId;
            Debug.WriteLine(SelectedId + " This ID");
            var currentUser = User.Identity.GetUserId();
            var deletionUser = UserManager.FindById(SelectedId);
            if (UserManager.Users.Where(x => x.teamLead.Id == deletionUser.Id).ToArray().Length == 0 && deletionUser.teamLead.Id == currentUser && deletionUser != null)
            {
                await UserManager.DeleteAsync(deletionUser);
                TempData["DelConfirmation"] = "The user was deleted";
            }
            else if (deletionUser == null)
                TempData["DeleteError"] = "The Team member does not exist";
            else
                TempData["DeleteError"] = "Cannot delete a member. User is a Team Leader or you do not have permission";
            return this.RedirectToAction("Register");
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
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

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
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
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
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