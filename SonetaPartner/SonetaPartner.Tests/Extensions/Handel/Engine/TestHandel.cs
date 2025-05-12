using System;
using System.Collections.Generic;
using NUnit.Framework;
using Soneta.Business;
using Soneta.Core;
using Soneta.CRM;
using SonetaPartner.Tests.Assemblers;
using Soneta.Magazyny;
using Soneta.Test;
using Soneta.Towary;
using Soneta.Handel;

namespace SonetaPartner.Tests.Extensions.Handel.Engine
{
	public abstract class TestHandel : TestBase
	{

		static protected readonly IDictionary<Type, IDictionary<string, string>> SymbolToGuidMap
			= new Dictionary<Type, IDictionary<string, string>>
			{
				{ typeof(DefDokHandlowego),
					new Dictionary<string, string>
					{
						{ "AC", "00000000-0011-0002-0045-000000000000" },
						{ "AD", "00000000-0011-0002-0046-000000000000" },
						{ "DWKPar", "00000000-0011-0002-006a-000000000000" },
						{ "FD", "00000000-0011-0002-0029-000000000000" },
						{ "FF", "00000000-0011-0002-0024-000000000000" },
						{ "FZAL", "00000000-0011-0002-0025-000000000000" },
						{ "FFZAL", "00000000-0011-0002-0038-000000000000" },
						{ "FN", "00000000-0011-0002-0031-000000000000" },
						{ "FPRO", "00000000-0011-0002-0027-000000000000" },
						{ "FV", "00000000-0011-0002-0001-000000000000" },
						{ "FV 2", "00000000-0011-0002-004c-000000000000" },
						{ "FVO", "00000000-0011-0002-0057-000000000000" },
						{ "FVPP", "00000000-0011-0002-006b-000000000000" },
						{ "FVOSS", "00000000-0011-0002-0087-000000000000" },
						{ "INW", "00000000-0011-0002-0012-000000000000" },
						{ "KFD", "00000000-0011-0002-0036-000000000000" },
						{ "KFZAL", "00000000-0011-0002-0028-000000000000" },
						{ "KMMP", "00000000-0011-0002-003a-000000000000" },
						{ "KMMW", "00000000-0011-0002-003b-000000000000" },
						{ "KPL", "00000000-0011-0002-001b-000000000000" },
						{ "KPLW", "00000000-0011-0002-001c-000000000000" },
						{ "KPZ 2", "00000000-0011-0002-0047-000000000000" },
						{ "KS", "00000000-0011-0002-0003-000000000000" },
						{ "KWN", "00000000-0011-0002-0079-000000000000" },
						{ "KWPZ", "00000000-0011-0002-001A-000000000000" },
						{ "KZ", "00000000-0011-0002-0008-000000000000" },
						{ "KZKUE", "00000000-0011-0002-0035-000000000000" },
						{ "KZD", "00000000-0011-0002-0083-000000000000" },
						{ "KZO", "00000000-0011-0002-0082-000000000000" },
						{ "MM", "00000000-0011-0002-000f-000000000000" },
						{ "MMP", "00000000-0011-0002-0010-000000000000" },
						{ "MMW", "00000000-0011-0002-0011-000000000000" },
						{ "OD", "00000000-0011-0002-0020-000000000000" },
						{ "OO", "00000000-0011-0002-0021-000000000000" },
						{ "PAR", "00000000-0011-0002-0002-000000000000" },
						{ "PAR 2", "b2d485c9-19d7-44cd-950b-aa07ec943161" },
						{ "PO", "00000000-0011-0002-0052-000000000000" },
						{ "PW", "00000000-0011-0002-000D-000000000000" },
						{ "PWOD", "00000000-0011-0002-007a-000000000000" },
						{ "PWP", "00000000-0011-0002-005c-000000000000" },
						{ "PZ", "00000000-0011-0002-000b-000000000000" },
						{ "PZ 2", "00000000-0011-0002-0048-000000000000" },
						{ "PZN 2", "00000000-0011-0002-0075-000000000000" },
						{ "REZ", "00000000-0011-0002-000c-000000000000" },
						{ "RW", "00000000-0011-0002-000e-000000000000" },
						{ "RWP", "00000000-0011-0002-005d-000000000000" },
						{ "UC", "00000000-0011-0002-0042-000000000000" },
						{ "UD", "00000000-0011-0002-0043-000000000000" },
						{ "WO", "00000000-0011-0002-0053-000000000000" },
						{ "ZWO", "00000000-0011-0002-0054-000000000000" },
						{ "WZ", "00000000-0011-0002-0009-000000000000" },
						{ "WZ 2", "00000000-0011-0002-004b-000000000000" },
						{ "KWZ 2", "00000000-0011-0002-004a-000000000000" },
						{ "ZD", "00000000-0011-0002-001f-000000000000" },
						{ "ZD2", "00000000-0011-0002-1101-000000000000" },
						{ "ZK", "00000000-0011-0002-0007-000000000000" },
						{ "ZK 2", "00000000-0011-0002-0049-000000000000" },
						{ "ZO", "00000000-0011-0002-001e-000000000000" },
						{ "ZO2", "00000000-0011-0002-1100-000000000000" },
						{ "ZP", "00000000-0011-0002-005b-000000000000" },
						{ "ZKOO", "00000000-0011-0002-0065-000000000000" },
						{ "ZKUE", "00000000-0011-0002-0030-000000000000" },
						{ "ZKUE 2", "00000000-0011-0002-0076-000000000000" },
						{ "ZS", "00000000-0011-0002-005F-000000000000" },
						{ "ZW", "00000000-0011-0002-0088-000000000000" },
						{ "ZZAL", "00000000-0011-0002-0026-000000000000" },
					}
				},
				{ typeof(DefinicjaCeny),
					new Dictionary<string, string>
					{
						{ "Podstawowa", "8942bfde-39ad-43ca-949a-e23949b0a56e" },
						{ "Detaliczna", "e3b12ca1-3f8a-481d-b349-fd7f3e8d1d8b" },
						{ "Hurtowa", "ccbff5aa-18e1-45e2-b46e-88fa4ae14f8f" }
					}
				},
				{ typeof(DefinicjaStawkiVat),
					new Dictionary<string, string>
					{
						{ "23%", "00000000-0001-0003-0010-000000000000" }
					}
				},
				{ typeof(Jednostka),
					new Dictionary<string, string>
					{
						{ "kg", "00000000-0011-0007-0002-000000000000" },
						{ "km", "5b6cf15e-6675-45f7-b27e-d928157a48a0" },
						{ "szt", "00000000-0011-0007-0001-000000000000" },
					}
				},
				{ typeof(Kontrahent),
					new Dictionary<string, string>
					{
						{ "ABC", "ba66f540-1660-11d7-9ab0-000795c951c8" },
						{ "ASPEN", "c339eb98-bb5f-4085-8546-68338de747e9" },
						{ "DRYNDA", "548a6c62-9f37-46f7-856f-78b5297b3cc4" },
						{ "ZEFIR", "ba66f548-1660-11d7-9ab0-000795c951c8" },
						{ "SZKIEŁKO", "8ec37b6d-70b7-48c1-9cf7-407c7e262cae" },
						{ "GAWRON", "ae655abe-4a7d-408e-baa4-44990dcadde9" },
						{ "ENERGOEKO", "acb28c22-e360-438b-8d7e-c69ebabe8a22" },
						{ "HIACYNT", "34482cd4-bf76-4bbb-a406-edae4132d733" },
						{ "ROLMAP", "ba66f54b-1660-11d7-9ab0-000795c951c8" },
						{ "JN", "9287d45d-ac59-44d2-b98f-4c2602007fcf" }
					}
				},
				{ typeof(Soneta.Magazyny.Magazyn),
					new Dictionary<string, string>
					{
						{ "F", "00000000-0011-0004-0001-000000000000" }
					}
				},
				{ typeof(Lokalizacja),
					new Dictionary<string, string>
					{
						{ "MG", "0b494c3d-00aa-48b3-b0ea-1f59e0e465b4" }
					}
				},
				{ typeof(Towar),
					new Dictionary<string, string>
					{
						{ KodyTowarów.Bikini, "65336878-70cf-4e64-bd72-b742cd26a657" },
						{ KodyTowarów.Buty42, "97e7f69a-a1dc-45e1-8d3b-31082c3da1d4" },
						{ KodyTowarów.Buty43, "fa0ec7f5-3ec8-4182-aecb-fc3938c9dbb0" },
						{ KodyTowarów.Buty44, "4e668c2f-5cbc-4634-89f4-591677a031d4" },
						{ KodyTowarów.Buty45, "61e50151-9f26-4de3-85dc-5d0983f56956" },
						{ KodyTowarów.Kij160, "078A7EBF-0628-47CD-99A2-B5FB9EFE000C" },
						{ KodyTowarów.Kij180, "4d5a02d3-2efe-4d3d-bd33-1f30d1d94e75" },
						{ KodyTowarów.Kombinezon, "be72516b-bde5-4b0f-aa6b-9c4ab8a7e419" },
						{ KodyTowarów.Kombinezon_S_M, "f859d779-a467-42f5-9e90-cb281db402b6" },
						{ KodyTowarów.Kombinezon_T_C, "8e0ce228-451a-4380-b853-3cddf3185a7a" },
						{ KodyTowarów.Kombinezon_T_N, "2f5ad2ee-6e9b-4184-b45d-d4cb8852d37b" },
						{ KodyTowarów.Kombinezon_T_X, "6147e846-f21e-4306-8dc6-cac472af01f5" },
						{ KodyTowarów.Lyzwy, "6f46293a-56a0-4c08-8f86-c823b338e07a" },
						{ KodyTowarów.Montaz, "0f8a8597-e2d1-40a6-a8e5-cc1045228660" },
						{ KodyTowarów.Namiot, "989739bc-84a2-4b6b-9325-780a26f6c9df" },
						{ KodyTowarów.Namiot2, "7a18b40f-0b13-4628-a7ae-72c0f5a672e0" },
						{ KodyTowarów.Narty150, "c89304fb-c3f9-4911-82db-b7665f748b30" },
						{ KodyTowarów.Rower, "22b75f83-8b8e-4edc-af8d-e18d05fc4774" },
						{ KodyTowarów.Smar, "2a6b883a-0038-47fd-a1be-d4f37bd7e443" },
						{ KodyTowarów.Transport, "0022a4cb-27ac-42ac-bac2-335db984c12c" },
						{ KodyTowarów.Zestaw, "104d7eb1-7379-41e4-af65-c4acfb2ff0ba" },
						{ KodyTowarów.Wiązania2, "00628a0d-2605-4a80-a27f-641080bec99a" },
						{ KodyTowarów.PrzesylkaKurierska, "dc818350-547d-4db7-a0e8-1fb4dfad4fb0" }
					}
				},
				{ typeof(FeatureSetDefinition),
					new Dictionary<string, string>
					{
						{ "Grupy", "5c4d8759-03e7-4843-90eb-dd0213456cb2" }
					}
				}
			};

