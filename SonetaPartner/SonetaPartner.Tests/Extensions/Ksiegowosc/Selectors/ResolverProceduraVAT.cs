using JetBrains.Annotations;
using Soneta.Business;
using Soneta.Core;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors
{
	public sealed class ResolverProceduraVAT
	{
		private SelectorProceduraVAT _selector;

		private ResolverProceduraVAT()
		{ }

		public static implicit operator ResolverProceduraVAT(SelectorProceduraVAT selector)
			=> new ResolverProceduraVAT { _selector = selector };

		public ProceduraVAT Resolve(ISessionable sProvider)
		{
			var finder = sProvider.Finder();

			switch (_selector)
			{
				case SelectorProceduraVAT.TypSprzedaz_FP:
					return finder.ProceduraVAT(TypProceduryVAT.TypDokumentuSprzedazVAT, "FP");
				case SelectorProceduraVAT.TypSprzedaz_RO:
					return finder.ProceduraVAT(TypProceduryVAT.TypDokumentuSprzedazVAT, "RO");
				case SelectorProceduraVAT.TypSprzedaz_WEW:
					return finder.ProceduraVAT(TypProceduryVAT.TypDokumentuSprzedazVAT, "WEW");

				case SelectorProceduraVAT.TypZakup_MK:
					return finder.ProceduraVAT(TypProceduryVAT.TypDokumentuZakupVAT, "MK");
				case SelectorProceduraVAT.TypZakup_VAT_RR:
					return finder.ProceduraVAT(TypProceduryVAT.TypDokumentuZakupVAT, "VAT_RR");
				case SelectorProceduraVAT.TypZakup_WEW:
					return finder.ProceduraVAT(TypProceduryVAT.TypDokumentuZakupVAT, "WEW");

				case SelectorProceduraVAT.ProcSprzedaz_B_MPV_Prowizja:
					return finder.ProceduraVAT(TypProceduryVAT.ProceduraSprzedazVAT, "B_MPV_Prowizja");
				case SelectorProceduraVAT.ProcSprzedaz_B_SPV:
					return finder.ProceduraVAT(TypProceduryVAT.ProceduraSprzedazVAT, "B_SPV");
				case SelectorProceduraVAT.ProcSprzedaz_B_SPV_Dostawa:
					return finder.ProceduraVAT(TypProceduryVAT.ProceduraSprzedazVAT, "B_SPV_Dostawa");
				case SelectorProceduraVAT.ProcSprzedaz_EE:
					return finder.ProceduraVAT(TypProceduryVAT.ProceduraSprzedazVAT, "EE");
				case SelectorProceduraVAT.ProcSprzedaz_I_42:
					return finder.ProceduraVAT(TypProceduryVAT.ProceduraSprzedazVAT, "I_42");
				case SelectorProceduraVAT.ProcSprzedaz_I_63:
					return finder.ProceduraVAT(TypProceduryVAT.ProceduraSprzedazVAT, "I_63");
				case SelectorProceduraVAT.ProcSprzedaz_IED:
					return finder.ProceduraVAT(TypProceduryVAT.ProceduraSprzedazVAT, "IED");
				case SelectorProceduraVAT.ProcSprzedaz_MPP:
					return finder.ProceduraVAT(TypProceduryVAT.ProceduraSprzedazVAT, "MPP");
				case SelectorProceduraVAT.ProcSprzedaz_MR_T:
					return finder.ProceduraVAT(TypProceduryVAT.ProceduraSprzedazVAT, "MR_T");
				case SelectorProceduraVAT.ProcSprzedaz_MR_UZ:
					return finder.ProceduraVAT(TypProceduryVAT.ProceduraSprzedazVAT, "MR_UZ");
				case SelectorProceduraVAT.ProcSprzedaz_SW:
					return finder.ProceduraVAT(TypProceduryVAT.ProceduraSprzedazVAT, "SW");
				case SelectorProceduraVAT.ProcSprzedaz_TP:
					return finder.ProceduraVAT(TypProceduryVAT.ProceduraSprzedazVAT, "TP");
				case SelectorProceduraVAT.ProcSprzedaz_TT_D:
					return finder.ProceduraVAT(TypProceduryVAT.ProceduraSprzedazVAT, "TT_D");
				case SelectorProceduraVAT.ProcSprzedaz_TT_WNT:
					return finder.ProceduraVAT(TypProceduryVAT.ProceduraSprzedazVAT, "TT_WNT");
				case SelectorProceduraVAT.ProcSprzedaz_WSTO_EE:
					return finder.ProceduraVAT(TypProceduryVAT.ProceduraSprzedazVAT, "WSTO_EE");

				case SelectorProceduraVAT.ProcZakup_IMP:
					return finder.ProceduraVAT(TypProceduryVAT.ProceduraZakupVAT, "IMP");
				case SelectorProceduraVAT.ProcZakup_MPP:
					return finder.ProceduraVAT(TypProceduryVAT.ProceduraZakupVAT, "MPP");

				case SelectorProceduraVAT.Gr01:
					return finder.ProceduraVAT(TypProceduryVAT.GrupaTowarowaVAT, "01");
				case SelectorProceduraVAT.Gr02:
					return finder.ProceduraVAT(TypProceduryVAT.GrupaTowarowaVAT, "02");
				case SelectorProceduraVAT.Gr03:
					return finder.ProceduraVAT(TypProceduryVAT.GrupaTowarowaVAT, "03");
				case SelectorProceduraVAT.Gr04:
					return finder.ProceduraVAT(TypProceduryVAT.GrupaTowarowaVAT, "04");
				case SelectorProceduraVAT.Gr05:
					return finder.ProceduraVAT(TypProceduryVAT.GrupaTowarowaVAT, "05");
				case SelectorProceduraVAT.Gr06:
					return finder.ProceduraVAT(TypProceduryVAT.GrupaTowarowaVAT, "06");
				case SelectorProceduraVAT.Gr07:
					return finder.ProceduraVAT(TypProceduryVAT.GrupaTowarowaVAT, "07");
				case SelectorProceduraVAT.Gr08:
					return finder.ProceduraVAT(TypProceduryVAT.GrupaTowarowaVAT, "08");
				case SelectorProceduraVAT.Gr09:
					return finder.ProceduraVAT(TypProceduryVAT.GrupaTowarowaVAT, "09");
				case SelectorProceduraVAT.Gr10:
					return finder.ProceduraVAT(TypProceduryVAT.GrupaTowarowaVAT, "10");
				case SelectorProceduraVAT.Gr11:
					return finder.ProceduraVAT(TypProceduryVAT.GrupaTowarowaVAT, "11");
				case SelectorProceduraVAT.Gr12:
					return finder.ProceduraVAT(TypProceduryVAT.GrupaTowarowaVAT, "12");
				case SelectorProceduraVAT.Gr13:
					return finder.ProceduraVAT(TypProceduryVAT.GrupaTowarowaVAT, "13");
			}

			throw new TestException($"ResolverProceduraVAT: invalid selector: {_selector}");
		}
	}
}
