using JetBrains.Annotations;
using Soneta.Business;
using Soneta.CRM;
using Soneta.Kadry;
using Soneta.Kasa;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors
{
	public sealed class ResolverPodmiot
	{
		private readonly string _kod;
		private readonly Kontrahent _kontrahent;
		private readonly Pracownik _pracownik;
		private readonly SelectorPodmiot? _selector;
		private readonly SelectorKontrahent? _selectorKon;

		private ResolverPodmiot(string kod = null, SelectorPodmiot? selector = null, SelectorKontrahent? selectorKon = null, Kontrahent kontrahent = null, Pracownik pracownik = null)
		{
			_kod = kod;
			_kontrahent = kontrahent;
			_pracownik = pracownik;
			_selector = selector;
			_selectorKon = selectorKon;
		}

		public static implicit operator ResolverPodmiot(string kod)
			=> new ResolverPodmiot(kod);

		public static implicit operator ResolverPodmiot(Kontrahent kontrahent)
			=> new ResolverPodmiot(kontrahent: kontrahent);

		public static implicit operator ResolverPodmiot(Pracownik pracownik)
			=> new ResolverPodmiot(pracownik: pracownik);

		public static implicit operator ResolverPodmiot(SelectorPodmiot selector)
			=> new ResolverPodmiot(selector: selector);

		public static implicit operator ResolverPodmiot(SelectorKontrahent selectorKon)
			=> new ResolverPodmiot(selectorKon: selectorKon);

		public IPodmiotKasowy Resolve(ISessionable sProvider)
		{
			if (_kontrahent != null)
				return sProvider.InSession(_kontrahent);

			if (_pracownik != null)
				return sProvider.InSession(_pracownik);

			if (!string.IsNullOrEmpty(_kod))
				return sProvider.Finder().Kontrahent(_kod);

			if (_selector != null)
				switch (_selector.Value)
				{
					case SelectorPodmiot.Abc:
					case SelectorPodmiot.Aspen:
					case SelectorPodmiot.Blanc:
					case SelectorPodmiot.Drynda:
					case SelectorPodmiot.Fiszbin:
					case SelectorPodmiot.Gawron:
					case SelectorPodmiot.Hiacynt:
					case SelectorPodmiot.Kobra:
					case SelectorPodmiot.Rolmap:
					case SelectorPodmiot.Seno:
					case SelectorPodmiot.Klon:
					case SelectorPodmiot.Zefir:
						return sProvider.Finder().Kontrahent(_selector.Value.ToString());

					case SelectorPodmiot.Andrzejewski:
						return sProvider.Finder().Pracownik("006");
					case SelectorPodmiot.Bednarek:
						return sProvider.Finder().Pracownik("007");
					case SelectorPodmiot.Bujak:
						return sProvider.Finder().Pracownik("008");

					case SelectorPodmiot.KAS:
						return sProvider.Finder().StdKAS();

					case SelectorPodmiot.ZUS:
						return sProvider.Finder().StdZUS();

					case SelectorPodmiot.Incydentalny:
						return sProvider.Finder().StdKontrahentIncydentalny();

					case SelectorPodmiot.UrzadCelny:
						return sProvider.Finder().StdUrzadCelny();

					default:
						throw TestException.MakeEnumOutOfRange(_selector.Value, "Invalid selector");
				}

			if (_selectorKon != null)
				switch (_selectorKon.Value)
				{
					case SelectorKontrahent.Abc:
					case SelectorKontrahent.Aspen:
					case SelectorKontrahent.Drynda:
					case SelectorKontrahent.Gawron:
					case SelectorKontrahent.Fiszbin:
					case SelectorKontrahent.Hiacynt:
					case SelectorKontrahent.Rolmap:
					case SelectorKontrahent.Zefir:
						return sProvider.Finder().Kontrahent(_selectorKon.Value.ToString());

					default:
						throw TestException.MakeEnumOutOfRange(_selectorKon.Value, "Invalid selector");
				}

			throw new TestException("Resolver PodmiotKasowy: no input data");
		}
	}
}
