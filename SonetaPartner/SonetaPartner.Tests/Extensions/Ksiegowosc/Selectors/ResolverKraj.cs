using JetBrains.Annotations;
using Soneta.Business;
using Soneta.Core;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;
using System.Diagnostics.CodeAnalysis;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors
{
	public sealed class ResolverKraj
	{
		private bool _poland;
		private string _selectorStr;

		internal static ResolverKraj PL = new ResolverKraj { _poland = true };

		private ResolverKraj()
		{ }

		public static implicit operator ResolverKraj(string selector)
			=> new ResolverKraj { _selectorStr = selector };

		public KrajTbl Resolve(ISessionable sProvider)
		{
			if (_poland)
				return sProvider.Finder().StdKrajPoland();

			if (_selectorStr != null && _selectorStr.Length == 2)
				return sProvider.Finder().KrajWgKod2(_selectorStr);
			if (_selectorStr != null && _selectorStr.Length == 3)
				return sProvider.Finder().KrajWgKod3(_selectorStr);

			throw new TestException($"ResolverKraj: nieprawidłowy selector kraju '{_selectorStr}'.");
		}
	}
}
