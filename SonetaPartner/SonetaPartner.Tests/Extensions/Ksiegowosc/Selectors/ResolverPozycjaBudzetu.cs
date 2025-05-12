using JetBrains.Annotations;
using Soneta.Business;
using Soneta.Zadania.Budzetowanie;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;
using System.Linq;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors
{
	public sealed class ResolverPozycjaBudzetu
	{
		private string _selectorStr;

		private ResolverPozycjaBudzetu()
		{ }

		public static implicit operator ResolverPozycjaBudzetu(string selector)
			=> new ResolverPozycjaBudzetu { _selectorStr = selector };

		public PozycjaBudzProj Resolve(ISessionable sProvider)
		{
			if (string.IsNullOrEmpty(_selectorStr))
				throw new TestException("ResolverPozycjaBudzetu: empty selector");

			var selector = _selectorStr.Split('/');
			if (selector.Length != 2 && selector.Any(string.IsNullOrEmpty))
				throw new TestException("ResolverPozycjaBudzetu: invalid selector");

			return sProvider.Finder().PozycjaBudzetu(selector[0], selector[1]);
		}
	}
}
