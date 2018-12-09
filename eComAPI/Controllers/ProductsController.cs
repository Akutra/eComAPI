using eComAPI.Data.Entity;
using eComAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace eComAPI.Controllers
{
    public class ProductsController : ApiController
    {
        //private ShopDBContext db = new ShopDBContext();

        public ProductsController()
        {
            dbInstance.DefaultData();
        }

        public ShopDBContext dbInstance
        {
            get
            {
                object ShopGlobal = HttpContext.Current.Application["ShopEntityDB"];
                if (ShopGlobal == null)
                {
                    ShopGlobal = new ShopDBContext();
                    HttpContext.Current.Application["ShopEntityDB"] = ShopGlobal as ShopDBContext;
                }
                return ShopGlobal as ShopDBContext;
            }
        }

        // GET: Products ...get all collections
        [HttpGet]
        [Route("Products")]
        public IHttpActionResult GetProducts() //
        {
            bool count = false;
            object rt = null;

            try
            {
                //get the search string from the content
                if (HttpContext.Current.Request.QueryString.Count > 0)
                {
                    if (HttpContext.Current.Request.QueryString["count"] == "true")
                        count = true;
                }

            }
            catch (Exception _e) { }

            if (count)
            {
                rt = new { count = dbInstance.Products.Values.Count };
            } else
            {
                rt = (object)dbInstance.Products.Values;
            }

            return new eOkResult(rt);
        }

        // GET: Products/{Product Id} (i.e. number) ...get a product
        [HttpGet]
        [Route("Products/{id:int}")]
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> GetProducts(int id)
        {
            Product product = await dbInstance.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return new eOkResult(product);
        }

        // PUT: Products/{Product Id} (i.e. number) ....update product
        [HttpPut]
        [Route("Products/{id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.CollectionId)
            {
                return BadRequest();
            }

            dbInstance.Products.Entry(product).State = EntityState.Modified;

            try
            {
                //await db.SaveChangesAsync();
            }
            catch (Exception) //catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    dbInstance.Products.Update(product);
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: Products ...add product
        [HttpPost]
        [Route("Products")]
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            dbInstance.Products.Add(product);
            //await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = product.ProductId }, product);
        }

        // DELETE: Products/{Product Id} (i.e. number) ....delete a product
        [HttpDelete]
        [Route("Products/{id:int}")]
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> DeleteProduct(int id)
        {
            Product product = await dbInstance.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            dbInstance.ProductCollections.Remove(product);
            //await db.SaveChangesAsync();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbInstance.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return dbInstance.Products.Count(e => e.CollectionId == id) > 0;
        }
    }
}
