using System.Net;

namespace server
{
    public class Program
    {
        static void Main(string[] args)
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            Server server = new Server(ipAddress);

            server.Start();
        }
    }
}
