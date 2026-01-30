using CsvHelper;
using CsvHelper.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SonetaPartner.Tests.TestyZDanymiZPliku
{
	public static class CsvTowaryTestData
	{
		public static IEnumerable<TowarCsvRow> GetTowary()
		{
			var config = new CsvConfiguration(new CultureInfo("pl-PL"))
			{
				Delimiter = ";",
				HasHeaderRecord = true
			};

			string projectDir = Path.GetFullPath(
				Path.Combine(AppContext.BaseDirectory, @"..\..\..\")
			);

			var resPath = Path.Combine(projectDir, "Res", "Towary.csv");

			using var reader = new StreamReader(resPath);
			using var csv = new CsvReader(reader, config);

			csv.Context.RegisterClassMap<TowarCsvMap>();

			return csv.GetRecords<TowarCsvRow>().ToList();
		}

		public static IEnumerable<TestCaseData> TowaryZCsv()
		{
			foreach (var row in GetTowary())
			{
				yield return new TestCaseData(row)
					.SetName($"NowyTowar_{row.Kod}");
			}
		}

		public static IEnumerable<TestCaseData> TowaryZCsv2()
		{
			int count = 1;
			foreach (var row in GetTowary().Concat(GetTowary()))
			{
				yield return new TestCaseData(row)
					.SetName($"NowyTowar_{row.Kod}_{count++}");
			}
		}
	}
}
