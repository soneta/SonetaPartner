using Soneta.Handel;
using Soneta.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonetaPartner.Tests.Assemblers
{
	static class DefRelacjiAssembler
	{
		internal static IRowBuilder<DefRelacjiHandlowej> Nowa()
		{
			return new RowBuilder<DefRelacjiHandlowej>(
				(drh, cx) => cx.Session.AddRow(new DefRelacjiKopiowania()),
				BuilderOptions.SetResultIntoContext_No | BuilderOptions.SessionMode_AsTable);
		}

		internal static IRowBuilder<DefRelacjiHandlowej> NowaZaliczka()
		{
			return new RowBuilder<DefRelacjiHandlowej>(
					(drh, cx) => cx.Session.AddRow(new DefRelacjiZaliczki()),
					BuilderOptions.SetResultIntoContext_No | BuilderOptions.SessionMode_AsTable);
		}

		internal static IRowBuilder<DefRelacjiHandlowej> WielePodrzednych(
			this IRowBuilder<DefRelacjiHandlowej> builder, bool value = true)
		{
			return builder.Enqueue(d => d.ZNadrzednego.WieleDokumentow = value);
		}

		internal static IRowBuilder<DefRelacjiHandlowej> Blokada(
			this IRowBuilder<DefRelacjiHandlowej> builder,
			bool value) => builder.Enqueue(d => d.Blokada = value);

		internal static IRowBuilder<DefRelacjiHandlowej> DefinicjaNadrzednego(
			this IRowBuilder<DefRelacjiHandlowej> builder,
			string symbol)
		{
			return builder
				.Enqueue(d => d.DefinicjaNadrzednego = d.Module.DefDokHandlowych.WgSymbolu[symbol]);
		}

		internal static IRowBuilder<DefRelacjiHandlowej> DefinicjaPodrzednego(
			this IRowBuilder<DefRelacjiHandlowej> builder,
			string symbol)
		{
			return builder
				.Enqueue(d => d.DefinicjaPodrzednego = d.Module.DefDokHandlowych.WgSymbolu[symbol]);
		}

		internal static IRowBuilder<DefRelacjiHandlowej> ZPodrzednegoDolaczDokument(
			this IRowBuilder<DefRelacjiHandlowej> builder, bool value = true)
		{
			return builder.Enqueue(d => d.ZPodrzednego.IstniejacyDokument = value);
		}

		internal static IRowBuilder<DefRelacjiHandlowej> WyborPozycji(
			this IRowBuilder<DefRelacjiHandlowej> builder,
			WyborPozycjiDlaRelacji wyborPozycji)
		{
			return builder.Enqueue(d => d.Zachowanie.WyborPozycji = wyborPozycji);
		}

		internal static IRowBuilder<DefRelacjiHandlowej> WieleNadrzednych(
			this IRowBuilder<DefRelacjiHandlowej> builder, bool value = true)
		{
			return builder.Enqueue(d => d.ZPodrzednego.WieleDokumentow = value);
		}
	}
}
