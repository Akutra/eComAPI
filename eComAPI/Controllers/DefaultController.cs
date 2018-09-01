using eComAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace eComAPI.Controllers
{
    public class DefaultController : ApiController
    {
        private ShopDBContext db = new ShopDBContext();

        [HttpGet]
        [Route("")]
        public string Get()
        {
            db.DefaultData();

            return DateTime.Now.ToString();
        }
    }
}
