using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace eComAPI.Models
{
    public class eOkResult : IHttpActionResult
    {
        string JsonReturn = "";

        public eOkResult(object Output)
        {
            if (Output!=null)
            {
                JsonReturn = JsonConvert.SerializeObject(Output);
            }
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(JsonReturn);

            MediaTypeHeaderValue _mhv = new MediaTypeHeaderValue("application/json");
            _mhv.CharSet = "UTF-8";
            
            response.Content.Headers.ContentType = _mhv;
            response.Headers.Date = new DateTimeOffset(DateTime.Now);
            
            
            
            return Task.FromResult(response);
        }

    }

    public class aboutContent
    {
        public string Title = "";
        public string Company = "";
        public string FullDescription = "";
        public string configuration = "";
        public string FullyQualifiedName = "";
        public string GroupCodeName = "";
        public string Copyright = "";
        public string Trademark = "";
        public string Culture = "";
        public int revloop = 0;
        public int enkre = 0;
        public string echo = "";
        public string CreationDatum = "09-08-0/45 AI";
        public string CurrentDatum = "";

        public aboutContent()
        {
            var _cAttr = Assembly.GetExecutingAssembly().CustomAttributes;
            string _attFQID = "";

            foreach(object attr in _cAttr)
            {
                _attFQID = attr.ToString();
                if (!_attFQID.Contains("Assembly"))
                    continue;

                if(_attFQID.Contains("AssemblyTitleAttribute"))
                    Title = ((CustomAttributeTypedArgument)((CustomAttributeData)attr).ConstructorArguments[0]).Value.ToString();

                if (_attFQID.Contains("AssemblyDescriptionAttribute"))
                    FullDescription = ((CustomAttributeTypedArgument)((CustomAttributeData)attr).ConstructorArguments[0]).Value.ToString();

                if (_attFQID.Contains("AssemblyConfigurationAttribute"))
                    configuration = ((CustomAttributeTypedArgument)((CustomAttributeData)attr).ConstructorArguments[0]).Value.ToString();

                if (_attFQID.Contains("AssemblyCompanyAttribute"))
                    Company = ((CustomAttributeTypedArgument)((CustomAttributeData)attr).ConstructorArguments[0]).Value.ToString();

                if (_attFQID.Contains("AssemblyProductAttribute"))
                    GroupCodeName = ((CustomAttributeTypedArgument)((CustomAttributeData)attr).ConstructorArguments[0]).Value.ToString();

                if (_attFQID.Contains("AssemblyCopyrightAttribute"))
                    Copyright = ((CustomAttributeTypedArgument)((CustomAttributeData)attr).ConstructorArguments[0]).Value.ToString();

                if (_attFQID.Contains("AssemblyTrademarkAttribute"))
                    Trademark = ((CustomAttributeTypedArgument)((CustomAttributeData)attr).ConstructorArguments[0]).Value.ToString();

                if (_attFQID.Contains("AssemblyCultureAttribute"))
                    Culture = ((CustomAttributeTypedArgument)((CustomAttributeData)attr).ConstructorArguments[0]).Value.ToString();

                if (_attFQID.Contains("AssemblyMetadataAttribute"))
                {
                    if (_attFQID.Contains("FullyQualifiedName"))
                        FullyQualifiedName = ((CustomAttributeTypedArgument)((CustomAttributeData)attr).ConstructorArguments[1]).Value.ToString();

                    if (_attFQID.Contains("revloop"))
                        int.TryParse(((CustomAttributeTypedArgument)((CustomAttributeData)attr).ConstructorArguments[1]).Value.ToString(), out revloop);

                    if (_attFQID.Contains("enkre"))
                        int.TryParse(((CustomAttributeTypedArgument)((CustomAttributeData)attr).ConstructorArguments[1]).Value.ToString(), out enkre);

                    if (_attFQID.Contains("echo"))
                        echo = ((CustomAttributeTypedArgument)((CustomAttributeData)attr).ConstructorArguments[1]).Value.ToString();

                    if (_attFQID.Contains("CreationDatum"))
                        CreationDatum = ((CustomAttributeTypedArgument)((CustomAttributeData)attr).ConstructorArguments[1]).Value.ToString();
                }
            }
        }
    }

    public class aboutResult : IHttpActionResult
    {
        aboutContent _rt = null;

        public aboutResult(aboutContent Output)
        {
            if (Output != null)
            {
                _rt = Output;
            } else
            {
                _rt = new aboutContent();
            }
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            string output = "<html><h1>" + _rt.FullDescription + "</h1>";
            output += "<h3>Fully Qualified Name : " + _rt.FullyQualifiedName + "</h3>";
            output += "<p>Code Name : " + _rt.GroupCodeName + ", revloop " + _rt.revloop + " enkre " + _rt.enkre + " echo " + _rt.echo + "</p>";
            output += "<p>Datum : " + _rt.CreationDatum + "</p>";
            output += "<p>Current Datum : " + _rt.CurrentDatum + "</p></html>";

            response.Content = new StringContent(output);

            MediaTypeHeaderValue _mhv = new MediaTypeHeaderValue("text/html");
            _mhv.CharSet = "UTF-8";

            response.Content.Headers.ContentType = _mhv;
            response.Headers.Date = new DateTimeOffset(DateTime.Now);



            return Task.FromResult(response);
        }

    }
}