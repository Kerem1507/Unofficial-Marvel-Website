using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using Marvel.Controllers;
using Marvel.Models.DataContext;
using Marvel.Models.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Marvel.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Blog_Details_View()
        {
            MarvelDBContext db = new MarvelDBContext();
            var controller = new HomeController(db);
            var result = controller.Blog(2) as ViewResult;

            //HomeController homeController = new HomeController();
            //ActionResult result = homeController.Index();
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [TestMethod]
        public void Urunleri_Goruntule()
        {
            MarvelDBContext db = new MarvelDBContext();
            var controller = new HomeController(db);
            var result = controller.Hizmetlerimiz() as ViewResult;
            var model = (IEnumerable<Marvel.Models.Model.Hizmet>)result.Model;
            Assert.AreNotEqual(model.ToList().Count,0);

        }
    }
}
