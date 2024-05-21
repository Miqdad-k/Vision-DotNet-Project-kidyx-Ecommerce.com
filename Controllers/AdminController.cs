using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using kidyx.com.Models;
using Microsoft.AspNet.Identity;

namespace kidyx.com.Controllers
{        
    [Authorize]
    public class AdminController : Controller

    {
        // GET: Admin
        //database connection
       
        Entities1 db = new Entities1();
        [OutputCache(Duration = 30)]
        [Authorize(Roles = "Admin,Author,Editor,Manager,Page_Controller")]
        public ActionResult Index()
        { 
            Session["Pending Order"] = db.shoporder.Count(p => p.status == "Pending");
            return View();
        }

        //catlist is used to show main catagory for sub catagory
        //if you are entering main catagory don't select
        [Authorize(Roles = "Admin,Author,Editor")]
        public ActionResult CreateCatagory()
        {
            //getting list and saving in c for dropdown
            var c = db.catagory.ToList();
            List<SelectListItem> catlist = new List<SelectListItem>();
            foreach (var item in c)
            {
                catlist.Add(new SelectListItem { Text = item.name, Value = item.c_id.ToString() });
            }
            ViewBag.roles = catlist;

            return View();
        }
        [HttpPost]
        public ActionResult CreateCatagory(catagory c)
        {

            if (ModelState.IsValid)
            {

                db.catagory.Add(c);
                db.SaveChanges();
                return RedirectToAction("ListCatagory");
            }
            return View(c);
        }
        // to show the list of catagory
        [OutputCache(Duration = 20)]
        [Authorize(Roles = "Admin,Author,Editor")]
        public ActionResult ListCatagory()
        {
            var c = db.catagory.ToList();
            return View(c);
        }
        //this is used to show name and image in nav corner
        [ChildActionOnly]
        public ActionResult profilepartial()
        {
            var c = db.UserProfile_details.ToList();
            //_profilepartial is view name shared folder
            return PartialView("_profilepartial", c);
        }
        // to delete the catagory
        [Authorize(Roles = "Admin,Author,Editor")]
        public ActionResult deleteCatagory(int? Id)
        {


            try
            {
                var d = db.catagory.Where(x => x.c_id == Id).SingleOrDefault();
                db.catagory.Remove(d);
                db.SaveChanges();
                return RedirectToAction("ListCatagory");
            }
            catch (Exception )
            {
                /* return View(e)*/
                
                return RedirectToAction("ListCatagory");
            }


        }
        //catlist is used to show main catagory for sub catagory
        [Authorize(Roles = "Admin,Author,Editor")]
        public ActionResult editCatagory(int? Id)
        {
            var cat = db.catagory.Find(Id);
            //getting list and saving in c for dropdown
            var c = db.catagory.ToList();
            List<SelectListItem> catlist = new List<SelectListItem>();
            foreach (var item in c)
            {
                catlist.Add(new SelectListItem { Text = item.name, Value = item.c_id.ToString() });
            }
            ViewBag.roles = catlist;
            return View(cat);
        }

