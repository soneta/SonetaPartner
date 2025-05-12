using JetBrains.Annotations;
using Soneta.Business;
using Soneta.Kasa;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Settings;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors
{
    public sealed class ResolverEwidencjaSP
    {
        private string _selectorBySymbol;
        private SelectorEwidencjaSP? _selector;

        private ResolverEwidencjaSP()
        { }

        public static implicit operator ResolverEwidencjaSP(SelectorEwidencjaSP selector)
            => new ResolverEwidencjaSP {_selector = selector};

        public static implicit operator ResolverEwidencjaSP(string selector)
            => new ResolverEwidencjaSP {_selectorBySymbol = selector};

        public EwidencjaSP Resolve(ISessionable sProvider)
        {
            if (!string.IsNullOrEmpty(_selectorBySymbol))
                return sProvider.Finder().EwidencjaSP(_selectorBySymbol);

            switch (_selector)
            {
                case SelectorEwidencjaSP.FirmowyRachunekBankowy:
                    return sProvider.Finder().EwidencjaSP(EwidencjaSP.RachunekBankowy);
                case SelectorEwidencjaSP.FirmowyRachunekBankowyEuro:
                    return sProvider.Finder().EwidencjaSP(Defaults.SymbolRachunekEuro);
                case SelectorEwidencjaSP.KasaGotowkowa:
                    return sProvider.Finder().EwidencjaSP(EwidencjaSP.Kasa);
            }

            throw TestException.MakeEnumOutOfRange(_selector, "Invalid selector");
        }
    }
}
