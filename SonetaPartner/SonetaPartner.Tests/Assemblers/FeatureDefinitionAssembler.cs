using Soneta.Business;
using Soneta.Test;

namespace SonetaPartner.Tests.Assemblers
{
	static class FeatureDefinitionAssembler
	{
		public static IRowBuilder<FeatureDefinition> Dictionary(this IRowBuilder<FeatureDefinition> builder, string value)
		{
			return builder.Enqueue(fd => fd.Dictionary = value);
		}

		public static IRowBuilder<FeatureDefinition> Nowa(string nazwa, string tableName)
		{
			return new RowBuilder<FeatureDefinition>((fd, tc) => tc.Session.AddRow(new FeatureDefinition(tableName)))
				.Nazwa(nazwa)
				.Typ(FeatureTypeNumber.String);
		}

		public static IRowBuilder<FeatureDefinition> Nazwa(this IRowBuilder<FeatureDefinition> builder, string value)
		{
			return builder.Enqueue(fd => fd.Name = value);
		}

		public static IRowBuilder<FeatureDefinition> Typ(this IRowBuilder<FeatureDefinition> builder, FeatureTypeNumber value)
		{
			return builder.Enqueue(fd => fd.TypeNumber = value);
		}

		public static IRowBuilder<T> SetFeature<T>(this IRowBuilder<T> builder, string featureName, object value) where T : Row
		{
			return builder.Enqueue(r => r[featureName] = value);
		}
	}
}
