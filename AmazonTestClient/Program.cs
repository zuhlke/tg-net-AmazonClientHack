using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Linq;

namespace AmazonTestClient
{
    public class Program
    {
        private const string MY_AWS_ACCESS_KEY_ID = "??????";
        private const string MY_AWS_SECRET_KEY = "??????";
        private const string MY_ASSOCIATE_ID = "??????";
        private const string DESTINATION = "webservices.amazon.co.uk";

        public static void Main()
        {
            SignedRequestHelper helper = new SignedRequestHelper(MY_AWS_ACCESS_KEY_ID, MY_AWS_SECRET_KEY, DESTINATION);

            /*
             * The helper supports two forms of requests - dictionary form and query string form.
             */
            String requestUrl;
            String searchResults;

            IDictionary<string, string> r1 = new Dictionary<string, String>();
            r1["Service"] = "AWSECommerceService";
            r1["Operation"] = "ItemSearch";
            r1["Keywords"] = "the hunger games";
            r1["SearchIndex"] = "Books";
            r1["AssociateTag"] = MY_ASSOCIATE_ID;

            requestUrl = helper.Sign(r1);
            searchResults = FetchResults(requestUrl);
            
            string firstAsin = null;
            using (var memStr = new System.IO.MemoryStream())
            {
                using (var writer = new System.IO.StreamWriter(memStr))
                {
                    writer.Write(searchResults);
                    writer.Flush();

                    memStr.Seek(0, System.IO.SeekOrigin.Begin);

                    var doc = new XmlDocument();
                    doc.Load(memStr);

                    firstAsin = doc.GetElementsByTagName("ASIN")[0].InnerText;
                }

                if (!string.IsNullOrEmpty(firstAsin))
                {
                    IDictionary<string, string> r2 = new Dictionary<string, String>();
                    r2["ItemType"] = "ASIN";
                    r2["ItemId"] = firstAsin;
                    r2["ResponseGroup"] = "Offers";
                    r2["Operation"] = "ItemLookup";
                    r2["Service"] = "AWSECommerceService";
                    r2["AssociateTag"] = MY_ASSOCIATE_ID;

                    var requestUrl2 = helper.Sign(r2);
                    try
                    {
                        var searchResults2 = FetchResults(requestUrl2);
                    }
                    catch (Exception ex)
                    {
                        int qq = 0;
                    }
                    
                }  
            }

            

            int q = 0;
        }

        private static string FetchResults(string url)
        {
            using (var wc = new System.Net.WebClient())
            {
                try
                {
                    return wc.DownloadString(url);
                }
                catch (Exception ex)
                {
                    int q = 0;
                    throw;
                }
                
            }
        }
    }
}
