using System;
using System.Net;
using System.Net.Sockets;

using System.Threading;
using System.Threading.Tasks;

using System.Text;
using System.Reflection;

namespace dotnet_async_learning
{
    internal class EchoServer
    {
        internal EchoServer(string ipAddr, UInt16 port)
        {
            _tcpListener = new TcpListener(IPAddress.Parse(ipAddr), port);
        }
        internal async Task Run()
        {
            _tcpListener.Start();
            while (true)
            {
                Console.WriteLine("Run, Thread: {0}", Thread.CurrentThread.ManagedThreadId);
                TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync();
                EndPoint endPoint = tcpClient.Client.RemoteEndPoint;
                Console.WriteLine("Connected: {0}", endPoint);
                ProcessConnection(tcpClient.GetStream(), endPoint);
            }
        }
        private async void ProcessConnection(NetworkStream netStream, EndPoint endPoint)
        {
            Console.WriteLine("ProcessConnection 1, Thread: {0}", Thread.CurrentThread.ManagedThreadId);
            while (true)
            {
                Console.WriteLine("ProcessConnection 2, Thread: {0}", Thread.CurrentThread.ManagedThreadId);
                byte [] buffer = new byte [1024];
                Int32 readedLength = await netStream.ReadAsync(buffer, 0, buffer.Length);
                if (readedLength == 0)
                {
                    netStream.Close();
                    Console.WriteLine("Closed: {0}", endPoint);
                    break;
                }
                string text = Encoding.UTF8.GetString(buffer, 0, readedLength);
                Console.WriteLine("Readed: {0}", text);
                await netStream.WriteAsync(buffer, 0, readedLength);
            }
        }
        private TcpListener _tcpListener;
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: {0} [port]", Assembly.GetCallingAssembly().GetName().Name);
                return;
            }

            EchoServer echoServer = new EchoServer("0.0.0.0", UInt16.Parse(args[0]));
            Task echoServerTask = echoServer.Run();
            echoServerTask.Wait();
        }
    }
}
