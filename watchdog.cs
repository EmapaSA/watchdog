using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;


namespace ConsoleApp2
{
    internal class Program
    {
        private static readonly string[] addresses = {
"https://inpost-searchapi.emapa.pl/address?searchphrase=aten&limitToCountry=PL&rnd=",
"https://inpost-searchapi.emapa.pl/degeocode/structured?lon=21&lat=54&radius=1000&rnd=",
"https://inpost-searchapi.emapa.pl/geocode?city=%C5%82%C3%B3d%C5%BA&street=%C5%82%C4%85kowa&houseno=11&postcode=90-562&rnd="
};
       public class Logi
        {
            public string adres { get; set; }
            public int serv_ans { get; set; }
            public string time { get; set; }
        }
        static async Task Main(string[] args)
        {

            var interval = new TimeSpan(0, 0, 1);
            var startTime = DateTime.Now;
            var endTime = startTime + TimeSpan.FromSeconds(100000000);
            while (DateTime.Now <= endTime)
            {
                foreach (string Address in addresses)
                {

                    await MakeRequest(Address);
                }
                await Task.Delay(interval);
            }
        }

        private static async Task MakeRequest(string address)
        {
            String GetTimestamp(DateTime value)
            {
                return value.ToString("yyyyMMddHHmmssffff");
            }
            String timeStamp = GetTimestamp(DateTime.Now);
            var requestId = Guid.NewGuid();
            var client = new HttpClient();
            address = address + timeStamp;
            Console.WriteLine($"[{requestId}][{DateTime.Now}] Making request to: {address}");
            var response = await client.GetAsync(address);
            if(response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine  ($"[{requestId}][{DateTime.Now}] Error (code {(int)response.StatusCode}):\n{await response.Content.ReadAsStringAsync()}");
                Logi logi = new Logi();
                    logi.adres= address;
                    logi.serv_ans= (int)response.StatusCode;
                    logi.time=timeStamp;
                XmlSerializer xs = new XmlSerializer(typeof(Logi));
                using (Stream s = File.Create("logi.xml"))
                    xs.Serialize(s, logi);
            }
            else
            {
                Console.WriteLine ($"[{requestId}][{DateTime.Now}] Everything's fine");
            }
        }
    }
}
