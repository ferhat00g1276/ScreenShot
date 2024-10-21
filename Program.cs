using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

Console.WriteLine("Server");

var ip = IPAddress.Parse("192.168.0.110");
var port = 27001;
var ep = new IPEndPoint(ip, port);
var listener = new TcpListener(ep);

void Accepter()
{
    try
    {
        listener.Start();
        while (true)
        {
            var client = listener.AcceptTcpClient();

            _ = Task.Run(() =>
            {
                var ns = client.GetStream();
                var remoteEp = client.Client.RemoteEndPoint as IPEndPoint;
                var dirName = $"{remoteEp.Address}";
                var currentDir = Environment.CurrentDirectory;
                var newDir = Path.Combine(currentDir, dirName);

                if (!Directory.Exists(newDir))
                {
                    Directory.CreateDirectory(newDir);
                }

                var path = Path.Combine(newDir, $"{DateTime.Now:dd.MM.yyyy_HH.mm.ss}.png");
                using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    int len;
                    var bytes = new byte[1024];
                    while ((len = ns.Read(bytes, 0, bytes.Length)) > 0)
                    {
                        fs.Write(bytes, 0, len);
                    }
                }
                client.Close();
            });
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

Accepter();
