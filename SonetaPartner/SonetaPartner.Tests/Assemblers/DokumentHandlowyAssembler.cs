using Microsoft.Extensions.DependencyInjection;
using Soneta.Core;
using Soneta.Handel;
using Soneta.Handel.RelacjeDokumentow.Api;
using Soneta.Test;
using Soneta.Tools;
using Soneta.Types;
using System.Linq;

namespace SonetaPartner.Tests.Assemblers
{
	public static class DokumentHandlowyAssembler
	{
		public static IRowBuilder<DokumentHandlowy> Korekta(
			DokumentHandlowy korygowany) => Nowy(korygowany).GetChild(
				(d, tc) =>
				{
					var apiRelacje = d.Session.GetRequiredService<IRelacjeService>();
					return apiRelacje.NowaKorekta(new[] { d })[0];
				});

		public static IRowBuilder<TD> Zatwierdz<TD>(
			this IRowBuilder<TD> builder) where TD : DokumentHandlowy
			=> builder.Enqueue(
				d => d.Stan = StanDokumentuHandlowego.Zatwierdzony);

		public static IRowBuilder<PozycjaDokHandlowego> Pozycja(
			this IRowBuilder<DokumentHandlowy> builder, string kod) => 
				builder.GetChild<PozycjaDokHandlowego>(builderOptions: BuilderOptions.SetResultIntoContext_No)
				.Towar(kod);

		internal static IRowBuilder<TD> Kontrahent<TD>(
			this IRowBuilder<TD> builder,
			string kod)
			where TD : DokumentHandlowy => builder.Enqueue(
				d => d.Kontrahent = d.Module.CRM.Kontrahenci.WgKodu[kod]);

		internal static IRowBuilder<TD> Odbiorca<TD>(
			this IRowBuilder<TD> builder,
			string kod)
			where TD : DokumentHandlowy => builder.Enqueue(
				d => d.Odbiorca = d.Module.CRM.Kontrahenci.WgKodu[kod]);

		internal static IRowBuilder<PozycjaDokHandlowego> Pozycja(
			this IRowBuilder<DokumentHandlowy> builder,
			int ident) => builder.GetChild(
				(d, tc) => d.PozycjaWgIdent(ident),
				alternativeBuildOptions: BuildActionOptions.CommitUI_No,
				builderOptions: BuilderOptions.SetResultIntoContext_No);

		static IRowBuilder<DokumentHandlowy> Nowy(DokumentHandlowy dokument)
			=> new RowBuilder<DokumentHandlowy>(dokument);

		internal static IRowBuilder<DokumentHandlowy> Data(
			this IRowBuilder<DokumentHandlowy> builder,
			Date data) => builder.Enqueue(d => d.Data = data);

		internal static IRowBuilder<DokumentHandlowy> DataOtrzymania(
			this IRowBuilder<DokumentHandlowy> builder,
			Date data) => builder.Enqueue(d => d.Obcy.DataOtrzymania = data);

		internal static IRowBuilder<DokumentHandlowy> MagazynDo(
			this IRowBuilder<DokumentHandlowy> builder,
			string magazynDocelowy) => builder.Enqueue(
				d =>
					d.MagazynDo =
						d.Module.Magazyny.Magazyny.WgSymbol[magazynDocelowy
							]);

		internal static IRowBuilder<DokumentHandlowy> CenaNaPodrzedny(
			this IRowBuilder<DokumentHandlowy> builder)
			=> builder.Enqueue(d => d.CenaNaPodrzedny = true);

		internal static IRowBuilder<DokumentHandlowy> DataOperacji(
			this IRowBuilder<DokumentHandlowy> builder,
			Date data) => builder.Enqueue(d => d.DataOperacji = data);

		internal static IRowBuilder<DokumentHandlowy> CyklUmowy(
		  this IRowBuilder<DokumentHandlowy> builder,
		  DefinicjaCykluRodzajTerminu rodzajTerminu,
		  DefinicjaCykluTyp value = DefinicjaCykluTyp.Miesieczny,
		  int dzienMiesiaca = 1,
		  DefinicjaCykluPozycjaDnia pozycjaDnia =
			DefinicjaCykluPozycjaDnia.Pierwszy,
		  DefinicjaCykluOkresCyklu okresCyklu = DefinicjaCykluOkresCyklu.Biezacy,
		  DefinicjaCykluSposobNaDniWolne dniWolne =
			DefinicjaCykluSposobNaDniWolne.None)
		=>
		builder.Enqueue(
		  d =>
			d.UmowaInfo.Cykl.Set(
			  value,
			  rodzajTerminu,
			  dzienMiesiaca,
			  pozycjaDnia,
			  okresCyklu,
			  dniWolne));

