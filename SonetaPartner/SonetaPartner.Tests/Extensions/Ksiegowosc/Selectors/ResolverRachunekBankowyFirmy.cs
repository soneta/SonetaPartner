using JetBrains.Annotations;
using Soneta.Business;
using Soneta.Kasa;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Settings;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors
{
    public sealed class ResolverRachunekBankowyFirmy
    {
        private string _selectorBySymbol;
        private SelectorRachunekBankowyFirmy _selector;

        private ResolverRachunekBankowyFirmy()
        { }

        public static implicit operator ResolverRachunekBankowyFirmy(SelectorRachunekBankowyFirmy selector)
            => new ResolverRachunekBankowyFirmy {_selector = selector};

        public static implicit operator ResolverRachunekBankowyFirmy(string selector)
            => new ResolverRachunekBankowyFirmy {_selectorBySymbol = selector};

        public RachunekBankowyFirmy Resolve(ISessionable sProvider)
        {
            if (!string.IsNullOrEmpty(_selectorBySymbol))
                return (RachunekBankowyFirmy) sProvider.Finder().EwidencjaSP(_selectorBySymbol);

            switch (_selector)
            {
                case SelectorRachunekBankowyFirmy.FirmowyRachunekBankowy:
                    return (RachunekBankowyFirmy) sProvider.Finder().EwidencjaSP(EwidencjaSP.RachunekBankowy);
                case SelectorRachunekBankowyFirmy.FirmowyRachunekBankowyEuro:
                    return (RachunekBankowyFirmy) sProvider.Finder().EwidencjaSP(Defaults.SymbolRachunekEuro);
            }

            throw TestException.MakeEnumOutOfRange(_selector, "Invalid selector");
        }
    }
}
