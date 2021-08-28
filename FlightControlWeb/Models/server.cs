using System;
using System.Text;

namespace FlightControlWeb.Controllers
{
    public class server
    {
        public string ServerId { get; set; }
        public string ServerURL { get; set; }

        public server()
        {
        }

        public server(string serverId, string serverURL)
        {
            ServerId = serverId;
            ServerURL = serverURL;
        }

         public string generateID()
        {
            Random random = new Random();
            StringBuilder sb = new StringBuilder();
            int num = random.Next(10, 100);
            sb.Append(num);
            char c = Convert.ToChar(random.Next(65, 90));
            sb.Append(c);
            c = Convert.ToChar(random.Next(65, 90));
            sb.Append(c);
            //c = Convert.ToChar(random.Next(65, 90));
            //sb.Append(c);
            //num = random.Next(10, 100);
            //sb.Append(num);
            ServerId = sb.ToString();
            return sb.ToString();
        }
    }
}
