using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using kidyx.com.Models;
using Microsoft.AspNet.Identity;
using PagedList;
using PagedList.Mvc;
using Stripe;

namespace kidyx.com.Controllers
{

    public class ProductsController : Controller
    {
         Entities1 db = new Entities1();
        // GET: Products
        
        public ActionResult ViewProductDetails(int? id)

        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var pro = db.Product.Find(id);
var u_id = User.Identity.GetUserId();
            if (u_id != null)
            {

                var so = db.shoporder.Where(x => x.status == "Delivered" && x.userid == u_id && x.reviewstatus == 0);
                if (so != null)
                {
                    Session["openclose"] = 0;
                    foreach (var item in so)
                    {
                        int a = 0;
                        foreach (var item1 in item.productlist)
                        {
                            if (item1.product_id == id && item.status == "Delivered")
                            {
                                Session["oderid"]=item.Id;
                                Session["openclose"] = 1;
                                a = 1;
                                break;
                            }

                        }
                        if (a == 1) { break; }

                    }
                }
            }
            if (pro == null)
            {
                return HttpNotFound();
            }
            var star1 = db.review.Where(x=>x.product_id==id && x.rating==1) ;
            var one = star1.Count();
            var star2 = db.review.Where(x => x.product_id == id && x.rating == 2);
            var two = star2.Count();
            var star3 = db.review.Where(x => x.product_id == id && x.rating == 3);
            var three = star3.Count();
            var star4 = db.review.Where(x => x.product_id == id && x.rating == 4);
            var four = star4.Count();
            var star5 = db.review.Where(x => x.product_id == id && x.rating == 5);
            var five = star5.Count();
            var result = 0;
           if ( five + four + three + two + one!=0) { 
            result = (5 * five + 4 * four + 3 * three + 2 * two + 1 * one) / (five + four + three + two + one);}
            ViewBag.tstar = result;
            //finding catagory with catagory id
            var catname = db.catagory.Find(pro.P_cid);
            ViewBag.catname = catname.name;
            int c = Convert.ToInt32(Session["p_id"]);
            var z = db.review.Where(x => x.product_id == c);
            Session["count"] = z.Count();
            return View(pro);
        }

