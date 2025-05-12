using JetBrains.Annotations;
using Soneta.Business;
using Soneta.Core;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors
{
	public sealed class ResolverCentrumKosztow
	{
		private SelectorCentrumKosztow _selector;
		private string _selectorStr;

		private ResolverCentrumKosztow()
		{ }

		public static implicit operator ResolverCentrumKosztow(SelectorCentrumKosztow selector)
			=> new ResolverCentrumKosztow { _selector = selector };

		public static implicit operator ResolverCentrumKosztow(string selector)
			=> new ResolverCentrumKosztow { _selectorStr = selector };

		public CentrumKosztow Resolve(ISessionable sProvider)
		{
			if (!string.IsNullOrEmpty(_selectorStr))
				return sProvider.Finder().CentrumKosztow(_selectorStr);

			switch (_selector)
			{
				case SelectorCentrumKosztow.Firma:
					return sProvider.Finder().StdCentrumKosztow();

				case SelectorCentrumKosztow.KosztyHandlowe:
					return sProvider.Finder().CentrumKosztow("Koszty handlowe");
				case SelectorCentrumKosztow.KosztySprzedazy:
					return sProvider.Finder().CentrumKosztow("Koszty sprzedaży");
				case SelectorCentrumKosztow.KosztyZarzadu:
					return sProvider.Finder().CentrumKosztow("Koszty zarządu");
				case SelectorCentrumKosztow.KosztyDzialalnoscPomocnicza:
					return sProvider.Finder().CentrumKosztow("Koszty działalności pomoc.");
				case SelectorCentrumKosztow.KosztyDzialalnoscPodstawowa:
					return sProvider.Finder().CentrumKosztow("Koszty działalności podstaw.");
				case SelectorCentrumKosztow.KosztyWydzialowe:
					return sProvider.Finder().CentrumKosztow("Koszty wydziałowe");

				default:
					throw TestException.MakeEnumOutOfRange(_selector, "Invalid selector");
			}
		}
	}
}
