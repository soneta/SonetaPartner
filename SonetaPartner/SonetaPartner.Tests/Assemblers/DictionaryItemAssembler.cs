using Soneta.Business.Db;
using Soneta.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonetaPartner.Tests.Assemblers
{
	static class DictionaryItemAssembler
	{
		internal static IRowBuilder<DictionaryItem> Nowa(string category)
		{
			return new RowBuilder<DictionaryItem>(
				(_, tc) => tc.Session.AddRow(new DictionaryItem(category, 1)));
		}

		internal static IRowBuilder<DictionaryItem> Value(this IRowBuilder<DictionaryItem> builder, string value)
		{
			return builder.Enqueue(d => d.Value = value);
		}
	}
}
