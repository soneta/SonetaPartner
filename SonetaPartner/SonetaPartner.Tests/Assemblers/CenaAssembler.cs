using Soneta.Test;
using Soneta.Towary;
using Soneta.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonetaPartner.Tests.Assemblers
{
	static class CenaAssembler
	{
		internal static IRowBuilder<Cena> Netto(this IRowBuilder<Cena> builder, double value)
			=> builder.Netto(value, "PLN");

		internal static IRowBuilder<Cena> Netto(this IRowBuilder<Cena> builder, double value, string symbol)
			=> builder.Enqueue(c => c.Netto = new DoubleCy(value, symbol));
	}
}
