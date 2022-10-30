using INF272_HW06.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using System.Collections;
using System.Data.Entity;

namespace INF272_HW06.Controllers
{
    public class HomeController : Controller
    {
        private readonly BikeStoresEntities db = new BikeStoresEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return PartialView();
        }

        public string GetCategoryNames()
        {
            object categoryData = db.categories.Select(p => new { id = p.category_id, name = p.category_name }).ToList();
            return JsonConvert.SerializeObject(categoryData);
        }

        public string GetProducts(int? i)
        {
            db.Configuration.ProxyCreationEnabled = false;
            object productDatas = db.products.Select(p => new { id = p.product_id, name = p.product_name, brand = p.brand.brand_name, category = p.category.category_name, model = p.model_year, price = p.list_price }).ToList().ToPagedList(i ?? 1, 10);
            return JsonConvert.SerializeObject(productDatas);
        }

        public string Search(string text)
        {
            db.Configuration.ProxyCreationEnabled = false;
            object productDatas = db.products.Where(o => o.product_name.Contains(text)).Select(p => new { id = p.product_id, name = p.product_name, brand = p.brand.brand_name, category = p.category.category_name, model = p.model_year, price = p.list_price }).ToList();
            return JsonConvert.SerializeObject(productDatas);
        }
        public string GetBrandData()
        {
            db.Configuration.ProxyCreationEnabled = false;

            List<brand> data = db.brands.ToList();

            return JsonConvert.SerializeObject(data);
        }
        public ActionResult Orders()
        {
            
            List<OrderVM> productDatas = db.order_items.Select(p => new OrderVM { orderid = p.order_id, category = p.product.category.category_name, product = db.products.Where(x => x.product_id == p.product_id).FirstOrDefault(), quantity = p.quantity, price = p.list_price, total = (p.list_price * p.quantity), orderdate = db.orders.Where(o => o.order_id == p.order_id).FirstOrDefault().order_date }).ToList();
            return View(productDatas);
        }

        public ActionResult SearchOrders(DateTime date)
        {
            string day = date.ToShortDateString();
            
            List<OrderVM> productDatas = db.order_items.Where(y => y.order.order_date <= date).Select(p => new OrderVM { orderid = p.order_id, category = p.product.category.category_name, product = db.products.Where(x => x.product_id == p.product_id).FirstOrDefault(), quantity = p.quantity, price = p.list_price, total = (p.list_price * p.quantity), orderdate = db.orders.Where(o => o.order_id == p.order_id).FirstOrDefault().order_date }).ToList();
            return View("Orders", productDatas);
        }

        public ActionResult Report()
        {

            return View();
        }

        public string ProductDetails(int id)
        {
            //db.Configuration.ProxyCreationEnabled = false;
            object productDetial = db.stocks.Where(y => y.product_id == id).Include(v => v.product).Select(p => new {
                productname = p.product.product_name,
                year = p.product.model_year,
                price = p.product.list_price,
                brand = p.product.brand.brand_name,
                category = p.product.category.category_name,
                stores = db.stocks.Where(s => s.product_id == id).Select(n => new { storename = n.store.store_name, quantity = n.quantity })

            }).FirstOrDefault();


            return JsonConvert.SerializeObject(productDetial);

        }

        public string GetReports()
        {
            db.Configuration.ProxyCreationEnabled = false;
            object bikes = db.orders.Select(o => new
            {
                //orderid = o.order_id,

                month = o.order_date.Month,
                bike = db.order_items.Where(x => x.order_id == o.order_id && x.product.category.category_id == 6).ToList(),
            }).ToList();

            return JsonConvert.SerializeObject(bikes);
        }


    }
}