        [HttpPost]
        public ActionResult editCatagory(int? Id, catagory c)
        {
            if (ModelState.IsValid)
            {

                db.Entry(c).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("ListCatagory");
            }
            return View(c);
        }
        //catlist is used to show  catagory in product
        [Authorize(Roles = "Admin,Author,Editor")]
        public ActionResult createproduct()
        {
            //getting list and saving in c for dropdown
            var c = db.catagory.ToList();
            List<SelectListItem> catlist = new List<SelectListItem>();
            foreach (var item in c)
            {
                catlist.Add(new SelectListItem { Text = item.name, Value = item.c_id.ToString() });
            }
            ViewBag.roles = catlist;


            return View();
        }
        //file in used to get picturefile
        [HttpPost]
        public ActionResult createproduct(IEnumerable<HttpPostedFileBase> file, Product p)
        {

            p.P_cid = Convert.ToInt32(p.P_c_id);

            if (ModelState.IsValid)
            {
                if (file.Count() > 0)
                {
                    string uplode = "";
                    foreach (var item in file)
                    {
                        //file path will save in database we adding datetime start of picture so duplicate name problem will not come
                        string filname = Guid.NewGuid() + Path.GetExtension(item.FileName);
                        string _filename = DateTime.Now.ToString("mmddyy") + filname;
                        string filepath = "/images/" + filname;
                        item.SaveAs(Path.Combine(Server.MapPath("~/images"), filname));
                        uplode += filepath + ":";

                    }
                    //array is used to for files upto 5 file can upload
                    string[] patharray = uplode.Split(':');
                    if (patharray.Length == 2)
                    {
                        p.P_img1 = patharray[0].ToString();
                        p.P_img2 = "";
                        p.P_img3 = "";
                        p.P_img4 = "";
                        p.P_img5 = "";
                    }

                    else if (patharray.Length == 3)
                    {
                        p.P_img1 = patharray[0].ToString();
                        p.P_img2 = patharray[1].ToString();
                        p.P_img3 = "";
                        p.P_img4 = "";
                        p.P_img5 = "";
                    }
                    else if (patharray.Length == 4)
                    {
                        p.P_img1 = patharray[0].ToString();
                        p.P_img2 = patharray[1].ToString();
                        p.P_img3 = patharray[2].ToString();
                        p.P_img4 = "";
                        p.P_img5 = "";
                    }
                    else if (patharray.Length == 5)
                    {
                        p.P_img1 = patharray[0].ToString();
                        p.P_img2 = patharray[1].ToString();
                        p.P_img3 = patharray[2].ToString();
                        p.P_img4 = patharray[3].ToString();
                        p.P_img5 = "";
                    }
                    else
                    {
                        p.P_img1 = patharray[0].ToString();
                        p.P_img2 = patharray[1].ToString();
                        p.P_img3 = patharray[2].ToString();
                        p.P_img4 = patharray[3].ToString();
                        p.P_img5 = patharray[4].ToString();
                    }
                    db.Product.Add(p);
                    db.SaveChanges();
                    return RedirectToAction("ViewProduct");
                }

            }

            return View(p);
        }
        //catlist is used to show  catagory in product
        [Authorize(Roles = "Admin,Author,Editor")]
        public ActionResult EditProduct(int? id)
        {
            //catlist is used to show  catagory in product
            var c = db.catagory.ToList();
            List<SelectListItem> catlist = new List<SelectListItem>();
            foreach (var item in c)
            {
                catlist.Add(new SelectListItem { Text = item.name, Value = item.c_id.ToString() });
            }
            ViewBag.pcat = catlist;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //saving picture path in section so if new picture not add
            var pro = db.Product.Find(id);
            Session["imgPath1"] = pro.P_img1;
            Session["imgPath2"] = pro.P_img2;
            Session["imgPath3"] = pro.P_img3;
            Session["imgPath4"] = pro.P_img4;
            Session["imgPath5"] = pro.P_img5;
            pro.P_cid = Convert.ToInt32(pro.P_c_id);
            if (pro == null)
            {
                return HttpNotFound();
            }
            return View(pro);
        }
        //file in used to get picturefile
        [HttpPost]
        public ActionResult EditProduct(IEnumerable<HttpPostedFileBase> file, Product p)
        {

            p.P_cid = Convert.ToInt32(p.P_c_id);

            if (ModelState.IsValid)
            {
                if (file.Count() > 1)
                {
                    //file path will save in database we adding datetime start of picture so duplicate name problem will not come
                    string uplode = "";
                    foreach (var item in file)
                    {
                        string filname = Guid.NewGuid() + Path.GetExtension(item.FileName);
                        string _filename = DateTime.Now.ToString("mmddyy") + filname;
                        string filepath = "/images/" + filname;
                        item.SaveAs(Path.Combine(Server.MapPath("~/images"), filname));
                        uplode += filepath + ":";
                    }
                    //array is used to for files upto 5 file can upload
                    string[] patharray = uplode.Split(':');

                    if (patharray.Length == 2)
                    {
                        p.P_img1 = patharray[0].ToString();
                        p.P_img2 = "";
                        p.P_img3 = "";
                        p.P_img4 = "";
                        p.P_img5 = "";
                    }

                    else if (patharray.Length == 3)
                    {
                        p.P_img1 = patharray[0].ToString();
                        p.P_img2 = patharray[1].ToString();
                        p.P_img3 = "";
                        p.P_img4 = "";
                        p.P_img5 = "";
                    }
                    else if (patharray.Length == 4)
                    {
                        p.P_img1 = patharray[0].ToString();
                        p.P_img2 = patharray[1].ToString();
                        p.P_img3 = patharray[2].ToString();
                        p.P_img4 = "";
                        p.P_img5 = "";
                    }
                    else if (patharray.Length == 5)
                    {
                        p.P_img1 = patharray[0].ToString();
                        p.P_img2 = patharray[1].ToString();
                        p.P_img3 = patharray[2].ToString();
                        p.P_img4 = patharray[3].ToString();
                        p.P_img5 = "";
                    }
                    else
                    {
                        p.P_img1 = patharray[0].ToString();
                        p.P_img2 = patharray[1].ToString();
                        p.P_img3 = patharray[2].ToString();
                        p.P_img4 = patharray[3].ToString();
                        p.P_img5 = patharray[4].ToString();
                    }

                    db.Entry(p).State = EntityState.Modified;

                    string oldimgapth1 = Request.MapPath(Session["imgPath1"].ToString());
                    string oldimgapth2 = Request.MapPath(Session["imgPath2"].ToString());
                    string oldimgapth3 = Request.MapPath(Session["imgPath3"].ToString());
                    string oldimgapth4 = Request.MapPath(Session["imgPath4"].ToString());
                    string oldimgapth5 = Request.MapPath(Session["imgPath5"].ToString());

                    if (db.SaveChanges() > 0)
                    {

                        if (System.IO.File.Exists(oldimgapth1))
                        {
                            System.IO.File.Delete(oldimgapth1);
                        }
                        if (System.IO.File.Exists(oldimgapth2))
                        {
                            System.IO.File.Delete(oldimgapth2);
                        }
                        if (System.IO.File.Exists(oldimgapth3))
                        {
                            System.IO.File.Delete(oldimgapth3);
                        }
                        if (System.IO.File.Exists(oldimgapth4))
                        {
                            System.IO.File.Delete(oldimgapth4);
                        }
                        if (System.IO.File.Exists(oldimgapth5))
                        {
                            System.IO.File.Delete(oldimgapth5);
                        }

                    }


                    return RedirectToAction("ViewProduct");
                }


                else
                {
                    p.P_img1 = Session["imgPath1"].ToString();
                    p.P_img2 = Session["imgPath2"].ToString();
                    p.P_img3 = Session["imgPath3"].ToString();
                    p.P_img4 = Session["imgPath4"].ToString();
                    p.P_img5 = Session["imgPath5"].ToString();
                    db.Entry(p).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("ViewProduct");
                }

            }

            return View(p);
        }
        [Authorize(Roles = "Admin,Author,Editor")]
        //to show the list of product
        [OutputCache(Duration = 20)]
        public ActionResult ViewProduct()
        {
            var pro = db.Product.ToList();
            return View(pro);
        }
        //for deleting th product
        [Authorize(Roles = "Admin,Author,Editor")]
        public ActionResult deleteProduct(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var abc = db.Product.Find(id);

            if (System.IO.File.Exists(abc.P_img1))
            {
                System.IO.File.Delete(abc.P_img1);
            }
            if (System.IO.File.Exists(abc.P_img2))
            {
                System.IO.File.Delete(abc.P_img2);
            }
            if (System.IO.File.Exists(abc.P_img3))
            {
                System.IO.File.Delete(abc.P_img3);
            }
            if (System.IO.File.Exists(abc.P_img4))
            {
                System.IO.File.Delete(abc.P_img4);
            }
            if (System.IO.File.Exists(abc.P_img5))
            {
                System.IO.File.Delete(abc.P_img5);
            }

            db.Entry(abc).State = EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("ViewProduct");
        }

