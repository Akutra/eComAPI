using eComAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.UI;

namespace eComAPI.Controllers
{

    public class DefaultController : ApiController
    {
        //private ShopDBContext ShopDB;

        public DefaultController()
        {
            //ShopDB = instance;
            //dbInstance.DefaultData();
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            string newDt = DateTime.Now.Month.ToString("D2") + "-" + DateTime.Now.Day.ToString("D2") + "-0/" + (DateTime.Now.Year - 1973) + " AI";

            aboutContent _rt = new aboutContent();
            _rt.CurrentDatum = newDt;

            //object _rt = new { FullDescription = "eComAPI RESTful Services engine revision", FullyQualifiedName = "RESTful-engine-eComAPI-LeapMaker-Ikhwwtre-stargroup", GroupCodeName = "Ikhwwtre", revloop = 1, enkre = 2, echo = "A", CreationDatum = "09-08-0/45 AI", TodaysDatum = newStr };

            //newStr = JsonConvert.SerializeObject(_rt);

            //HttpContext.Current.Response.Write(newStr);
            
            return new aboutResult(_rt);
        }

        /*
        public ShopDBContext dbInstance
        {
            get
            {
                object ShopGlobal = HttpContext.Current.Application["ShopEntityDB"];
                if( ShopGlobal==null )
                {
                    ShopGlobal = new ShopDBContext();
                    HttpContext.Current.Application["ShopEntityDB"] = ShopGlobal as ShopDBContext;
                }
                return ShopGlobal as ShopDBContext;
            }
        }*/
    }
}
