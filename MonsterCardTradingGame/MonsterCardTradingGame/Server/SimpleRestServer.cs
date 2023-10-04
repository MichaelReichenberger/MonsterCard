using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Net;
using System.Text;
namespace MonsterCardTradingGame.Server
{
    internal class SimpleRestServer
    {
        private readonly HttpListener _listener = new HttpListener();

        public SimpleRestServer(string[] prefixes)
        {
            if (!HttpListener.IsSupported)
            {
                throw new NotSupportedException("Der HttpListener wird nicht unterstützt");
            }

            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException(" No prefixes");

            foreach (string s in prefixes)
            {
                _listener.Prefixes.Add(s);
            }
            _listener.Start();
        }

        public void Run()
        {
            Console.WriteLine("Listening...");
            while (_listener.IsListening)
            {
                var context = _listener.GetContext();
                ProcessRequest(context);
            }
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            string responseText;
            if (context.Request.Url.AbsolutePath == "/hello")
            {
                responseText = "Hallo, Welt!";
            }
            else
            {
                responseText = "Nicht gefunden";
                context.Response.StatusCode = 404;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(responseText);
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}