        [Authorize(Roles = "Admin,Page_Controller")]
        public ActionResult pagedetailsedit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var pd = db.pagedetails.Find(id);
            if (pd == null)
            {
                return HttpNotFound();
            }
            return View(pd);
        }
        [HttpPost]
        public ActionResult pagedetailsedit(pagedetails pd)
        {
            if (ModelState.IsValid)
            {

                
                db.Entry(pd).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pd);
        }
        [Authorize(Roles = "Admin,Page_Controller")]
        public ActionResult fsectioncreate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult fsectioncreate(fsection fs, HttpPostedFileBase file)
        {
            if (ModelState.IsValid){
                string filename = Path.GetFileName(file.FileName);
                string _filename = DateTime.Now.ToString("yymmssfff") + filename;
                string extension = Path.GetExtension(file.FileName);
                string path = Path.Combine(Server.MapPath("~/images/"), _filename);
                fs.picture = "~/images/" + _filename;
                file.SaveAs(path);
                db.fsection.Add(fs);
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            return View();
        }
        [Authorize(Roles = "Admin,Page_Controller")]
        public ActionResult fsectionEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var im = db.fsection.Find(id);


            if (im == null)
            {
                return HttpNotFound();
            }
            Session["imgPath"] = im.picture;
            return View(im);
        }
        [HttpPost]
        public ActionResult fsectionEdit(fsection fs, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {   if(file != null) { 
                string filename = Path.GetFileName(file.FileName);
                string _filename = DateTime.Now.ToString("yymmssfff") + filename;
                string extension = Path.GetExtension(file.FileName);
                string path = Path.Combine(Server.MapPath("~/images/"), _filename);
                fs.picture = "~/images/" + _filename;
                    file.SaveAs(path);
                    if (System.IO.File.Exists(Request.MapPath(Session["imgPath"].ToString())))
                    {
                        System.IO.File.Delete(Request.MapPath(Session["imgPath"].ToString()));
                    }
                }
                else
                {
                    fs.picture = Session["imgPath"].ToString();
                }
                db.Entry(fs).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            return View();
        }
        [Authorize(Roles = "Admin,Page_Controller")]
        public ActionResult fsectiondelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var abc = db.fsection.Find(id);

            if (System.IO.File.Exists(Request.MapPath(abc.picture.ToString())))
            {
                System.IO.File.Delete(Request.MapPath(abc.picture.ToString()));
            }
            

            db.Entry(abc).State = EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("index");
        }
        [Authorize(Roles = "Admin,Page_Controller")]
        public ActionResult fsectionlist()
        {
            var c = db.fsection.ToList();
            return View(c);
        }
        [Authorize(Roles = "Admin,Page_Controller")]
        public ActionResult tsectioncreate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult tsectioncreate(tsection ts, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                string filename = Path.GetFileName(file.FileName);
                string _filename = DateTime.Now.ToString("yymmssfff") + filename;
                string extension = Path.GetExtension(file.FileName);
                string path = Path.Combine(Server.MapPath("~/images/"), _filename);
                ts.picture = "~/images/" + _filename;
                file.SaveAs(path);
                db.tsection.Add(ts);
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            return View();
        }
        [Authorize(Roles = "Admin,Page_Controller")]
        public ActionResult tsectionEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var im = db.tsection.Find(id);