        public ActionResult checkout(int id)
        {
            ViewBag.check = id;
             ViewBag.StripePublishKey = ConfigurationManager.AppSettings["stripePublishableKey"];
            if (User.Identity.GetUserId() != null)
            {
                var im = db.UserProfile_details.Find(User.Identity.GetUserId());
                ordermanagement om = new ordermanagement();
                om.user_pname = im.user_pname;
                om.user_fname = im.user_fname;
                om.zipcode = im.zipcode;
                om.email = User.Identity.GetUserName();
                om.address1 = im.address1;
                om.phoneno = im.phoneno;
                return View(om);
            }

            return View();
        }
        [HttpPost]
        public ActionResult checkout(string stripeToken, string stripeEmail , ordermanagement om)
        {
            if (ModelState.IsValid) {
                if (stripeEmail != null)
                {
                    if (Session["cart"] != null)
                    {
                        Stripe.StripeConfiguration.SetApiKey("pk_test_51IvlSfGyW3TAVeDAqAmrMiVeHdETsCucFMD2O20k5wugpRTfs1PK9FZi68tv7zufgeeTudi7aTm276bzBtlzjQLZ00wLyHr6aU");
                        Stripe.StripeConfiguration.ApiKey = "sk_test_51IvlSfGyW3TAVeDArjC8TIn3QR5qRjuYmbO78YX7jJgYFhA3wasIMDnbHm0SuYlFzs6IIVF1iHpfyySvv6Ks1Xr3004ajqeIv0";

                        var myCharge = new Stripe.ChargeCreateOptions();
                        // always set these properties
                        myCharge.Amount = Convert.ToInt32(Session["Stripetotal"]);
                        myCharge.Currency = "pkr";
                        myCharge.ReceiptEmail = stripeEmail;
                        myCharge.Description = "Sample Charge";
                        myCharge.Source = stripeToken;
                        myCharge.Capture = true;
                        var chargeService = new Stripe.ChargeService();
                        Charge stripeCharge = chargeService.Create(myCharge);
                        var bill = Guid.NewGuid() + "";
                        foreach (cart item in (List<cart>)Session["cart"])
                        {

                            shoporder so = new shoporder();
                            so.Id = "Order" + Guid.NewGuid();
                            so.name = om.user_pname;
                            so.fname = om.user_fname;
                            so.email = om.email;
                            so.address = om.address1;
                            so.phone = om.phoneno;
                            so.zipcode = om.zipcode;
                            so.amount_status = "paid";
                            so.datetime = DateTime.Now;
                            so.status = "Pending";
                            so.reviewstatus = 0;
                            if (User.Identity.GetUserId() != null) { so.userid = User.Identity.GetUserId(); }
                            so.bill_id = bill;
                            so.total = item.productprice * item.qty;
                            productlist pl = new productlist();
                            pl.product_id = item.productid;
                            pl.quantity = item.qty;
                            pl.so_id = so.Id;
                            pl.productweight = null;
                            pl.productprice = item.productprice;
                            db.shoporder.Add(so);
                            db.SaveChanges();
                            db.productlist.Add(pl);
                            db.SaveChanges();
                            var pt = db.Product.Find(item.productid);
                            pt.P_quantity = pl.quantity - pt.P_quantity;
                            db.Entry(pt).State = EntityState.Modified;
                            db.SaveChanges();






                        }
                        Session.Remove("cart");
                        Session["c"] = 0;
                        return View();
                    }
                }
                else
                {
                    if (Session["cart"] != null)
                    {
                       
                        var bill = Guid.NewGuid() + "";
                        foreach (cart item in (List<cart>)Session["cart"])
                        {

                            shoporder so = new shoporder();
                            so.Id = "Order" + Guid.NewGuid();
                            so.name = om.user_pname;
                            so.fname = om.user_fname;
                            so.email = om.email;
                            so.address = om.address1;
                            so.phone = om.phoneno;
                            so.zipcode = om.zipcode;
                            so.amount_status = "Cod";
                            so.datetime = DateTime.Now;
                            so.status = "Pending";
                            so.reviewstatus = 0;
                            if (User.Identity.GetUserId() != null) { so.userid = User.Identity.GetUserId(); }
                            so.bill_id = bill;
                            so.total = item.productprice * item.qty;
                            productlist pl = new productlist();
                            pl.product_id = item.productid;
                            pl.quantity = item.qty;
                            pl.so_id = so.Id;
                            pl.productweight = null;
                            pl.productprice = item.productprice;
                            db.shoporder.Add(so);
                            db.SaveChanges();
                            db.productlist.Add(pl);
                            db.SaveChanges();
                            var pt = db.Product.Find(item.productid);
                            pt.P_quantity =pt.P_quantity - pl.quantity  ;
                            db.Entry(pt).State = EntityState.Modified;
                            db.SaveChanges();





                        }
                        Session.Remove("cart");
                        Session["c"] = 0;
                        return View();
                    }
                }
               
            }
            return View();
        }
        
        public ActionResult orderlist(string search)
        {
            if (search == null) { 
            var a = db.shoporder.OrderByDescending(p => p.sort).ToList(); 
            return View(a);
            }
            else if (search == "Pending")
            {
                var s = db.shoporder.Where(p => p.status == "Pending");
                var c = s.OrderByDescending(p => p.sort).ToList();
                 
                return View(c);
            }
            else if (search == "Confirmed")
            {
                var s = db.shoporder.Where(p => p.status == "Confirmed");
                var c = s.OrderByDescending(p => p.sort).ToList();

                return View(c);
            }
            else if (search == "Shipping")
            {
                var s = db.shoporder.Where(p => p.status == "Shipping");
                var c = s.OrderByDescending(p => p.sort).ToList();

                return View(c);
            }
            else if (search == "Delivered")
            {
                var s = db.shoporder.Where(p => p.status == "Delivered");
                var c = s.OrderByDescending(p => p.sort).ToList();

                return View(c);
            }
            else if (search == "Canceled")
            {
                var s = db.shoporder.Where(p => p.status == "Canceled");
                var c = s.OrderByDescending(p => p.sort).ToList();

                return View(c);
            }

            return View();
        }
        [HttpGet]
        public ActionResult orderdetails(string id)
        {
            var so = db.shoporder.Where(p => p.bill_id == id);
            return View(so);
        }







        
        public ActionResult conformorder(string id)
        {
            var so = db.shoporder.Find(id);
            var bill = so.bill_id;
            so.status = "Confirmed";
            db.Entry(so).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction(Url.Content("~/orderdetails/"+bill));
        }
        
