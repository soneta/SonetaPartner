using Soneta.Business;
using Soneta.Handel;
using System;
using System.Collections.Generic;

namespace SonetaPartner.Tests.Extensions.Handel.Engine
{
	public static class MyContextExtensions
	{
		/// <summary>
		/// Skrót na potrzeby inicjacji kontekstu.
		/// </summary>
		/// <param name="context">Kontekst roboczy metody.</param>
		/// <param name="defDokHandlowego">Domyślnie: <c>ZK</c></param>
		/// <param name="magazyn">Domyślnie: <c>F</c></param>
		/// <param name="kontrahent">Domyślnie: <c>ABC</c></param>
		/// <param name="towar">Domyślnie: <c>null</c> </param>
		/// <returns>Kontekst roboczy metody w celu umożliwienia potokowego wywołania metody.</returns>
		public static Context Set(
			this Context context,
			string defDokHandlowego = "ZK",
			string magazyn = "F",
			string kontrahent = "ABC",
			string towar = null)
		{
			var module = HandelModule.GetInstance(
				context ?? throw new ArgumentNullException(nameof(context)));

			var dct = new Dictionary<Func<HandelModule, SubTable>, string>
			{
				{ m => m.DefDokHandlowych.WgSymbolu, defDokHandlowego },
				{ m => m.Magazyny.Magazyny.WgSymbol, magazyn },
				{ m => m.CRM.Kontrahenci.WgKodu, kontrahent },
				{ m => m.Towary.Towary.WgKodu, towar },
			};

			foreach (var kv in dct)
			{
				if (kv.Value != null)
				{
					context.Set(
						kv.Key(module).Find(kv.Value) ??
						throw new RowNotFoundException(
							$"{kv.Value}@{kv.Key(module)}"));
				}
			}

			return context;
		}

		public static Context UpdateModule(
			this Context context,
			Action<HandelModule> moduleAction)
		{
			using (var t = context.Session.Logout(true))
			{
				var module = HandelModule.GetInstance(context);
				moduleAction(module);

				t.Commit();
			}

			return context;
		}
	}
}