            if (im == null)
            {
                return HttpNotFound();
            }
            Session["imgPath"] = im.picture;
            return View(im);
        }
        [HttpPost]
        public ActionResult tsectionEdit(tsection ts, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string filename = Path.GetFileName(file.FileName);
                    string _filename = DateTime.Now.ToString("yymmssfff") + filename;
                    string extension = Path.GetExtension(file.FileName);
                    string path = Path.Combine(Server.MapPath("~/images/"), _filename);
                    ts.picture = "~/images/" + _filename;
                    file.SaveAs(path);
                    if (System.IO.File.Exists(Request.MapPath(Session["imgPath"].ToString())))
                    {
                        System.IO.File.Delete(Request.MapPath(Session["imgPath"].ToString()));
                    }
                }
                else
                {
                    ts.picture = Session["imgPath"].ToString();
                }
                db.Entry(ts).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            return View();
        }
        [Authorize(Roles = "Admin,Page_Controller")]
        public ActionResult tsectiondelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var abc = db.tsection.Find(id);

            if (System.IO.File.Exists(Request.MapPath(abc.picture.ToString())))
            {
                System.IO.File.Delete(Request.MapPath(abc.picture.ToString()));
            }


            db.Entry(abc).State = EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("index");
        }
        [Authorize(Roles = "Admin,Page_Controller")]
        public ActionResult tsectionlist()
        {
            var c = db.tsection.ToList();
            return View(c);
        }
        [Authorize(Roles = "Admin,Page_Controller")]
        public ActionResult thsectionEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var im = db.thsection.Find(id);
            //getting list and saving in c for dropdown
            var c = db.catagory.ToList();
            List<SelectListItem> catlist = new List<SelectListItem>();
            foreach (var item in c)
            {
                catlist.Add(new SelectListItem { Text = item.name, Value = item.c_id.ToString() });
            }
            ViewBag.roles = catlist;

            if (im == null)
            {
                return HttpNotFound();
            }
            Session["imgPath"] = im.picture;
            return View(im);
        }
        [HttpPost]
        public ActionResult thsectionEdit(thsection th, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string filename = Path.GetFileName(file.FileName);
                    string _filename = DateTime.Now.ToString("yymmssfff") + filename;
                    string extension = Path.GetExtension(file.FileName);
                    string path = Path.Combine(Server.MapPath("~/images/"), _filename);
                    th.picture = "~/images/" + _filename;
                    file.SaveAs(path);
                    if (System.IO.File.Exists(Request.MapPath(Session["imgPath"].ToString())))
                    {
                        System.IO.File.Delete(Request.MapPath(Session["imgPath"].ToString()));
                    }
                }
                else
                {
                    th.picture = Session["imgPath"].ToString();
                }
                db.Entry(th).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            return View();
        }
        [Authorize(Roles = "Admin,Page_Controller")]
        public ActionResult fosectionEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var im = db.fosection.Find(id);
            //getting list and saving in c for dropdown
            var c = db.catagory.ToList();
            List<SelectListItem> catlist = new List<SelectListItem>();
            foreach (var item in c)
            {
                catlist.Add(new SelectListItem { Text = item.name, Value = item.c_id.ToString() });
            }
            ViewBag.roles = catlist;

            if (im == null)
            {
                return HttpNotFound();
            }
            Session["imgPath"] = im.picture;
            return View(im);
        }
        [HttpPost]
        public ActionResult fosectionEdit(fosection fo, HttpPostedFileBase file)
        {
            var a = fo;
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string filename = Path.GetFileName(file.FileName);
                    string _filename = DateTime.Now.ToString("yymmssfff") + filename;
                    string extension = Path.GetExtension(file.FileName);
                    string path = Path.Combine(Server.MapPath("~/images/"), _filename);
                    fo.picture = "~/images/" + _filename;
                    file.SaveAs(path);
                    if (System.IO.File.Exists(Request.MapPath(Session["imgPath"].ToString())))
                    {
                        System.IO.File.Delete(Request.MapPath(Session["imgPath"].ToString()));
                    }
                }
                else
                {
                    fo.picture = Session["imgPath"].ToString();
                }
                db.Entry(fo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            return RedirectToAction("Index");
        }





        [Authorize(Roles = "Admin,Page_Controller")]
        public ActionResult fisectionEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var im = db.fisection.Find(id);
            //getting list and saving in c for dropdown
            var c = db.catagory.ToList();
            List<SelectListItem> catlist = new List<SelectListItem>();
            foreach (var item in c)
            {
                catlist.Add(new SelectListItem { Text = item.name, Value = item.c_id.ToString() });
            }
            ViewBag.roles = catlist;

            if (im == null)
            {
                return HttpNotFound();
            }
            Session["imgPath"] = im.picture;
            return View(im);
        }
        [HttpPost]
        public ActionResult fisectionEdit(fisection fi, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string filename = Path.GetFileName(file.FileName);
                    string _filename = DateTime.Now.ToString("yymmssfff") + filename;
                    string extension = Path.GetExtension(file.FileName);
                    string path = Path.Combine(Server.MapPath("~/images/"), _filename);
                    fi.picture = "~/images/" + _filename;
                    file.SaveAs(path);
                    if (System.IO.File.Exists(Request.MapPath(Session["imgPath"].ToString())))
                    {
                        System.IO.File.Delete(Request.MapPath(Session["imgPath"].ToString()));
                    }
                }
                else
                {
                    fi.picture = Session["imgPath"].ToString();
                }
                db.Entry(fi).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            return View();
        }
        [Authorize(Roles = "Admin,Page_Controller")]
        public ActionResult EditContactus(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var im = db.contactus.Find(id);
            return View(im);
        }
        [HttpPost]
        public ActionResult EditContactus(int id,contactus cus)
        {
            if (ModelState.IsValid) {
            db.Entry(cus).State = EntityState.Modified;
            db.SaveChanges(); }
            return View();
        }

    }
}