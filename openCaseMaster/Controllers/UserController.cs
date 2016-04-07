using CaptchaMvc.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using openCaseMaster.Models;
using openCaseMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace openCaseMaster.Controllers
{
    public class UserController : Controller
    {
        [AllowAnonymous]
        // GET: User
        public ActionResult Login(string ReturnUrl)
        {
            //Request.IsAjaxRequest();
            //headh中 X-Requested-With:XMLHttpRequest
            //ajax 调用时反悔错误信息(coding)
            ViewBag.returnUrl = ReturnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]//对应@Html.AntiForgeryToken()
        public ActionResult Login(LoginViewModel model, string ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            QCTESTEntities qc = new QCTESTEntities();
            var loginUser = qc.admin_user.Where
                (t => t.Username == model.UserName && t.Password == model.Password).FirstOrDefault();
            if (loginUser == null)
            {
                ModelState.AddModelError("", "用户名或密码错误。");
                return View(model);
            }
            string userRole = "user";
            //设置权限组
            switch (loginUser.Type)
            {
                case 1:
                    userRole += ",admin";
                    break;
                case null:
                    //userRole = "guest";
                    break;
                default:
                    break;
            }
            

            JObject userJ = new JObject();
            userJ["ID"] = loginUser.ID;
            userJ["Roles"] = userRole;
            userJ["Permission"] = loginUser.Permission;


            
    

            //创建身份验证票据 
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                                           loginUser.Name,
                                           DateTime.Now,
                                           DateTime.Now.AddHours(2),
                                           false,
                                           userJ.ToString(),//用户组暂不处理
                                           //loginUser.Type.ToString(),//用户所属的角色字符串 
                                           FormsAuthentication.FormsCookiePath);
            //加密身份验证票据 
            string hash = FormsAuthentication.Encrypt(ticket);

           
            //创建要发送到客户端的cookie 
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hash);

            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }
            //把准备好的cookie加入到响应流中 
            Response.Cookies.Add(cookie);
            loginUser.LastDate = DateTime.Now;
            qc.SaveChanges();
            return RedirectToLocal(ReturnUrl);



        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [CaptchaVerify("Captcha is not valid")]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                QCTESTEntities QC_DB = new QCTESTEntities();

                var ss = QC_DB.admin_user.FirstOrDefault(t => t.Username == model.UserName);
                if (ss != null)
                {
                    ModelState.AddModelError("", "用户名已存在");
                    return View(model);
                }
                admin_user tmp = new admin_user();
                tmp.Username = model.UserName;
                tmp.Type = null;//游客
                tmp.Name = model.Name == null ? model.UserName : model.Name;
                tmp.GreatDate = DateTime.Now;
                tmp.Password = model.Password;

                tmp.Permission = "";

                QC_DB.admin_user.Add(tmp);

                //私有项目
                var newP = new project();
                newP.Pname = "private project";
                newP.userID = tmp.ID;
                QC_DB.project.Add(newP);


                //私有框架
                var newF = new caseFramework();
                newF.workName = "你的框架";
                newF.userID = tmp.ID;
                QC_DB.caseFramework.Add(newF);

                QC_DB.SaveChanges();
                return RedirectToAction("Login", "User");
                    

            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }




        /// <summary>
        /// 跳转URL
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [NonAction] 
        private ActionResult RedirectToLocal(string ReturnUrl)
        {
            if (Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }
            return RedirectToAction("Index", "TestCase");
        }


        [Authorize(Roles="admin")]
        public ActionResult UserManage()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        public string userList(int page, int rows)
        {
            UserListModel ulm = new UserListModel(page, rows);

            string json = JsonConvert.SerializeObject(ulm);

            return json;


        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult editUser(int id)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();
            var user = QC_DB.admin_user.First(t => t.ID == id);


            var tp = from t in QC_DB.user_type
                     select t;


            SelectList selList1 = new SelectList(tp, "ID", "Type");
            ViewData["typeList"] = selList1.AsEnumerable();

            var pp = (from t in QC_DB.project
                      select new checkListModel
                     {
                         Value = t.ID.ToString(),
                         Text = t.Pname
                     }).ToList();
            if (user.Permission != null)
            {
                var psn = user.Permission.Split(',');

                foreach (var p in pp)
                {
                    if (psn.Contains(p.Value))
                        p.isCheck = true;
                    else
                        p.isCheck = false;
                }
            }


            ViewData["proList"] = pp;  


            return PartialView("_editUser", user);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public string editUser(admin_user user)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();
            var u = QC_DB.admin_user.First(t => t.ID == user.ID);
            u.Name = user.Name;
            u.Permission = user.Permission;
            u.Type = user.Type;
            QC_DB.SaveChanges();

            return "";
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public string deleteUser(int id)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();
            var u = QC_DB.admin_user.First(t => t.ID == id);
            QC_DB.admin_user.Remove(u);
            QC_DB.SaveChanges();
            return "";
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult addUser()
        {
            QCTESTEntities QC_DB = new QCTESTEntities();
          
            var tp = from t in QC_DB.user_type
                     select t;

            SelectList selList1 = new SelectList(tp, "ID", "Type");
            ViewData["typeList"] = selList1.AsEnumerable();

            var pp = (from t in QC_DB.project
                      select new checkListModel
                      {
                          Value = t.ID.ToString(),
                          Text = t.Pname
                      }).ToList();

            ViewData["proList"] = pp;

            return PartialView("_editUser",new admin_user());
        }


        [Authorize(Roles = "admin")]
        [HttpPost]
        public int addUser(admin_user user)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();


            admin_user tmp = new admin_user();
            tmp.Username = user.Username;
            tmp.Type = user.Type;
            tmp.Permission = user.Permission;
            tmp.Name = user.Name;
            tmp.GreatDate = DateTime.Now;
            tmp.Password = "Cpic1234";

            QC_DB.admin_user.Add(tmp);
            QC_DB.SaveChanges();
            return tmp.ID;
            
        }

        
    }
}