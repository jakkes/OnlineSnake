using Snake.Client.Models;
using Snake.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Snake.Client.Controllers
{
    public class HomeController : Controller
    {
        // GET: Default
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["User"] != null && !string.IsNullOrEmpty(((UserModel)Session["User"]).Token))
                return RedirectToAction("Index", "Game");
            return View();
        }

        [HttpPost]
        public ActionResult Index(string Username, string Password)
        {
            var db = new UserDb();
            Session["User"] = db.LoginUser(char.ToUpper(Username[0]) + Username.Substring(1), Password);
            return RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            Session["User"] = null;
            return RedirectToAction("Index");
        }

        // GET: Default/Register
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // POST: Default/Register
        [HttpPost]
        public ActionResult Register(string Username, string Password)
        {
            var db = new UserDb();
            if (db.CreateUser(Username, Password))
                return RedirectToAction("Index");
            else return RedirectToAction("Register");
        }
    }
}