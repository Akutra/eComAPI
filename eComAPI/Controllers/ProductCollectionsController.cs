using eComAPI.Data.Entity;
using eComAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace eComAPI.Controllers
{
    public class ProductCollectionsController : ApiController
    {
        private ShopDBContext db = new ShopDBContext();

        // GET: api/ProductCollections
        public IQueryable<ProductCollection> GetProductCollections()
        {
            return db.ProductCollections.Values.AsQueryable();
        }

        // GET: api/ProductCollections/5
        [ResponseType(typeof(ProductCollection))]
        public async Task<IHttpActionResult> GetProductCollections(int id)
        {
            ProductCollection productCollections = await db.ProductCollections.FindAsync(id);
            if (productCollections == null)
            {
                return NotFound();
            }

            return Ok(productCollections);
        }

        // PUT: api/ProductCollections/5
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

            db.ProductCollections.Entry(productCollections).State = EntityState.Modified;

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
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ProductCollections
        [ResponseType(typeof(ProductCollection))]
        public async Task<IHttpActionResult> PostProductCollections(ProductCollection productCollections)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ProductCollections.Add(productCollections);
            //await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = productCollections.CollectionId }, productCollections);
        }

        // DELETE: api/ProductCollections/5
        [ResponseType(typeof(ProductCollection))]
        public async Task<IHttpActionResult> DeleteProductCollections(int id)
        {
            ProductCollection productCollections = await db.ProductCollections.FindAsync(id);
            if (productCollections == null)
            {
                return NotFound();
            }

            db.ProductCollections.Remove(productCollections);
            //await db.SaveChangesAsync();

            return Ok(productCollections);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductCollectionsExists(int id)
        {
            return db.ProductCollections.Count(e => e.CollectionId == id) > 0;
        }
    }
}
