using JetBrains.Annotations;
using Soneta.SrodkiTrwale;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors
{
    public sealed class ResolverRodzajST
    {
        private string _selectorStr;

        private ResolverRodzajST()
        { }

        public static implicit operator ResolverRodzajST(string selector)
            => new ResolverRodzajST {_selectorStr = selector};

        public RodzajST Resolve(SrodekTrwalyBase sProvider)
            => sProvider.Session.Finder().RodzajST(sProvider.Typ, _selectorStr);
    }
}
