using FileHelpers;
using Lottery.Common;
using Lottery.HttpClient;
using Lottery.Open.Model.kaijiang._500;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using System.Xml;

namespace UnitTestProject1
{
    [TestClass]
    public class HttpRequestFactoryTests
    {
        [TestMethod]
        public void GetsTest()
        {
            var response = HttpRequestFactory.Get("https://kaijiang.500.com/static/info/kaijiang/xml/index.xml");
            var result = response.Result.ContentAsString();
            Console.WriteLine(result);
        }

        [TestMethod]
        public void GetsTest1()
        {
            var response = HttpRequestFactory.Get("https://kaijiang.500.com/static/info/kaijiang/xml/index.xml");
            Byte[] b = response.Result.Content.ReadAsByteArrayAsync().Result;
            var streamReceive = new GZipStream(response.Result.Content.ReadAsStreamAsync().Result, CompressionMode.Decompress);
            XmlDocument dom = new XmlDocument();
            dom.Load(streamReceive);//这个地方需要注意
            XmlNodeList xmlNodeList = dom.SelectNodes("//lottery");
            XmlNode xmlNode = null;
            foreach (XmlNode el in xmlNodeList)
            {
                if (el.InnerXml.Contains("qxc") && el.InnerXml.Contains("7星彩"))
                {
                    xmlNode = el;
                }
            }
            //XmlNode xmlNode1 = dom.SelectSingleNode("//lottery");
            //XmlNode xmlNode = dom.SelectSingleNode("//open");


            //StringBuilder output = new StringBuilder();
            //XmlWriter xmlWriter = XmlWriter.Create(output);
            //xmlNode.WriteTo(xmlWriter);
            //Console.WriteLine(output.ToString());
            if (xmlNode != null)
            {
                lottery l = XMLSerilizable.XMLToObject<lottery>(xmlNode.OuterXml, Encoding.UTF8);
            }
            //open or = XMLSerilizable.XMLToObject<open>(xmlNode.OuterXml, Encoding.UTF8);

            string Flag = "";
            //string Flag = System.Text.Encoding.UTF8.GetString(b, 0, b.Length);
            //string Flag = System.Text.Encoding.Unicode.GetString(b, 0, b.Length);
            //string andy = System.Text.Encoding.GetEncoding("GB2312").GetString(b).Trim();
            using (System.IO.StreamReader sr = new System.IO.StreamReader(streamReceive, Encoding.UTF8))
            {
                Flag = sr.ReadToEnd();
            }
            Console.WriteLine(Flag);
        }

        [DelimitedRecord("")]
        public class Orders
        {
            public Orders() { }
            public Orders(string id)
            {
                this.OrderID = id;
            }
            public string OrderID;
        }

        [TestMethod]
        public void FilehelpersTest1()
        {
            var engine = new FileHelperEngine<Orders>();
            var orders = new List<Orders>();
            for (int i = 0; i < 100; i++)
            {
                orders.Add(new Orders($@"[Column(ordinal: ""{i}"")]
        public float t{i};"));
            }
            engine.WriteFile("Output.Txt", orders);
        }

        [TestMethod]
        public void DateTimeUtilityTest()
        {
            var dt =DateTime.Parse("2019-07-08");
            Console.WriteLine(dt.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine(DateTimeUtility.ConvertToTimeStamp(dt));
        }

        [TestMethod]
        public void IndexOfTest()
        {
            string str = "1,2,3,4,5,6,7,13232";
            Console.WriteLine(str);
            int indexOne = str.IndexOf(',');
            Console.WriteLine(indexOne);
            str = str.Substring(indexOne+1);
            Console.WriteLine(str);
            int indexTwo = str.LastIndexOf(',');
            Console.WriteLine(indexTwo);
            str = str.Substring(0, indexTwo + 1);
            Console.WriteLine(str);

        }
    }
}
