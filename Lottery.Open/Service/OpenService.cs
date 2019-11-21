using Lottery.HttpClient;
using Lottery.Open.Model;
using Lottery.Open.Model.kaijiang._500;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using System.Xml;

namespace Lottery.Open.Service
{
    public class OpenService: IOpenService
    {
        public OpenResult GetOpenResult()
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
            lottery l = null;
            if (xmlNode != null)
            {
                l = XMLSerilizable.XMLToObject<lottery>(xmlNode.OuterXml, Encoding.UTF8);
            }
            return l;
        }
    }
}
