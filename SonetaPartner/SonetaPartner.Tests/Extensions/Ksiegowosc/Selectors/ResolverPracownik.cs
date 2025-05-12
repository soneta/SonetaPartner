using JetBrains.Annotations;

using Soneta.Business;
using Soneta.Kadry;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors
{
    public sealed class ResolverPracownik
    {
        private string _selectorStr;

        private ResolverPracownik()
        { }

        public static implicit operator ResolverPracownik(string selector)
            => new ResolverPracownik {_selectorStr = selector};

        public Pracownik Resolve(ISessionable sProvider)
            => sProvider.Finder().Pracownik(_selectorStr);
    }
}