		internal static IRowBuilder<DokumentHandlowy> CyklDostawy(
		  this IRowBuilder<DokumentHandlowy> builder,
		  DefinicjaCykluRodzajTerminu rodzajTerminu,
		  DefinicjaCykluTyp value,
		  int dzienMiesiaca = 1,
		  DefinicjaCykluPozycjaDnia pozycjaDnia =
			DefinicjaCykluPozycjaDnia.Pierwszy,
		  DefinicjaCykluOkresCyklu okresCyklu = DefinicjaCykluOkresCyklu.Biezacy,
		  DefinicjaCykluSposobNaDniWolne dniWolne =
			DefinicjaCykluSposobNaDniWolne.None)
		=>
		builder.Enqueue(
		  d =>
			d.Dostawa.Cykl.Set(
			  value,
			  rodzajTerminu,
			  dzienMiesiaca,
			  pozycjaDnia,
			  okresCyklu,
			  dniWolne));

		internal static IRowBuilder<DokumentHandlowy> NowyWRelacji(
			DokumentHandlowy dokument,
			string symbol,
			HandlerSet handlers = null) => Nowy(dokument).GetChild(
				(d, cx) =>
				{
					var relationsApi = cx.Session.GetService<IRelacjeService>();

					var result =
						relationsApi.NowyPodrzednyIndywidualny(
							new[] { d },
							symbol,
							cx,
							handlers);
					return result.Coalesce().Length > 0 ? result[0] : null;
				},
				alternativeBuildOptions: BuildActionOptions.CommitUI_Yes,
				builderOptions:
					BuilderOptions.SetResultIntoContext_Yes |
					BuilderOptions.AlternativeBuild_Only);

		internal static IRowBuilder<DokumentHandlowy> NoweWRelacji(
			this IRowBuilder<DokumentHandlowy>[] inputs,
			string symbol,
			HandlerSet handlers = null) => new RowBuilder<DokumentHandlowy>(
				(_, cx) =>
				{
					var source = inputs.Build();
					var relationsApi = cx.Session.GetService<IRelacjeService>();
					var result = relationsApi.NowyPodrzednyIndywidualny(
						source,
						symbol,
						context: cx,
						handlers: handlers);
					return result.First();
				},
				BuilderOptions.SetResultIntoContext_Yes |
				BuilderOptions.AlternativeBuild_Only);

		internal static IRowBuilder<DokumentHandlowy> UtworzZbiorczy(
			this IRowBuilder<DokumentHandlowy>[] inputs,
			string symbol,
			HandlerSet handlers = null) => new RowBuilder<DokumentHandlowy>(
				(_, cx) =>
				{
					var source = inputs.Build();
					var relationsApi = cx.Session.GetService<IRelacjeService>();

					var result = relationsApi.NowyPodrzednyZbiorczy(
						source,
						symbol,
						cx,
						handlers);
					return result.Coalesce().Length > 0 ? result[0] : null;
				},
				BuilderOptions.SetResultIntoContext_Yes |
				BuilderOptions.AlternativeBuild_Only);

		internal static IRowBuilder<DokumentHandlowy> DolaczNadrzedny(
			this IRowBuilder<DokumentHandlowy> builder,
			string relationName,
			HandlerSet handlers = null) => builder.GetChild(
				(d, cx) =>
				{
					var api = cx.Session.GetRequiredService<IRelacjeService>();
					return api.DolaczNadrzedny(
						new[] { d },
						relationName,
						cx,
						handlers)[0];
				},
				alternativeBuildOptions: BuildActionOptions.CommitUI_Yes,
				builderOptions:
					BuilderOptions.SetResultIntoContext_No |
					BuilderOptions.AlternativeBuild_Only);

		internal static IRowBuilder<TD> Bufor<TD>(
			this IRowBuilder<TD> builder) where TD : DokumentHandlowy
			=> builder.Enqueue(
				d => d.Stan = StanDokumentuHandlowego.Bufor);

		internal static IRowBuilder<DokumentHandlowy> LiczonaOd(
			this IRowBuilder<DokumentHandlowy> builder,
			SposobLiczeniaVAT value) => builder.Enqueue(
				d => d.LiczonaOd = value);

		internal static IRowBuilder<TD> BruttoCy<TD>(
			this IRowBuilder<TD> builder,
			decimal value)
			where TD : DokumentHandlowy => builder.Enqueue(
				d => d.BruttoCy = new Currency(value, d.BruttoCy.Symbol));

	}
}
