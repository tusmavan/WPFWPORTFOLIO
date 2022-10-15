using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Pretpark
{
    class Program
    {
        /*static void Main(string[] args)
        {
            TcpListener server = new TcpListener(new System.Net.IPAddress(new byte[] { 127,0,0,1 }), 5000);
            server.Stop();

            server.Start();
            var teller = 0;
            while(true)
            {
                Socket connectie = server.AcceptSocket();
                //connectie.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

                Stream request = new NetworkStream(connectie);
                StreamReader requestLezer = new StreamReader(request);                
                string[]? regel1 = requestLezer.ReadLine()?.Split(" ");
                if (regel1 == null) return;
                (string methode, string url, string httpversie) = (regel1[0], regel1[1], regel1[2]);
                string? regel = requestLezer.ReadLine();
                int contentLength = 0;
                var useragent = "";
                var mogelijkeUserAgents = new List<string>();

                while (!string.IsNullOrEmpty(regel) && !requestLezer.EndOfStream)
                {
                    string[] stukjes = regel.Split(":");
                    (string header, string waarde) = (stukjes[0], stukjes[1]);
                    if (header.ToLower() == "content-length")
                        contentLength = int.Parse(waarde);
                    if (header.ToLower() == "user-agent")
                        useragent = waarde;
                    regel = requestLezer.ReadLine();
                }
                if (contentLength > 0)
                {
                    char[] bytes = new char[(int)contentLength];
                    requestLezer.Read(bytes, 0, (int)contentLength);
                }

                if (useragent != null)
                {
                    if (useragent.Contains("Mozilla")) mogelijkeUserAgents.Add("<li>Firefox</li>");
                    if (useragent.Contains("Chrome")) mogelijkeUserAgents.Add("<li>Chrome</li>");
                    if (useragent.Contains("Safari")) mogelijkeUserAgents.Add("<li>Safari</li>");
                    if (useragent.Contains("OPR")) mogelijkeUserAgents.Add("<li>Opera</li>");
                    if (useragent.Contains("Edg")) mogelijkeUserAgents.Add("<li>Edge</li>");
                }
                if (useragent == "") mogelijkeUserAgents.Add("Ik heb geen idee wat het zou kunnen zijn.");
                if (url == "/contact") connectie.Send(System.Text.Encoding.ASCII.GetBytes("HTTP/1.0 200 OK\r\nContent-Type: text/html\r\nContent-Length: 89\r\n\r\n<a href=\"https://nl.wikipedia.org/wiki/Den_Haag\">Welkom bij de website van niet Den Haag!</a>"));
                else if (url == "/teller")
                {
                    teller++;
                    connectie.Send(System.Text.Encoding.ASCII.GetBytes($"HTTP/1.0 200 OK\r\nContent-Type: text/html\r\nContent-Length: {57 + teller.ToString().Count()}\r\n\r\n<a href=\"https://nl.wikipedia.org/wiki/Den_Haag\">teller: {teller}</a>"));
                }
                else if (url == "/add?a=3&b=4")
                {
                    connectie.Send(System.Text.Encoding.ASCII.GetBytes($"HTTP/1.0 200 OK\r\nContent-Type: text/html\r\nContent-Length: 1\r\n\r\n7"));
                }
                else if (url.Contains("mijnteller")) connectie.Send(System.Text.Encoding.ASCII.GetBytes("HTTP/1.0 200 OK\r\nContent-Type: text/html\r\nContent-Length: 103\r\n\r\n<a>De teller staat op 1, klik </a><a href=\"http://localhost:5000/tellerstand2\">hier</a> om te verhogen."));
                else if (url.Contains("tellerstand"))
                {
                    var stand = Int32.Parse(url.Remove(0, 12));

                    connectie.Send(System.Text.Encoding.ASCII.GetBytes($"HTTP/1.0 200 OK\r\nContent-Type: text/html\r\nContent-Length: {103 + stand.ToString().Count()}\r\n\r\n<a>De teller staat op {stand}, klik </a><a href=\"http://localhost:5000/tellerstand{stand + 1}\">hier</a> om te verhogen."));
                }
                else if (url == "/")
                {
                    var body = " <h1><a href=\\\"http://localhost:5000/contact\\\">ContactPagina</a></h1> <ul><h2>Je gebruikt waarschijnlijk een van deze browsers</h2>";
                    foreach(var i in mogelijkeUserAgents)
                    {
                        body+=i;
                    }
                    body += "</ul>";
                    connectie.Send(System.Text.Encoding.ASCII.GetBytes($"HTTP/1.0 404 Page Not Found\r\nContent-Type: text/html\r\nContent-Length: {body.Length}\r\n\r\n" +body));
                }
                else connectie.Send(Encoding.ASCII.GetBytes("HTTP/1.0 404 Page Not Found\r\nContent-Type: text/html\r\nContent-Length: 20\r\n\r\nPagina Niet Gevonden"));
            }
            server.Stop();
        }*/
    }
}