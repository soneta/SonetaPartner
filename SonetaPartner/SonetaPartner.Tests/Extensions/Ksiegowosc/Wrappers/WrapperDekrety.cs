using Soneta.Core;
using Soneta.Ksiega;
using System.Collections.Generic;
using System.Linq;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Wrappers
{
	public sealed class WrapperDekrety
	{
		private readonly List<DekretBase> _dekrety;
		private readonly List<ZapisKsiegowy> _zapisy;

		private WrapperDekrety(IEnumerable<DekretBase> dekrety)
		{
			_dekrety = dekrety.ToList();
			_zapisy = _dekrety.SelectMany(λ => λ.Zapisy).ToList();
		}

		public WrapperDekrety(ManagerKsiegowan.Rezultat result)
			: this(result.Dekrety.Cast<DekretBase>())
		{ }

		public int DCount
			=> _dekrety.Count;

		public int ZCount
			=> _zapisy.Count;

		public IEnumerable<ZapisKsiegowy> Select(string konto = null, StronaKsiegowania strona = StronaKsiegowania.Brak, decimal? kwota = null, string numerDokumentu = null, string opis = null)
		{
			return _zapisy.Where(IsMatch);

			bool IsMatch(ZapisKsiegowy zapis)
				=> (konto == null || zapis.Konto.Symbol == konto) &&
				   (strona == StronaKsiegowania.Brak || zapis.Strona == strona) &&
				   (kwota == null || zapis.KwotaOperacji.Value == kwota) &&
				   (numerDokumentu == null || zapis.NumerDokumentu == numerDokumentu) &&
				   (opis == null || zapis.Opis == opis);
		}
	}
}
