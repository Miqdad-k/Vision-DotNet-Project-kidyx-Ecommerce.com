using kidyx.com.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace kidyx.com.Controllers
{
    public class HomeController : Controller
    {
        //database connection
        Entities1 db = new Entities1();
        //front page to website
        
        public ActionResult Index()
        {
            //condition is used to see user is login or not
            //if user is login check profile if filled
            //if not redirect to profile details
            if (User.Identity.GetUserId() != null)
            {
                var im = db.UserProfile_details.Find(User.Identity.GetUserId());
                if (im == null)
                {
                    return RedirectToAction("Profiledetails_Create");
                }
                else
                {
                    var b = User.Identity.GetUserId();
                     var a= db.Wishlist.Where(x => x.userid ==b );
                    Session["wishlist"] = a.ToList();
                    Session["wishlistcount"] = a.Count();
                    return View();
                }
            }
            else { return View(); }

        }
       
        //this is used to show name and image in nav corner
        [ChildActionOnly]
        public ActionResult headerpartial()
        {
            var pd = db.pagedetails.Find(1);
            Session["pEmail"] = pd.Email;
            Session["pAddress"] = pd.Address;
            Session["pPhonenumber"] = pd.Phonenumber;
            Session["pFacebook"] = pd.Facebook;
            Session["pTwitter"] = pd.Twitter;
            Session["pInstagram"] = pd.Instagram;
            Session["pPrintest"] = pd.Printest;
            
            
            var c = db.UserProfile_details.ToList();
            //_headerpartial1 is view name shared folder
            return PartialView("_headerpartial1", c);
        }
       
        //this used to check profiledetails to create or edit button
        [ChildActionOnly]
        public ActionResult profileCEpartial()
        {

            var im = db.UserProfile_details.Find(User.Identity.GetUserId());

            if (im != null)
            {
                var ab = User.Identity.GetUserId();
                ViewBag.roles = ab;
            }
            else
            {
                ViewBag.roles = null;
            }
            return PartialView("_profileCEpartial", im);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [Authorize]
        public ActionResult Profiledetails_Create()
        {
            return View();
        }
        // //file in used to get picturefile
        [HttpPost]
        public ActionResult Profiledetails_Create(HttpPostedFileBase file, UserProfile_details u)
        {

            if (ModelState.IsValid)
            {
                //file path will save in database we adding datetime start of picture so duplicate name problem will not come
                string filename = Path.GetFileName(file.FileName);
                string _filename = DateTime.Now.ToString("yymmssfff") + filename;
                string extension = Path.GetExtension(file.FileName);
                string path = Path.Combine(Server.MapPath("~/images/"), _filename);
                u.profile_img = "~/images/" + _filename;

                if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                {
                    if (file.ContentLength <= 1000000)
                    {

                        u.user_id1 = User.Identity.GetUserId();
                        db.UserProfile_details.Add(u);


                        if (db.SaveChanges() > 0)
                        {
                            file.SaveAs(path);
                            ViewBag.msg = "Record Added";
                            ModelState.Clear();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ViewBag.msg = "file must be Equal  or less then 1mb";
                        }
                    }
                    else
                    {
                        ViewBag.msg = "Invalid file Type";
                    }
                }
            }
            return View(u);



        }
        //session is used to save file path for if user don't change image
        public ActionResult Profiledetails_Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var im = db.UserProfile_details.Find(id);


            if (im == null)
            {
                return HttpNotFound();
            }
            Session["imgPath"] = im.profile_img;
            return View(im);
        }
        [HttpPost]
        public ActionResult Profiledetails_Edit(HttpPostedFileBase file, UserProfile_details u)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    //file path will save in database we adding datetime start of picture so duplicate name problem will not come
                    string filename = Path.GetFileName(file.FileName);
                    string _filename = DateTime.Now.ToString("yymmssfff") + filename;
                    string extension = Path.GetExtension(file.FileName);
                    string path = Path.Combine(Server.MapPath("~/images/"), _filename);
                    u.profile_img = "~/images/" + _filename;

                    if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                    {
                        if (file.ContentLength <= 1000000)
                        {
                            db.Entry(u).State = EntityState.Modified;
                            string oldimgapth = Request.MapPath(Session["imgPath"].ToString());
                            if (db.SaveChanges() > 0)
                            {
                                file.SaveAs(path);
                                //deleting the old image
                                if (System.IO.File.Exists(oldimgapth))
                                {
                                    System.IO.File.Delete(oldimgapth);
                                }
                                TempData["msg"] = "data Updated";
                                return RedirectToAction("Index");
                            }
                        }

                    }
                }
                else
                {
                    //saving old image path
                    u.profile_img = Session["imgPath"].ToString();
                    db.Entry(u).State = EntityState.Modified;
                    if (db.SaveChanges() > 0)
                    {
                        TempData["msg"] = "data Updated";
                        return RedirectToAction("Index");
                    }

                }
            }
            return View();

        }

        public ActionResult addtocart(Product p, string unit, int? id)
        {
            var pd = db.Product.Find(id);
            if (Session["cart"] == null)
            {
                List<cart> item = new List<cart>();
                item.Add(new cart()
                {
                    productid = pd.P_id,
                    productname = pd.P_name,
                    productpic = pd.P_img1,
                    productprice = Convert.ToInt32(pd.P_discprice),
                    qty = Convert.ToInt32(unit),

                });
                Session["c"] = item.Count();
                Session["cart"] = item;
            }
            else
            {
                List<cart> li2 = Session["cart"] as List<cart>;
                int flag = 0;
                foreach (var item in li2)
                {
                    if (item.productid == pd.P_id)
                    {
                        cart c = new cart();
                        item.qty += Convert.ToInt32(unit);
                        flag = 1;

                    }

                }

                if (flag == 0)
                {
                    List<cart> item = (List<cart>)Session["cart"];
                    item.Add(new cart()
                    {
                        productid = pd.P_id,
                        productname = pd.P_name,
                        productpic =pd.P_img1,
                        productprice=Convert.ToInt32( pd.P_discprice),
                        qty = Convert.ToInt32(unit),
                    });
                    
                    Session["cart"] = item;
                    Session["c"] = item.Count();
                }
            }
            return RedirectToAction(Url.Content("../Products/ViewProductDetails/"+id));


        }
        public ActionResult RemoveFromCart(int productId)
        {
            List<cart> cart = (List<cart>)Session["cart"];
            foreach (var item in cart)
            {
                if (item.productid == productId)
                {
                    cart.Remove(item);
                    break;
                }
            }
            
            Session["cart"] = cart;
            Session["c"] = cart.Count();
            return RedirectToAction(Url.Content("../Products/ViewProductDetails/" + productId));
        }
        
        [ChildActionOnly]
        public ActionResult fsectionlist()
        {
            var c = db.fsection.ToList();
            //_headerpartial1 is view name shared folder
            return PartialView("_fsectionlist",c);
        }
  
        [ChildActionOnly]
        public ActionResult tsectionlist()
        {
            var c = db.tsection.ToList();
            //_headerpartial1 is view name shared folder
            return PartialView("_tsectionlist", c);
        }
   
        [ChildActionOnly]
        public ActionResult thsectionlist()
        {   var ths = db.thsection.Find(1);
            ViewBag.role4 = ths.catagoryid;
            ViewBag.role5 = ths.catagory.name.ToUpper();
            ViewBag.role1 = ths.picture;
            ViewBag.role2 = ths.name;
            ViewBag.role3 = ths.link;
            var r = new Random();
            var c = db.Product.ToList();
            var b = c.Where(x => x.P_cid == ths.catagoryid);
            var a= b.OrderByDescending(u => r.Next()  ).Take(5);
            
            
           
            return PartialView("_thsectionlist", a);
        }
        
        [ChildActionOnly]
        public ActionResult fisectionlist()
        {
            var r = new Random();
            var c = db.Product.ToList();
            var a = c.OrderByDescending(u => r.Next()).Take(5);
            var ths = db.fisection.Find(1);
            ViewBag.role4 = ths.catagory.name;
            ViewBag.role5 = ths.catagory.name.ToUpper();
            ViewBag.role1 = ths.picture;
            ViewBag.role2 = ths.name;
            ViewBag.role3 = ths.link;
            return PartialView("_fisectionlist", a);
        }
        public ActionResult shippinglable(string id)
        {
            var so = db.shoporder.Where(x=>x.bill_id == id);
            return View(so);
        }
        public ActionResult GeneratePDF(string id)
        {
            return new Rotativa.ActionAsPdf(Url.Content("~/shippinglable/"+id));
        }
  
        [ChildActionOnly]
        public ActionResult fosectionlist()
        {
          var a=  db.fosection.ToList();
            return PartialView("_fosectionlist", a);
        }

        public ActionResult Wishlist(int id  )
        {
            var pd = db.Product.Find(id);
            Wishlist wl = new Wishlist();
            wl.productid = id;
            wl.productname = pd.P_name;
            wl.productprice =pd.P_discprice;
            wl.productpic = pd.P_img1;
            wl.userid = User.Identity.GetUserId();
            db.Wishlist.Add(wl);
            db.SaveChanges();
            var b = User.Identity.GetUserId();
            var a = db.Wishlist.Where(x => x.userid == b);
            Session["wishlist"] = a.ToList();
            Session["wishlistcount"] = a.Count();
            return RedirectToAction(Url.Content("../Products/ViewProductDetails/" + id));
        }
        public ActionResult Wishlistremove(int id)
        {
            var a = db.Wishlist.Find(id);
            var rid = a.productid;
            
            db.Entry(a).State = EntityState.Deleted;
            db.SaveChanges();
            var b = User.Identity.GetUserId();
            var s = db.Wishlist.Where(x => x.userid == b);
            Session["wishlist"] = s.ToList();
            Session["wishlistcount"] = s.Count();
            return RedirectToAction(Url.Content("../Products/ViewProductDetails/" + rid));
        }
        public ActionResult Shoppingcart()
        {
            return View();
        }
        public ActionResult ContactUs()
        {
            var im = db.contactus.Find(1);
            return View(im);
        }


        public ActionResult viewProfile(string id)
        {
            var abc = db.UserProfile_details.Find(id);
            return View(abc);
        }
    }
}