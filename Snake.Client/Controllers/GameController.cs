using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Snake.Client.Models;
using Snake.Server.Models;
using System;
using System.Web.Mvc;

namespace Snake.Client.Controllers
{
    public class GameController : Controller
    {

        private UserModel _user { get { return (UserModel)Session["User"]; } }

        // GET: Game
        public ActionResult Index()
        {
            if (Session["User"] == null || string.IsNullOrEmpty(((UserModel)Session["User"]).Token))
                return RedirectToAction("Index", "Home");
            return View((UserModel)Session["User"]);
        }

        [HttpPost]
        public JsonResult Loop(LoopRequestModel model)
        {
            try
            {
                return Json(GameConnection.Loop(_user.Token, model));
            }
            catch (Exception e)
            {
                return Json(e.Message,JsonRequestBehavior.AllowGet);
            }
        }

        public string ReloadSettings()
        {
            GameConnection.ReloadSettings();
            return "Success";
        }

        public JsonResult Shoot()
        {
            try
            {
                return Json(GameConnection.Shoot(_user.Token), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(e.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetSettings()
        {
            try
            {
                return Json(GameConnection.GetSettings(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(e.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Join()
        {
            try
            {
                return Json(GameConnection.Join(_user.Token,_user.Name), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(e.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetData()
        {
            try { return Json(GameConnection.GetOnlineData(_user.Token), JsonRequestBehavior.AllowGet); }
            catch (Exception e)
            {
                return Json(e.Message, JsonRequestBehavior.AllowGet);
            }
        }
    }
}