using JetBrains.Annotations;
using Soneta.Business;
using Soneta.Core;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors
{
	public sealed class ResolverStawkaVAT
	{
		private SelectorStawkaVAT? _selector;
		private DefinicjaStawkiVat _definicja;
		private decimal? _wysokosc;

		private ResolverStawkaVAT()
		{ }

		public static implicit operator ResolverStawkaVAT(SelectorStawkaVAT selector)
			=> new ResolverStawkaVAT { _selector = selector };

		public static implicit operator ResolverStawkaVAT(DefinicjaStawkiVat definicja)
			=> new ResolverStawkaVAT { _definicja = definicja };

		public static implicit operator ResolverStawkaVAT(ProxyRecord<DefinicjaStawkiVat> definicja)
			=> new ResolverStawkaVAT { _definicja = definicja.Row };

		public static implicit operator ResolverStawkaVAT(decimal wysokosc)
			=> new ResolverStawkaVAT { _wysokosc = wysokosc };

		public DefinicjaStawkiVat Resolve(ISessionable sProvider)
		{
			if (_definicja != null)
				return sProvider.Session.Get(_definicja);

			var finder = sProvider.Finder();

			if (_selector != null)
				switch (_selector.Value)
				{
					case SelectorStawkaVAT.NiePodlega:
						return finder.StdStawkaVATNiePodlega();
					case SelectorStawkaVAT.Brak:
						return finder.StdStawkaVATBrak();
					case SelectorStawkaVAT.Podstawowa:
						return finder.StdStawkaVATPodstawowa();
					case SelectorStawkaVAT.Zwolniona:
						return finder.StdStawkaVATZwolniona();
					case SelectorStawkaVAT.Zero:
						return finder.StdStawkaVATZero();
					case SelectorStawkaVAT.Procent8:
						return finder.StdStawkaVAT8Procent();
					case SelectorStawkaVAT.Procent5:
						return finder.StdStawkaVAT5Procent();
				}

			if (_wysokosc != null)
				return finder.DefinicjaStawkiVATWgProcent(_wysokosc.Value);

			throw TestException.MakeEnumOutOfRange(_selector, "Invalid selector");
		}
	}
}
