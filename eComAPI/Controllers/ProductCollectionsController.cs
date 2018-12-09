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
    public class ProductCollectionsController : ApiController
    {
        //private ShopDBContext db = new ShopDBContext();

        public ProductCollectionsController()
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

        // GET: ProductCollections ...get all collections
        [HttpGet]
        [Route("ProductCollections")]
        public IHttpActionResult GetProductCollections() //
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
                rt = new { count = dbInstance.ProductCollections.Values.Count };
            }
            else
            {
                rt = (object)dbInstance.ProductCollections.Values;
            }

            return new eOkResult((object)dbInstance.ProductCollections.Values);
        }

        // GET: ProductCollections/{Collection Id} (i.e. number) ...get a collection
        [HttpGet]
        [Route("ProductCollections/{id:int}")]
        [ResponseType(typeof(ProductCollection))]
        public async Task<IHttpActionResult> GetProductCollections(int id)
        {
            ProductCollection productCollections = await dbInstance.ProductCollections.FindAsync(id);
            if (productCollections == null)
            {
                return NotFound();
            }

            return new eOkResult(productCollections);
        }

        // PUT: ProductCollections/{Collection Id} (i.e. number) ....update collection
        [HttpPut]
        [Route("ProductCollections/{id:int}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProductCollections(int id, ProductCollection productCollections)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productCollections.CollectionId)
            {
                return BadRequest();
            }

            dbInstance.ProductCollections.Entry(productCollections).State = EntityState.Modified;

            try
            {
                //await db.SaveChangesAsync();
            }
            catch (Exception) //catch (DbUpdateConcurrencyException)
            {
                if (!ProductCollectionsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    dbInstance.ProductCollections.Update(productCollections);
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: ProductCollections ...add collection
        [HttpPost]
        [Route("ProductCollections")]
        [ResponseType(typeof(ProductCollection))]
        public async Task<IHttpActionResult> PostProductCollections(ProductCollection productCollections)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            dbInstance.ProductCollections.Add(productCollections);
            //await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = productCollections.CollectionId }, productCollections);
        }

        // DELETE: ProductCollections/{Collection Id} (i.e. number) ....delete a collection
        [HttpDelete]
        [Route("ProductCollections/{id:int}")]
        [ResponseType(typeof(ProductCollection))]
        public async Task<IHttpActionResult> DeleteProductCollections(int id)
        {
            ProductCollection productCollections = await dbInstance.ProductCollections.FindAsync(id);
            if (productCollections == null)
            {
                return NotFound();
            }

            dbInstance.ProductCollections.Remove(productCollections);
            //await db.SaveChangesAsync();

            return Ok(productCollections);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbInstance.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductCollectionsExists(int id)
        {
            return dbInstance.ProductCollections.Count(e => e.CollectionId == id) > 0;
        }
    }
}
