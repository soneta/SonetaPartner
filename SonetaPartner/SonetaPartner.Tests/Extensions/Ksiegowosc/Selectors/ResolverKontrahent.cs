using Soneta.Business;
using Soneta.CRM;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors
{
	public sealed class ResolverKontrahent
	{
		private readonly SelectorKontrahent? _selector;
		private readonly string _kod;

		private ResolverKontrahent(SelectorKontrahent? selector = null, string kod = null)
		{
			_selector = selector;
			_kod = kod;
		}

		public static implicit operator ResolverKontrahent(SelectorKontrahent selector)
			=> new ResolverKontrahent(selector);

		public Kontrahent Resolve(ISessionable sProvider)
		{
			if (!string.IsNullOrEmpty(_kod))
				return sProvider.Finder().Kontrahent(_kod);

			if (_selector != null)
				switch (_selector.Value)
				{
					case SelectorKontrahent.Abc:
					case SelectorKontrahent.Aspen:
					case SelectorKontrahent.Blanc:
					case SelectorKontrahent.Drynda:
					case SelectorKontrahent.Fiszbin:
					case SelectorKontrahent.Gawron:
					case SelectorKontrahent.Hiacynt:
					case SelectorKontrahent.Klon:
					case SelectorKontrahent.Kobra:
					case SelectorKontrahent.Rolmap:
					case SelectorKontrahent.Zefir:
					case SelectorKontrahent.Seno:
						return sProvider.Finder().Kontrahent(_selector.Value.ToString());

					case SelectorKontrahent.Incydentalny:
						return sProvider.Finder().StdKontrahentIncydentalny();

					default:
						throw TestException.MakeEnumOutOfRange(_selector.Value, "Invalid selector");
				}

			throw new TestException("Resolver Kontrahent: no input data");
		}
	}
}
