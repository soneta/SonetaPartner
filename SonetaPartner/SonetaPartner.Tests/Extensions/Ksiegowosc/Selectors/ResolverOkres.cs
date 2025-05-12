using JetBrains.Annotations;
using Soneta.Business;
using Soneta.Ksiega;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors
{
    public sealed class ResolverOkres
    {
        private bool _selectorDefault;
        private int? _selectorYear;
        private OkresObrachunkowy _selectorOkres;

        private ResolverOkres()
        { }

        public static implicit operator ResolverOkres(int selector)
            => new ResolverOkres { _selectorYear = selector };

        public static implicit operator ResolverOkres(SelectorOkres selector)
            => new ResolverOkres { _selectorDefault = true };

        public static implicit operator ResolverOkres(OkresObrachunkowy okres)
            => new ResolverOkres { _selectorOkres = okres };

        public OkresObrachunkowy Resolve(ISessionable sProvider)
            => _selectorDefault ? sProvider.Finder().StdOkresObrachunkowy() :
                _selectorOkres != null ? sProvider.InSession(_selectorOkres) :
                _selectorYear != null ? sProvider.Finder().OkresObrachunkowy(_selectorYear.Value) :
                throw new TestException("ResolverOkres: invalid selector");
    }
}
