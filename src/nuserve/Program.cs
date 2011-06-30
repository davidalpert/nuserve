using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topshelf;
using Topshelf.Configuration.Dsl;
using Kayak;
using Kayak.Http;
using System.Net;
using System.Diagnostics;

namespace nuserve
{
    public class Program
    {
        void Main(string[] args)
        {
            var cfg = RunnerConfigurator.New(x =>
            {
                x.Service<Server>(s =>
                {
                    s.SetServiceName("nuserve");
                    s.ConstructUsing(name => new Server());
                    s.WhenStarted(ns => ns.Start());
                    s.WhenPaused(ns => ns.Pause());
                    s.WhenContinued(ns => ns.Continue());
                    s.WhenStopped(ns => ns.Stop());
                });

                x.RunAsLocalSystem();
            });

            Runner.Host(cfg, args);
        }
    }

    public class SchedulerDelegate : ISchedulerDelegate
    {
        public void OnException(IScheduler scheduler, Exception e)
        {
            Console.WriteLine("Error on scheduler.");
            e.DebugStacktrace();
        }

        public void OnStop(IScheduler scheduler)
        {
            Console.WriteLine("Stop on scheduler.");
        }
    }

    public class BufferedProducer : IDataProducer
    {
        ArraySegment<byte> data;

        public BufferedProducer(string data) : this(data, Encoding.UTF8) { }
        public BufferedProducer(string data, Encoding encoding) : this(encoding.GetBytes(data)) { }
        public BufferedProducer(byte[] data) : this(new ArraySegment<byte>(data)) { }
        public BufferedProducer(ArraySegment<byte> data)
        {
            this.data = data;
        }

        public IDisposable Connect(IDataConsumer channel)
        {
            // null continuation, consumer must swallow the data immediately.
            channel.OnData(data, null);
            channel.OnEnd();
            return null;
        }
    }

    public class BufferedConsumer : IDataConsumer
    {
        List<ArraySegment<byte>> buffer = new List<ArraySegment<byte>>();
        Action<string> resultCallback;
        Action<Exception> errorCallback;

        public BufferedConsumer(Action<string> resultCallback,
    Action<Exception> errorCallback)
        {
            this.resultCallback = resultCallback;
            this.errorCallback = errorCallback;
        }
        public bool OnData(ArraySegment<byte> data, Action continuation)
        {
            // since we're just buffering, ignore the continuation. 
            // TODO: place an upper limit on the size of the buffer. 
            // don't want a client to take up all the RAM on our server! 
            buffer.Add(data);
            return false;
        }
        public void OnError(Exception error)
        {
            errorCallback(error);
        }

        public void OnEnd()
        {
            // turn the buffer into a string. 
            // 
            // (if this isn't what you want, you could skip 
            // this step and make the result callback accept 
            // List<ArraySegment<byte>> or whatever) 
            // 
            var str = buffer
                .Select(b => Encoding.UTF8.GetString(b.Array, b.Offset, b.Count))
                .Aggregate((result, next) => result + next);

            resultCallback(str);
        }
    }

    public class HttpRequestDelegate : IHttpRequestDelegate
    {
        public void OnRequest(HttpRequestHead head, IDataProducer requestBody, IHttpResponseDelegate response)
        {
            Console.WriteLine("Request on scheduler.");

            var headers = new HttpResponseHead()
            {
                Status = "200 OK",
                Headers = new Dictionary<string, string>() 
                    {
                        { "Content-Type", "text/plain" },
                    }
            };
            var body = new BufferedProducer("Hello world.\r\nHello.");

            response.OnResponse(headers, body);
        }
    }

    public class Server
    {
        public void Start()
        {
            var port = 5555;
            var scheduler = new KayakScheduler(new SchedulerDelegate());
            scheduler.Post(() =>
            {
                KayakServer.Factory
                    .CreateHttp(new HttpRequestDelegate())
                    .Listen(new IPEndPoint(IPAddress.Any, port));
            });
            Console.WriteLine("nuserve started: listening on port {0}", port);

            scheduler.Start();
        }

        public void Stop()
        {
            Console.WriteLine("nuserve stopped.");
        }

        public void Pause()
        {
            Console.WriteLine("nuserve paused.");
        }

        public void Continue()
        {
            Console.WriteLine("nuserve resumed.");
        }
    }
}
