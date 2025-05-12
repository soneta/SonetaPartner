using Soneta.Business;
using Soneta.Kadry;
using Soneta.Place;
using Soneta.Test;
using Soneta.Types;
using System;
using System.Linq;

namespace SonetaPartner.Tests.Assemblers
{
	public static class NaliczanieWyplatyAssembler
	{
		static public IRowBuilderCollection<TPracownik, Wyplata> NaliczWyplaty<TPracownik>(this IRowBuilder<TPracownik> builder, string okres, Method<NaliczanieSeryjne.PracownikParams> paramsUpdate = null) where TPracownik : Pracownik
		{
			return builder.NaliczWyplaty(okres, null, null, 0, paramsUpdate);
		}

		static public IRowBuilderCollection<TPracownik, Wyplata> NaliczWyplaty<TPracownik>(this IRowBuilder<TPracownik> builder, string okres, string dataWypłaty, Method<NaliczanieSeryjne.PracownikParams> paramsUpdate = null) where TPracownik : Pracownik
		{
			return builder.NaliczWyplaty(okres, dataWypłaty, null, 0, paramsUpdate);
		}

		static public IRowBuilderCollection<TPracownik, Wyplata> NaliczWyplaty<TPracownik>(this IRowBuilder<TPracownik> builder, string okres, string dataWypłaty, string miesiącZUS, int miesięcyWstecz, Method<NaliczanieSeryjne.PracownikParams> paramsUpdate = null, string dataListy = null) where TPracownik : Pracownik
		{

			return builder.GetChildren<Wyplata>((pracownik, ctx) => {
				var pars = new NaliczanieSeryjne.PracownikParams(ctx);

				var okr = FromTo.Parse(okres);

				pars.DataWypłaty = pars.DataListy = dataWypłaty != null ? Date.Parse(dataWypłaty) : okr.To;
				if (dataListy != null) pars.DataListy = Date.Parse(dataListy);

				if (miesiącZUS != null)
				{
					pars.EdycjaMiesiącaZUS = true;
					pars.MiesiącZUS = YearMonth.Parse(miesiącZUS);
				}
				else
					pars.EdycjaMiesiącaZUS = false;

				pars.Naliczanie = TypNaliczenia.PłatnaZDołu;
				pars.MiesWstecz = miesięcyWstecz;
				pars.Okres = okr;

				pars.TypWypłaty = TypWyplaty.Wszystkie;
				pars.DefinicjaListy = null;
				pars.Wydział = null;
				pars.Seria = "";
				pars.Dodatek = null;

				paramsUpdate?.Invoke(pars);

				var naliczanie = new NaliczanieSeryjne.Pracownika(pars);
				naliczanie.Pracownik = pracownik;

				NaliczanieWypłat nw = naliczanie.Nalicz();
				return nw.WszystkieWypłaty;
			});
		}

		static public IRowBuilderCollection<Umowa, Wyplata> NaliczUmowe(this IRowBuilder<Umowa> builder, string okres, string dataWypłaty, string miesiącZUS, int miesięcyWstecz, Method<NaliczanieSeryjne.UmowaParams> paramsUpdate = null, string dataListy = null)
			=> builder.GetChildren<Wyplata>((umowa, ctx) => {

				var pars = new NaliczanieSeryjne.UmowaParams(ctx);
				Update(pars, okres, dataWypłaty, miesiącZUS, miesięcyWstecz, dataListy);
				paramsUpdate?.Invoke(pars);

				var naliczanie = new NaliczanieSeryjne.Umowy(pars);
				naliczanie.Umowa = umowa;

				NaliczanieWypłat nw = naliczanie.Nalicz();
				return nw.Wypłaty;
			});

		static public IRowBuilderCollection<Dodatek, Wyplata> NaliczDodatek(this IRowBuilder<Dodatek> builder, string okres, string dataWypłaty, string miesiącZUS, int miesięcyWstecz, Method<NaliczanieSeryjne.DodatkiParams> paramsUpdate = null, string dataListy = null)
			=> builder.GetChildren<Wyplata>((dodatek, ctx) => {
				var pars = new NaliczanieSeryjne.DodatkiParams(ctx);
				Update(pars, okres, dataWypłaty, miesiącZUS, miesięcyWstecz, dataListy);
				paramsUpdate?.Invoke(pars);

				var naliczanie = new NaliczanieSeryjne.Dodatki(pars)
				{
					Dodatek = dodatek
				};
				NaliczanieWypłat nw = naliczanie.Nalicz();
				return nw.Wypłaty;
			});

		static void Update(NaliczanieSeryjne.Params pars, string okres, string dataWypłaty, string miesiącZUS, int miesięcyWstecz, string dataListy)
		{
			var okr = FromTo.Parse(okres);

			pars.DataWypłaty = pars.DataListy = dataWypłaty != null ? Date.Parse(dataWypłaty) : okr.To;
			if (dataListy != null) pars.DataListy = Date.Parse(dataListy);

			if (miesiącZUS != null)
			{
				pars.EdycjaMiesiącaZUS = true;
				pars.MiesiącZUS = YearMonth.Parse(miesiącZUS);
			}
			else
				pars.EdycjaMiesiącaZUS = false;

			pars.Okres = okr;
			pars.MiesWstecz = miesięcyWstecz;

			pars.DefinicjaListy = null;
			pars.Wydział = null;
			pars.Seria = "";
		}

		#region obsolete

		static public IRowBuilderCollection<TPracownik, Wyplata> _NaliczWyplaty<TPracownik>(this IRowBuilder<TPracownik> builder, FromTo okres, Date dataWypłaty, Method<NaliczanieSeryjne.PracownikParams> paramsUpdate = null) where TPracownik : Pracownik
		{
			return builder._NaliczWyplaty(okres, dataWypłaty, YearMonth.Empty, paramsUpdate);
		}

		static public IRowBuilderCollection<TPracownik, Wyplata> _NaliczWyplaty<TPracownik>(this IRowBuilder<TPracownik> builder, FromTo okres, Date dataWypłaty, YearMonth miesiącZUS, Method<NaliczanieSeryjne.PracownikParams> paramsUpdate = null) where TPracownik : Pracownik
		{
			return builder.NaliczWyplaty(okres.ToString(),
				dataWypłaty == Date.Empty ? null : dataWypłaty.ToString(),
				miesiącZUS == YearMonth.Empty ? null : miesiącZUS.ToString(),
				0,
				paramsUpdate);
		}

		#endregion
	}
}
