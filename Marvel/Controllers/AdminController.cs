using Marvel.Models;
using Marvel.Models.DataContext;
using Marvel.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Marvel.Controllers
{
    public class AdminController : Controller
    {
        MarvelDBContext db = new MarvelDBContext();
        // GET: Admin
        public ActionResult Index()
        {
            ViewBag.UrunSay = db.Hizmet.Count();
            ViewBag.HaberSay = db.Blog.Count();
            ViewBag.YorumSay = db.Yorum.Where(x=>x.Onay == true).Count();
            ViewBag.KategoriSay = db.Kategori.Count();
            ViewBag.YorumOnay = db.Yorum.Where(x => x.Onay == false).Count();
            var sorgu = db.Kategori.ToList();
            return View(sorgu);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Admin admin)
        {
            var login = db.Admin.Where(x => x.Eposta == admin.Eposta).SingleOrDefault();
            if (login.Eposta==admin.Eposta && login.Sifre==Crypto.Hash(admin.Sifre, "MD5"))
            {
                Session["adminid"] = login.AdminId;
                Session["eposta"] = login.Eposta;
                Session["yetki"] = login.Yetki;
                return RedirectToAction("Index", "Admin");
            }
            ViewBag.Uyari = "Girdiğiniz Eposta ya da Şifre Hatalı!";
            return View(admin);
        }

        //[HttpPost]
        //public ActionResult Login(Admin admin)
        //{
        //    //var md5pass = Crypto.Hash(sifre, "MD5");
        //    var login = db.Admin.Where(x => x.Eposta == admin.Eposta && x.Sifre == admin.Sifre).SingleOrDefault();
        //    if (login != null)
        //    {
        //        if (login.Eposta == admin.Eposta && login.Sifre ==  admin.Sifre/*Crypto.Hash(admin.Sifre, "MD5")*/)  
        //        {
        //            Session["adminid"] = login.AdminId;
        //            Session["eposta"] = login.Eposta;
        //            Session["yetki"] = login.Yetki;
        //            return RedirectToAction("Index", "Admin");
        //        }
        //    }

        //    ViewBag.Uyari = "Girdiğiniz Eposta ya da Şifre Hatalı!";
        //    return View(admin);

        //}

        public ActionResult Logout()
        {
            Session["adminid"] = null;
            Session["eposta"] = null;
            Session.Abandon();

            return RedirectToAction("Login", "Admin");
        }

        public ActionResult SifremiUnuttum()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SifremiUnuttum(string eposta)
        {
            var mail = db.Admin.Where(x => x.Eposta == eposta).SingleOrDefault();
            if (mail != null)
            {
                Random rnd = new Random();
                int yenisifre = rnd.Next();

                Admin admin = new Admin();
                mail.Sifre = Crypto.Hash(Convert.ToString(yenisifre), "MD5");
                db.SaveChanges();

                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.EnableSsl = true;
                WebMail.UserName = "spikecowboy123@gmail.com";
                WebMail.Password = "kerem1907";
                WebMail.SmtpPort = 587;
                WebMail.Send(eposta, "Marvel Admin Giriş Şifreniz", "Şifreniz: " + yenisifre);
                ViewBag.Uyari = "Yeni Şifreniz başarıyla gönderildi!";
            }
            else
            {
                ViewBag.Uyari = "Hata oluştu. Tekrar deneyiniz!";
            }

            return View();
        }

        public ActionResult Adminler()
        {
            return View(db.Admin.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Admin admin, string sifre, string eposta)
        {
            if (ModelState.IsValid)
            {
                admin.Sifre = Crypto.Hash(sifre, "MD5");
                db.Admin.Add(admin);
                db.SaveChanges();
                return RedirectToAction("Adminler");
            }

            return View(admin);
        }

        public ActionResult Edit(int id)
        {
            var a = db.Admin.Where(x => x.AdminId == id).SingleOrDefault();
            return View(a);
        }
        [HttpPost]
        public ActionResult Edit(int id, Admin admin, string sifre, string eposta)
        {
            if (ModelState.IsValid)
            {
                var a = db.Admin.Where(x => x.AdminId == id).SingleOrDefault();
                a.Sifre = Crypto.Hash(sifre, "MD5");
                a.Eposta = admin.Eposta;
                a.Yetki = admin.Yetki;
                db.SaveChanges();
                return RedirectToAction("Adminler");
            }
            
            return View(admin);
        }

        public ActionResult Delete(int id)
        {
            var a = db.Admin.Where(x => x.AdminId == id).SingleOrDefault();
            if (a!=null)
            {
                db.Admin.Remove(a);
                db.SaveChanges();
                return RedirectToAction("Adminler");
            }
            return View();
        }
    }
}