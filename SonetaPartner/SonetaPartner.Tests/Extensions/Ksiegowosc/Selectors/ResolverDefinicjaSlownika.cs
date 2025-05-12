using System;
using JetBrains.Annotations;
using Soneta.Business;
using Soneta.Ksiega;


namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;
    public sealed class ResolverDefinicjaSlownika
    {
        private Guid _selectorGuid;

        private ResolverDefinicjaSlownika()
        { }

        public static implicit operator ResolverDefinicjaSlownika(Guid selector)
            => new ResolverDefinicjaSlownika {_selectorGuid = selector};

        public DefinicjaSlownika Resolve(ISessionable sProvider)
            => sProvider.Finder().DefinicjaSlownika(_selectorGuid);
    }

