using Soneta.Business;
using Soneta.Windykacja;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Engine
{
    public sealed class Deleter : ISessionable
    {
        public Session Session { get; }

        public Deleter(Session session)
            => Session = session;

    }
}