        public ActionResult shiporder(string id)
        {
            var so = db.shoporder.Find(id);
            so.status = "Shipping";
            var bill = so.bill_id;
            db.Entry(so).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction(Url.Content("~/orderdetails/" + bill));
        }
        
        public ActionResult deliverorder(string id)
        {
            var so = db.shoporder.Find(id);
            var bill = so.bill_id;
            so.status = "Delivered";
            db.Entry(so).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction(Url.Content("~/orderdetails/" + bill));
        }
        
        public ActionResult cancelorder(string id)
        {
            var so = db.shoporder.Find(id);
            var bill = so.bill_id;
            so.status = "Canceled";
            db.Entry(so).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction(Url.Content("~/orderdetails/" + bill));
        }
        [HttpPost]
        public ActionResult Review(string message, int rt)
        {
            review r = new review();
            r.user_id = User.Identity.GetUserId();
            r.message = message;
            r.rating = rt;
            r.date = DateTime.Now;
            int a = Convert.ToInt32(Session["p_id"]);
            r.product_id = a;

            db.review.Add(r);
            db.SaveChanges();
            var o_id = Session["oderid"];
            var so=db.shoporder.Find(o_id);
            so.reviewstatus = 1;
            db.Entry(so).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ViewProductDetails", "Products",new{ id = a });

        }

        [ChildActionOnly]
        public ActionResult DetailReview()
        { int c =Convert.ToInt32( Session["idr"]);
            var a = db.review.Where(x=>x.product_id==c);
            Session["count"] = a.Count();
            return PartialView("_DetailReview", a);
        }

        public ActionResult Shop(string seacrhing, int? cat_id, int? page, int? minprices, int? maxprices)
        {

            var max = (from p in db.Product select p.P_discprice).Max();
            Session["max"] = max;
            var min = (from p in db.Product select p.P_discprice).Min();
           
            Session["min"] = min;
            Session["c"] = "";
            var abc = from x in db.Product select x;


             
           var random = abc.OrderByDescending(s=>Guid.NewGuid()).Take(10);
            if (seacrhing != null && cat_id == null)
            {
                
                random = random.Where(x => x.P_name.Contains(seacrhing)
                || x.catagory.name.Contains(seacrhing));
            }
    
            if (cat_id != null && seacrhing == null)
            {
                random = abc;
                random = random.Where(x => x.P_cid == cat_id);
            }
            if (cat_id != null && seacrhing != null)
            {
                random = abc;
                random = random.Where(x => x.P_cid == cat_id && x.P_name.Contains(seacrhing));
            }
            if (minprices != null && maxprices != null && cat_id == null)
            {
                random = abc;
                random = random.Where(x => x.P_discprice >= minprices && x.P_discprice <= maxprices);
            }
            if (minprices != null && maxprices != null && cat_id != null)
            {
                random = abc;
                random = random.Where(x => x.P_discprice >= minprices && x.P_discprice <= maxprices && x.P_cid == cat_id);
            }
             
            Session["c"] = cat_id;

            return View(random.ToList().ToPagedList(page ?? 1, 3));
        }
        [ChildActionOnly]
        public ActionResult prolist()
        {
            var a = db.catagory.ToList();
            return PartialView("prolist", a);
        }



    }
}