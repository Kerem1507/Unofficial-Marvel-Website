using Marvel.Models.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using Marvel.Models.Model;

namespace Marvel.Controllers
{
    public class HomeController : Controller
    {
        private MarvelDBContext db = new MarvelDBContext();

        // For tests, apply this code and delete the code in above.
        //private MarvelDBContext db;
        //public HomeController(MarvelDBContext ctx)
        //{
        //    db = ctx;
        //}

        // GET: Home
        //[Route("")]
        //[Route("Anasayfa")]
        //[Route("/Home/Index")]
        public ActionResult Index()
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            ViewBag.Hizmetler = db.Hizmet.ToList().OrderByDescending(x => x.HizmetId).Take(4);
            ViewBag.Iletisim = db.Iletisim.SingleOrDefault();
            ViewBag.Blog = db.Blog.ToList().OrderByDescending(x => x.BlogId).Take(4);
            return View();
        }

        public ActionResult SliderPartial()
        {
            return View(db.Slider.ToList().OrderByDescending(x=>x.SliderId));
        }

        public ActionResult HizmetPartial()
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            return View(db.Hizmet.ToList().OrderByDescending(x=>x.HizmetId));
        }

        public ActionResult Hakkimizda()
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            return View(db.Hakkimizda.SingleOrDefault());
        }

        public ActionResult Hizmetlerimiz()
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            return View(db.Hizmet.ToList().OrderByDescending(x => x.HizmetId));
        }

        [HttpPost]
        public ActionResult Hizmetlerimiz(string adsoyad = null, string email = null, string adres = null, string urun = null)
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            if (adsoyad != null && email != null)
            {
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.EnableSsl = true;
                WebMail.UserName = "spikecowboy123@gmail.com";
                WebMail.Password = "kerem1907";
                WebMail.SmtpPort = 587;
                WebMail.Send("spikecowboy123@gmail.com", adsoyad, email + " - " + adres + " - " + urun);
                ViewBag.Uyari = "Formunuz başarıyla gönderildi!";
            }
            else
            {
                ViewBag.Uyari = "Hata oluştu. Tekrar deneyiniz!";
            }
            return View(db.Hizmet.ToList().OrderByDescending(x => x.HizmetId));
        }

        public ActionResult Iletisim()
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            return View(db.Iletisim.SingleOrDefault());
        }

        [HttpPost]
        public ActionResult Iletisim(string adsoyad=null, string email=null, string konu=null, string mesaj=null)
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            if (adsoyad != null && email != null)
            {
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.EnableSsl = true;
                WebMail.UserName = "spikecowboy123@gmail.com";
                WebMail.Password = "kerem1907";
                WebMail.SmtpPort = 587;
                WebMail.Send("spikecowboy123@gmail.com", konu, email + " - " + mesaj);
                ViewBag.Uyari = "Formunuz başarıyla gönderildi!";
            }
            else
            {
                ViewBag.Uyari = "Hata oluştu. Tekrar deneyiniz!";
            }

            return View();
        }

        public ActionResult Blog(int Sayfa = 1)
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            return View(db.Blog.Include("Kategori").OrderByDescending(x => x.BlogId).ToPagedList(Sayfa, 5));
        }
        public ActionResult KategoriBlog(int id, int Sayfa=1)
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            var b = db.Blog.Include("Kategori").OrderByDescending(x=>x.BlogId).Where(x => x.Kategori.KategoriId == id).ToPagedList(Sayfa,5);
            return View(b);
        }

        public ActionResult BlogDetay(int id)
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            var b = db.Blog.Include("Kategori").Include("Yorums").Where(x => x.BlogId == id).SingleOrDefault();
            return View(b);
        }

        public JsonResult YorumYap(string adsoyad, string eposta, string icerik, int blogid)
        {
            if (icerik == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            db.Yorum.Add(new Yorum { AdSoyad = adsoyad, Eposta = eposta, Icerik = icerik, BlogId = blogid, Onay = false });
            db.SaveChanges();

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BlogKategoriPartial()
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            //db.Configuration.LazyLoadingEnabled = false;
            return PartialView(db.Kategori.Include("Blogs").ToList().OrderBy(x => x.KategoriAd));
        }
    }
}