		protected static Soneta.Magazyny.Magazyn DodajDrugiMagazyn(
			string symbol = "m2",
			Action<Soneta.Magazyny.Magazyn> configure = null)
		{
			return Nowy<Soneta.Magazyny.Magazyn>()
							 .Symbol(symbol).Nazwa(symbol).Opis(symbol)
							 .PrzyznajUprawnienie()
							 .Enqueue(configure ?? delegate { })
							 .Utwórz();
		}
		
		protected static IRowBuilder<T> Get<T>(string symbol) where T : GuidedRow
		{
			var guid = new Guid(SymbolToGuidMap[typeof(T)][symbol]);
			return new RowBuilder<T>((_, cx) => cx.Session.Get<T>(guid));
		}

		protected static IRowBuilder<T> GetBuilderOf<T>(Guid guid, BuilderOptions options = BuilderOptions.None)
		where T : GuidedRow
		=> new RowBuilder<T>((_, cx) => cx.Session.Get<T>(guid), options);

		protected static IRowBuilder<T> Nowy<T>() where T : Row
		{
			return new RowBuilder<T>();
		}

		protected static class KodyTowarów
		{
			public const string Bikini = "BIKINI";
			[Obsolete]
			public const string Buty = "Buty";
			public const string Buty42 = "BUT_NAR_42";
			public const string Buty43 = "BUT_NAR_43";
			public const string Buty44 = "BUT_NAR_44";
			public const string Buty45 = "BUT_NAR_45";
			public const string Kij160 = "KIJ_NAR_160";
			public const string Kij180 = "KIJ_NAR_180";
			public const string Koszulki = "Koszulki";
			public const string Kombinezon = "KOM_NAR_S_C";
			public const string Kombinezon_S_M = "KOM_NAR_S_M";
			public const string Kombinezon_T_C = "KOM_NAR_T_C";
			public const string Kombinezon_T_X = "KOM_NAR_T_X";
			public const string Kombinezon_T_N = "KOM_NAR_T_N";
			public const string Krawat = "Krawat";
			public const string Lyzwy = "LYZ_FB_36";
			public const string Lyzwy42 = "LYZ_FC_42";
			public const string Montaz = "MONTAZ";
			public const string Namiot = "NAM_HOME1";
			public const string Namiot2 = "NAM_HOME2";
			public const string Namiot4 = "NAM_HOME4";
			public const string Narty = "NAR_BIEG_220";
			public const string Narty150 = "NAR_Z0_150";
			public const string Produkt1T01 = "P001";
			public const string Produkt2T01 = "P002";
			public const string Produkt3T02 = "P003";
			public const string Produkt4T03 = "P004";
			public const string Rower = "ROW_G_SCOUT_18_C";
			public const string Śpiwór = "SPIWOR";
			public const string Smar = "SMA_NAR";
			public const string SmarWWorku = "SMA_NAR_WOR";
			public const string Transport = "TRANSPORT";
			public const string Trentino = "TRENTINO";
			public const string Wiązania2 = "WIA_NAR_02";
			public const string Wrotki = "WRO_1SCC";
			public const string Zestaw = "ZES_Z190";
			public const string PrzesylkaKurierska = "PK";
		}
	}
}
