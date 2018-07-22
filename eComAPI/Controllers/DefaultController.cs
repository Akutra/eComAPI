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
        [HttpGet]
        [Route("")]
        public string Get()
        {
            return DateTime.Now.ToString();
        }
    }
}
