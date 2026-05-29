using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HomeModel;
using TPM_QAS.Filters;
using System.Security.Cryptography;
using TPM_QAS.Helpers;
using System.IO;
using System.Threading.Tasks;

namespace TPM_QAS.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            string userAD = string.Empty;
            string vUserAD = string.Empty;
            string systemName = string.Empty;

            AuthenticatorModel model = new AuthenticatorModel();
            //userAD = Environment.UserName;
            userAD = System.Web.HttpContext.Current.User.Identity.Name;
            systemName = ConfigurationManager.AppSettings["SystemName"];
            string[] splitWords = userAD.Split('\\');
            vUserAD = splitWords[splitWords.Length - 1];

            DB db = new DB();
            model = await db.ValidateUserInfo(vUserAD, systemName);
            ViewBag.Validate = model.VALID_USER;
            ViewBag.userAD = userAD;
            ViewBag.userAD2 = vUserAD;
            if (model.VALID_USER == true)
            {
                Session["AclUser"] = new ACL_UserObj
                {
                    ID_ACL_USER = model.ID_ACL_USER,
                    ID_ACL_ROLE = model.ID_ACL_ROLE,
                    ID_ACL_RESOURCE = model.ID_ACL_RESOURCE,
                    USER_ID = model.USER_ID,
                    USR_EMAIL = model.USR_EMAIL,
                    COMPANY = model.COMPANY,
                    EMP_NO = model.EMP_NO,
                    EMP_NAME = model.EMP_NAME,
                    ROLE_NAME = model.ROLE_NAME,
                    ROLE_DESC = model.ROLE_DESC,
                    RESOURCE_NAME = model.RESOURCE_NAME,
                    RESOURCE_DESC = model.RESOURCE_DESC
                };
            }

            return View(model);

        }

        public ActionResult Help()
        {
            return View();
        }

        #region Login Logout
        public async Task<ActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(AuthenticatorModel model)
        {
            if (ModelState.IsValid)  //checking model is valid or not
            {
                string systemName = string.Empty;
                systemName = ConfigurationManager.AppSettings["SystemName"];
                string passwordTMP = model.PASSWORD;
                DB db = new DB();
                model = await db.ValidateUserInfo(model.LOGIN_ID, systemName);

                if (VerifyHashedPassword(model.PASSWORD, passwordTMP))
                {
                    model.VALID_USER = true;
                }
                else
                {
                    model.VALID_USER = false;
                }

                ViewBag.Validate = model.VALID_USER;

                ModelState.Clear();
                if (model.VALID_USER == true)
                {
                    Session["AclUser"] = new ACL_UserObj
                    {
                        ID_ACL_USER = model.ID_ACL_USER,
                        ID_ACL_ROLE = model.ID_ACL_ROLE,
                        ID_ACL_RESOURCE = model.ID_ACL_RESOURCE,
                        USER_ID = model.USER_ID,
                        USR_EMAIL = model.USR_EMAIL,
                        COMPANY = model.COMPANY,
                        EMP_NO = model.EMP_NO,
                        EMP_NAME = model.EMP_NAME,
                        ROLE_NAME = model.ROLE_NAME,
                        ROLE_DESC = model.ROLE_DESC,
                        RESOURCE_NAME = model.RESOURCE_NAME,
                        RESOURCE_DESC = model.RESOURCE_DESC
                    };
                    //return RedirectToAction("Cap7", model);
                    return RedirectToAction("Menu", "Home");
                }
                else
                {
                    return View(model);
                }

            }
            else
            {
                ModelState.AddModelError("", "Error in saving data");
                return View();
            }

        }

        public async Task<ActionResult> LogOut()
        {
            Session.Abandon();
            return RedirectToAction("Login", "Home");
        }
        #endregion

        #region Menu_Sidebar
        [SessionExpire]
        public async Task<ActionResult> Menu()
        {
            return View();
        }

        [SessionExpire]
        public async Task<ActionResult> SideBar()
        {
            DB db = new DB();
            int roleID = (Session["AclUser"] as ACL_UserObj).ID_ACL_ROLE;
            string systemName = ConfigurationManager.AppSettings["SystemName"];
            var dt = await db.sideBarDB(roleID, systemName);
            List<SideBarContent> SideBarModel = CommonMethod.ConvertToList<SideBarContent>(dt);
            return View(SideBarModel);
        }
        #endregion

        #region Change Password
        [SessionExpire]
        public async Task<ActionResult> ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> ChangePassword(ChangePasswordModel ChangePasswordModel)
        {
            try
            {
                var aclUser = Session["AclUser"];
                DB db = new DB();
                DataTable dt = await db.oldPassword((aclUser as ACL_UserObj).ID_ACL_USER);
                //DataTable dt = db.oldPassword(25);
                bool a = VerifyHashedPassword(dt.Rows[0][0].ToString(), ChangePasswordModel.OLD_PASSWORD);
                if (VerifyHashedPassword(dt.Rows[0][0].ToString(), ChangePasswordModel.OLD_PASSWORD))
                {
                    if (await db.NewPassWord((aclUser as ACL_UserObj).ID_ACL_USER, HashPassword(ChangePasswordModel.NEW_PASSWORD)) == "Y")
                    {
                        ViewData["Message"] = "Password is successfully changed. Please relogin again.";
                        ViewData["MessageType"] = "Y";
                    }
                    else
                    {
                        ViewData["Message"] = "Failed change your password. Please try again.";
                        ViewData["MessageType"] = "E";
                    }
                }
                else
                {
                    ViewData["Message"] = "Incorrect old password. Please try again.";
                    ViewData["MessageType"] = "E";
                }
                ModelState.Clear();
            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
                ViewData["Message"] = ex.Message;
                ViewData["MessageType"] = "E";
            }
            return View();
        }

        public string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }

        public bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] buffer4;
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] src = Convert.FromBase64String(hashedPassword);
            //if ((src.Length != 0x31) || (src[0] != 0))
            //{
            //    return false;
            //}
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }

            return buffer3.SequenceEqual(buffer4);
        }
        #endregion

        #region General Function
        public async Task<ActionResult> ViewFile(string fileName)
        {
            // User credentials for impersonation
            string login = "tpm_cmms_app";
            string domain = "TORAY";
            string password = "tpmcmms@pp";

            string folderPath = @"\\10.200.0.19\tpm\tpm-engineering\5. MECHANICAL\4. General\CMMS\CMMS 2.0\fileAttachment\";

            // Construct the full path to the file
            string filePath = Path.Combine(folderPath, fileName);

            try
            {
                using (UserImpersonation user = new UserImpersonation(login, domain, password))
                {
                    if (user.ImpersonateValidUser())
                    {
                        if (System.IO.File.Exists(filePath))
                        {
                            byte[] fileData = System.IO.File.ReadAllBytes(filePath);

                            // Determine the content type based on the file extension
                            string contentType;
                            if (Path.GetExtension(fileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                            {
                                contentType = "application/pdf";
                            }
                            else if (Path.GetExtension(fileName).Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                     Path.GetExtension(fileName).Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                     Path.GetExtension(fileName).Equals(".png", StringComparison.OrdinalIgnoreCase))
                            {
                                contentType = "image/jpeg"; // Change content type based on the actual image format support
                            }
                            else
                            {
                                // Unsupported file type, return 404 (Not Found) status
                                return HttpNotFound("Unsupported file type.");
                            }

                            // Return the file content as a response with the appropriate content type
                            return File(fileData, contentType);
                        }
                    }
                    // If the file is not found or impersonation fails, return a 404 (Not Found) status
                    return HttpNotFound("File not found or access denied.");
                }
            }
            catch(Exception ex)
            {
                return HttpNotFound(ex.Message);
            }
        }
        #endregion

        #region Dashboard
        public async Task<ActionResult> Cap7()
        {
            return View();
        }
        public async Task<ActionResult> FBD5()
        {
            return View();
        }
        #endregion
    }
}