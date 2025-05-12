using JetBrains.Annotations;
using Soneta.Business;
using Soneta.Core;
using Soneta.Ksiega;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors
{
	public sealed class ResolverKonto
	{
		private string _selectorStr;
		private OddzialFirmy _firma;

		private ResolverKonto()
		{ }

		public static implicit operator ResolverKonto(string selector)
			=> new ResolverKonto { _selectorStr = selector };

		public static implicit operator ResolverKonto((string konto, OddzialFirmy firma) selector)
			=> new ResolverKonto { _selectorStr = selector.konto, _firma = selector.firma };

		public KontoBase Resolve(ISessionable sProvider)
			=> sProvider.Finder().Konto(_selectorStr, sProvider.Session.Get(_firma));

		public KontoBase Resolve(OkresObrachunkowy okres)
			=> okres.Finder().Konto(okres, _selectorStr, okres.Session.Get(_firma));
	}
}
