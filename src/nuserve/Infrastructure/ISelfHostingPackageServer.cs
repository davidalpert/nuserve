using System;

namespace nuserve.Infrastructure
{
    /// <summary>
    /// The basic interface for a <see cref="SelfHostingPackageServer"/> used
    /// by TopShelf to manage the self-hosting server.
    /// </summary>
    public interface ISelfHostingPackageServer
    {
        void Start();
        void Stop();
        void Pause();
        void Continue();

        bool IsListening { get; }
    }
}
