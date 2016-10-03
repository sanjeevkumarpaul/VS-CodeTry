//using System;
using System.Net;
using System.Threading.Tasks;
using System.IO;


namespace Utilities.WebClients
{
    public class WebCall
    {
        public async Task<string> Call(string url = "http://www.dreamincode.net/forums/xml.php?showuser=1253")
        {        
            string xml = string.Empty;
            var webReq =  WebRequest.Create(url);
            {
                WebResponse response = await webReq.GetResponseAsync();
                using(var stream = response.GetResponseStream())
                {
                    var tStream = new StreamReader(stream);
                    xml = tStream.ReadToEnd();
                }
            }
                        
            return xml;
        }
    
    }    
}