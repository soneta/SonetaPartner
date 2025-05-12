using Soneta.Test;
using Soneta.Towary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonetaPartner.Tests.Assemblers
{
	static class DefinicjaCenyAssembler
	{
		internal static IRowBuilder<DefinicjaCeny> Rabat1Rodzaj(
			this IRowBuilder<DefinicjaCeny> builder,
			RodzajRabatu value)
			=> builder.Enqueue(d => d.Rabat1.Rodzaj = value);

		internal static IRowBuilder<DefinicjaCeny> Rabat2Rodzaj(
			this IRowBuilder<DefinicjaCeny> builder,
			RodzajRabatu value)
			=> builder.Enqueue(d => d.Rabat2.Rodzaj = value);

		internal static IRowBuilder<DefinicjaCeny> Rabat3Rodzaj(
			this IRowBuilder<DefinicjaCeny> builder,
			RodzajRabatu value)
			=> builder.Enqueue(d => d.Rabat3.Rodzaj = value);

		internal static IRowBuilder<DefinicjaCeny> Rabat4Rodzaj(
			this IRowBuilder<DefinicjaCeny> builder,
			RodzajRabatu value)
			=> builder.Enqueue(d => d.Rabat4.Rodzaj = value);

		internal static IRowBuilder<DefinicjaCeny> Rabat5Rodzaj(
		   this IRowBuilder<DefinicjaCeny> builder,
		   RodzajRabatu value)
			=> builder.Enqueue(d => d.Rabat5.Rodzaj = value);

		internal static IRowBuilder<DefinicjaCeny> Rabat1Wliczaj(
			this IRowBuilder<DefinicjaCeny> builder,
			WliczanieRabatu value)
			=> builder.Enqueue(d => d.Rabat1.Wliczaj = value);

		internal static IRowBuilder<DefinicjaCeny> Rabat2Wliczaj(
			this IRowBuilder<DefinicjaCeny> builder,
			WliczanieRabatu value)
			=> builder.Enqueue(d => d.Rabat2.Wliczaj = value);

		internal static IRowBuilder<DefinicjaCeny> Rabat3Wliczaj(
			this IRowBuilder<DefinicjaCeny> builder,
			WliczanieRabatu value)
			=> builder.Enqueue(d => d.Rabat3.Wliczaj = value);

		internal static IRowBuilder<DefinicjaCeny> Rabat4Wliczaj(
			this IRowBuilder<DefinicjaCeny> builder,
			WliczanieRabatu value)
			=> builder.Enqueue(d => d.Rabat4.Wliczaj = value);

		internal static IRowBuilder<DefinicjaCeny> Rabat5Wliczaj(
			this IRowBuilder<DefinicjaCeny> builder,
			WliczanieRabatu value)
			=> builder.Enqueue(d => d.Rabat5.Wliczaj = value);

		internal static IRowBuilder<DefinicjaCeny> Rabat1Grupa(
			this IRowBuilder<DefinicjaCeny> builder,
			Guid guid)
			=> builder.Enqueue(d => d.Rabat1.Grupa = d.Module.Business.FeatureDefs[guid]);

		internal static IRowBuilder<DefinicjaCeny> Rabat2Grupa(
		   this IRowBuilder<DefinicjaCeny> builder,
		   Guid guid)
			=> builder.Enqueue(d => d.Rabat2.Grupa = d.Module.Business.FeatureDefs[guid]);

		internal static IRowBuilder<DefinicjaCeny> Rabat3Grupa(
		  this IRowBuilder<DefinicjaCeny> builder,
		  Guid guid)
			=> builder.Enqueue(d => d.Rabat3.Grupa = d.Module.Business.FeatureDefs[guid]);

		internal static IRowBuilder<DefinicjaCeny> Rabat2GrupaTowarowa(
			this IRowBuilder<DefinicjaCeny> builder,
			Guid guid)
			=> builder.Enqueue(d => d.Rabat2.GrupaTowarowa = d.Module.Business.FeatureDefs[guid]);

		internal static IRowBuilder<DefinicjaCeny> Rabat4GrupaTowarowa(
			this IRowBuilder<DefinicjaCeny> builder,
			Guid guid)
			=> builder.Enqueue(d => d.Rabat4.GrupaTowarowa = d.Module.Business.FeatureDefs[guid]);
	}
}
