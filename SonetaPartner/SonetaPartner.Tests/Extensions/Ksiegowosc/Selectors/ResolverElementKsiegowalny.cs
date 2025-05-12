using Soneta.Business;
using Soneta.Core;
using Soneta.Kasa;
using Soneta.Ksiega;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors
{
	public sealed class ResolverElementKsiegowalny
	{
		private readonly SelectorElementKsiegowalny _selector;

		private ResolverElementKsiegowalny(SelectorElementKsiegowalny selector)
			=> _selector = selector;

		public static implicit operator ResolverElementKsiegowalny(SelectorElementKsiegowalny ksiegowalny)
			=> new ResolverElementKsiegowalny(ksiegowalny);

		public IElementKsiegowalny Resolve(Row resolutionContext)
		{
			if (resolutionContext is ZapisKsiegowy zapis)
			{
				if (_selector == SelectorElementKsiegowalny.PlatnoscEwidencji)
					return zapis.Dekret.Ewidencja.Platnosci.InCollection<Platnosc>();
				if (_selector == SelectorElementKsiegowalny.ZobowiazanieEwidencji)
					return zapis.Dekret.Ewidencja.Platnosci.InCollection<Zobowiazanie>(skipOtherTypes: true);
				if (_selector == SelectorElementKsiegowalny.NaleznoscEwidencji)
					return zapis.Dekret.Ewidencja.Platnosci.InCollection<Naleznosc>(skipOtherTypes: true);
			}

			throw new TestException($"Resolver ElementKsiegowany: invalid selector ({_selector}, {resolutionContext}).");
		}
	}
}
