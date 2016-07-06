using CaptchaMvc.Attributes;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using openCaseMaster.Models;
using openCaseMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;

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


            ClaimsIdentity _identity = new ClaimsIdentity("ApplicationCookie");
            _identity.AddClaim(new Claim(ClaimTypes.Name, loginUser.Name));
            _identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, loginUser.ID.ToString()));
            _identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity"));

            _identity.AddClaim(new Claim("userName", loginUser.Username));
            _identity.AddClaim(new Claim("Permission", loginUser.Permission));

            _identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
            
            //设置权限组
            switch (loginUser.Type)
            {
                case 1:
                    _identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));
                    break;
                case 5:
                    _identity.AddClaim(new Claim(ClaimTypes.Role, "tester"));
                    break;
                case null:
                    //userRole = "guest";
                    break;
                default:
                    break;
            }


            loginUser.LastDate = DateTime.Now;
            qc.SaveChanges();

            

            
            HttpContext.GetOwinContext().Authentication.SignOut("ApplicationCookie");
            HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties() { IsPersistent = true }, _identity);

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
        [CaptchaVerify("Captcha is not valid")]//验证码
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
                tmp.Avatar = "auto.jpg";

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
                newF.controlXML = new XElement("Steps").ToString();
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
            return Redirect("/");
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
            tmp.Avatar = "auto.jpg";
      

            QC_DB.admin_user.Add(tmp);
            QC_DB.SaveChanges();
            return tmp.ID;
            
        }




        [Authorize]
        [HttpGet]
        public ActionResult userInfo(string id)
        {
            userInfoModel ul;
            if (id == null)
                ul = new userInfoModel();
            else
                ul = new userInfoModel(id);
            return View(ul);

        }

       
       


        [Authorize]
        [HttpPost]
        public string ChangePassword(ChangePasswordModel req)
        {
            if (!ModelState.IsValid)
            {
                Response.Status = "400";
                return "非法提交";
            }
            QCTESTEntities QC_DB = new QCTESTEntities();

            int id = User.userID();

            admin_user user = QC_DB.admin_user.First(t => t.ID == id);

            if (user.Password != req.Password)
            {
                Response.Status = "400";
                return "旧密码错误";
            }

            user.Password = req.Password;

            QC_DB.SaveChanges();

            return true.ToString();

        }

        [Authorize]
        [HttpGet]
        public ActionResult notification(int page = 1,int rows=20)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();


            int userID = User.userID();

            var date = QC_DB.notification.Where(t => t.userID == userID);

            var query = from t in date
                        orderby t.state, t.createDate descending
                        select new notificationModel
                        {
                            ID = t.ID,
                            topicID = t.topicID == null ? t.topicReply.topic.ID : t.topicID.Value,
                            User = new topicUserModel { ID = t.admin_user.ID, Avatar = t.admin_user.Avatar, Name = t.admin_user.Name, userName = t.admin_user.Username },
                            title = t.topicID == null ? t.topicReply.topic.title : t.topic.title,
                            body = t.topicID == null ? t.topicReply.body : t.topic.body,
                            floor = t.topicID == null ? t.topicReply.floor : 0,
                            creatDate = t.createDate,
                            state = t.state
                        };

            var model = query.Skip(20 * (page - 1)).Take(20).ToList();

            foreach(var m in model)
            {
                if (m.state == 0)
                    QC_DB.notification.First(t => t.ID == m.ID).state = 1;
            }
            QC_DB.SaveChanges();

            ViewBag.page = page;

            ViewBag.page = page;
            ViewBag.rows = rows;
            ViewBag.total = date.Count();
            return View(model);
        }

        
    }
}