using JetBrains.Annotations;
using Soneta.Business;
using Soneta.Kasa;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors
{
    public sealed class ResolverFormaPlatnosci
    {
        private SelectorFormaPlatnosci _selector;

        private ResolverFormaPlatnosci()
        { }

        public static implicit operator ResolverFormaPlatnosci(SelectorFormaPlatnosci selector)
            => new ResolverFormaPlatnosci {_selector = selector};

        public FormaPlatnosci Resolve(ISessionable sProvider)
        {
            switch (_selector)
            {
                case SelectorFormaPlatnosci.Gotowka:
                    return sProvider.Finder().FormaPlatnosci(FormaPlatnosci.Gotowka);
                case SelectorFormaPlatnosci.Przelew:
                    return sProvider.Finder().FormaPlatnosci(FormaPlatnosci.Przelew);
                case SelectorFormaPlatnosci.PrzelewMPP:
                    return sProvider.Finder().FormaPlatnosci(FormaPlatnosci.PrzelewMPP);

                default:
                    throw TestException.MakeEnumOutOfRange(_selector, "Invalid selector");
            }
        }
    }
}
