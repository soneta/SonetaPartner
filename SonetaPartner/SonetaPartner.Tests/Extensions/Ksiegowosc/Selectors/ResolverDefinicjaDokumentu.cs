using JetBrains.Annotations;
using Soneta.Business;
using Soneta.Core;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors
{
	public sealed class ResolverDefinicjaDokumentu
	{
		private enum Selector
		{
			Symbol,
			Definicja
		}

		private Selector? _selector;
		private string _selectorStr;
		private DefinicjaDokumentu _definicja;

		private ResolverDefinicjaDokumentu()
		{ }

		public static implicit operator ResolverDefinicjaDokumentu(string selector)
			=> new ResolverDefinicjaDokumentu
			{
				_selector = Selector.Symbol,
				_selectorStr = selector
			};

		public static implicit operator ResolverDefinicjaDokumentu(DefinicjaDokumentu definicja)
				=> new ResolverDefinicjaDokumentu
				{
					_selector = Selector.Definicja,
					_definicja = definicja
				};

		public DefinicjaDokumentu Resolve(ISessionable sProvider)
		{
			if (_selector == Selector.Definicja)
				return sProvider.InSession(_definicja);
			if (_selector == Selector.Symbol)
				return sProvider.Finder().DefinicjaDokumentu(_selectorStr);

			throw new TestException($"ResolverDefinicjaDokumentu: nieprawidłowy selector definicji dokumentu '{_selector}'.");
		}
	}
}
