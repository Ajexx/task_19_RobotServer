using System.Net;
using System.Net.Sockets;
using System.Text;

int port = 5000;

using Socket server = new Socket
    (
    AddressFamily.InterNetwork,
    SocketType.Stream,
    ProtocolType.Tcp
    );

EndPoint ipenpoint = new IPEndPoint(IPAddress.Loopback, port);

server.Bind(ipenpoint);
server.Listen(10);

Console.WriteLine($"Server started on port {port}");
Console.WriteLine("Waiting for client...");

Socket client = server.Accept();

object locker = new object();

int x = 0;
int y = 0;

byte[] buffer = new byte[1024];

bool running = true;
while (running)
{
    int recived;
    try
    {
        recived = client.Receive(buffer);
    }
    catch (Exception)
    {
        break;
    }

    if (recived == 0)
    {
        break;
    }

    var message = Encoding.UTF8.GetString(buffer, 0, recived)
        .Trim().ToUpperInvariant();

    string buff;
    switch (message)
    {
        case "UP":
            lock (locker)
            {
                y++;
                buff = $"Robot coordinates: x = {x}, y = {y}";
            }
            break;
        case "DOWN":
            lock (locker)
            {
                y--;
                buff = $"Robot coordinates: x = {x}, y = {y}";
            }
            break;
        case "LEFT":
            lock (locker)
            {
                x--;
                buff = $"Robot coordinates: x = {x}, y = {y}";
            }
            break;
        case "RIGHT":
            lock (locker)
            {
                x++;
                buff = $"Robot coordinates: x = {x}, y = {y}";
            }
            break;
        case "POSITION":
            lock (locker)
            {
                buff = $"Robot coordinates: x = {x}, y = {y}";
            }
            break;
        case "EXIT":
            buff = "Goodbye";
            running = false;
            break;

        default:
            buff = "Incorrect input!";
            break;
    }
    client.Send(Encoding.UTF8.GetBytes(buff));
}
