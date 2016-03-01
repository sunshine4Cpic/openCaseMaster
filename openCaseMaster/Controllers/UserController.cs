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
        // GET: User 改动4 改动5
        public ActionResult Login(string ReturnUrl)
        {
            ViewBag.returnUrl = ReturnUrl;
            return View();
        }

        //
        // POST: /Account/Login改动6
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
                ModelState.AddModelError("", "无效的登录尝试。");
                return View(model);
            }

            //创建身份验证票据 
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                                           loginUser.Name,
                                           DateTime.Now,
                                           DateTime.Now.AddHours(2),
                                           false,
                                           "admin",//用户组暂不处理
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


        /// <summary>
        /// 跳转URL
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private ActionResult RedirectToLocal(string ReturnUrl)
        {
            if (Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
       
    }
}