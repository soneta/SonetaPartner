using Soneta.Business;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Engine
{
    public sealed class ProxySettings<T> : ISessionable
        where T : Module
    {
        internal T Module { get; }


        public Session Session
            => Module.Session;


        public ProxySettings(T module)
            => Module = module;
    }
}
