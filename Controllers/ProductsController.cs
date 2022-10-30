using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using INF272_HW06.Models;
using Newtonsoft.Json;

namespace INF272_HW06.Controllers
{
    public class ProductsController : Controller
    {
        private BikeStoresEntities db = new BikeStoresEntities();

        // GET: Products
        public ActionResult Index()
        {
            var products = db.products.Include(p => p.brand).Include(p => p.category);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
         

            return PartialView();

        }

   // get products
        public ActionResult Create()
        {
            ViewBag.brand_id = new SelectList(db.brands, "brand_id", "brand_name");
            ViewBag.category_id = new SelectList(db.categories, "category_id", "category_name");
            return View();
        }

        
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "product_id,product_name,brand_id,category_id,model_year,list_price")] product product)
        {
            if (ModelState.IsValid)
            {
                db.products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.brand_id = new SelectList(db.brands, "brand_id", "brand_name", product.brand_id);
            ViewBag.category_id = new SelectList(db.categories, "category_id", "category_name", product.category_id);
            return View(product);
        }

        // GET: Products to edit
        public ActionResult EditView()
        {
            return PartialView();
        }
        public string EditProd(int? id)
        {
           
            db.Configuration.ProxyCreationEnabled = false;
           
            object product = db.products.Where(x => x.product_id == id).Select(p => new { id = p.product_id, name = p.product_name, brand = p.brand.brand_name, catergory = p.category.category_name, model = p.model_year, price = p.list_price }).FirstOrDefault();
        
            return JsonConvert.SerializeObject(product);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public ActionResult Edit(int product_id, string product_name, int brand_id, int category_id, short model_year, decimal list_price)
        {
            product product = new product
            {
                product_id = product_id,
                product_name = product_name,
                brand_id = brand_id,
                category_id = category_id,
                model_year = model_year,
                list_price = list_price

            };
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.brand_id = new SelectList(db.brands, "brand_id", "brand_name", product.brand_id);
            ViewBag.category_id = new SelectList(db.categories, "category_id", "category_name", product.category_id);
            return View(product);
        }

        // GET: Products/Delete/
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return PartialView();
        }

     
        //[ValidateAntiForgeryToken]
        public string DeleteConfirmed(int id)
        {
            product product = db.products.Find(id);
            db.products.Remove(product);
            db.SaveChanges();
            return JsonConvert.SerializeObject(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}