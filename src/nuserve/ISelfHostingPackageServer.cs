using System;

namespace nuserve
{
    public interface ISelfHostingPackageServer
    {
        void Start();
        void Stop();
        void Pause();
        void Continue();

        bool IsListening { get; }
    }
}
