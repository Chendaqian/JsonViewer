using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using System.Xml;

namespace TestConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            /*  System.Collections classes
             *  System.Collections.ArrayList
             *  System.Collections.BitArray
             *  System.Collections.HashTable
             *  System.Collections.Queue
             *  System.Collections.SortedList
             *  System.Collections.Stack
             *  System.Collections.Specialized classes
             *  System.Collections.Specialized.HybridDictionary
             *  System.Collections.Specialized.ListDictionary
             *  System.Collections.Specialized.NameValueCollection
             *  System.Collections.Specialized.OrderedDictionary
             *  System.Collections.Specialized.StringCollection
             *  System.Collections.Specialized.StringDictionary
             *
             *  All classes derived from System.Collections.CollectionBase
             *  All classes derived from System.Collections.Specialized.NameObjectCollectionBase
             *  System.Collections.Generic classes
             *  System.Collections.Generic.Dictionary
             *  System.Collections.Generic.List
             *  System.Collections.Generic.LinkedList
             *  System.Collections.Generic.Queue
             *  System.Collections.Generic.SortedDictionary
             *  System.Collections.Generic.SortedList
             *  System.Collections.Generic.Stack
             *
             *  IIS classes, as used by
             *  System.Web.HttpRequest.Cookies
             *  System.Web.HttpRequest.Files
             *  System.Web.HttpRequest.Form
             *  System.Web.HttpRequest.Headers
             *  System.Web.HttpRequest.Params
             *  System.Web.HttpRequest.QueryString
             *  System.Web.HttpRequest.ServerVariables
             *  System.Web.HttpResponse.Cookies
             */

            //NameValueCollection
            //NameValueCollection Params = Request.Params;
            //NameValueCollection QueryString = Request.QueryString;
            //NameValueCollection Form = Request.Form;
            //NameValueCollection RequestHeaders = Request.Headers;
            //NameValueCollection ResponseHeaders = Response.Headers;
            NameValueCollection nameValueCollection = new NameValueCollection
            {
                {"name1","value1"},
                {"name2","value2"},
                {"name3","value3"},
                {"name4","value4"},
            };

            ////HttpCookieCollection
            //HttpCookieCollection RequestCookies = Request.Cookies;
            //HttpCookieCollection ResponseCookies = Response.Cookies;
            HttpCookieCollection httpCookieCollection = new HttpCookieCollection
            {
                new HttpCookie("name1","value1"),
                new HttpCookie("name1","value1"),
                new HttpCookie("name1","value1"),
            };

            //List
            var list = new List<int> { 1, 2, 3 };

            // 测试 json
            string json = @"{
  'NAME' : 'Luckey' ,
  'Sex' : '男' ,
  'LOCATION' : {
     'PROVINCE' : 'LN' ,
     'CITY' : 'DL'
} ,
  'INTEREST' : [
    '.NET'
  ]
}";
            TestXML();
            Console.ReadKey();
        }

        private static void TestXML()
        {
            string url = "https://im.yuge.com/im/yx/attachment/text/1roqp5qrz8dy3ws813eiky53is.xml";//新版
            using (Stream stream = WebRequest.Create(url).GetResponse().GetResponseStream())
            {
                XmlDocument xmldoc = new XmlDocument();


                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();


                xmldoc.Load(stream);
                string json = JsonConvert.SerializeXmlNode(xmldoc).Replace("@", "").Replace("#", "");
                XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(json, "root");

                string jsonText = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented, true);
            }
        }
    }
}