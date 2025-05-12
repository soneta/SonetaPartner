using Soneta.Business;
using Soneta.Core;
using Soneta.Handel;
using Soneta.Magazyny;
using Soneta.Tools;
using Soneta.Types;


[assembly: NewRow(typeof(Soneta.Magazyny.Magazyn))]
namespace SonetaPartner.Tests.Extensions.Magazyn.Engine
{
	[DefaultWidth(12)]
	public class Magazyn : MagazynyModule.MagazynRow,
		IElementSlownika,
		IPulpitHost,
		IElemSysZewnHost,
		IOddzialProvider
	{
		#region Verifiers
		internal class CechaAlgorytmuVerifier : RowVerifier
		{
			internal CechaAlgorytmuVerifier(Magazyn m) : base(m)
			{
			}

			new Magazyn Row
			{
				get { return (Magazyn)base.Row; }
			}

			protected override bool IsValid()
			{
				Table t = Row.tabelaCechyAlgorytmu();
				if (t == null) return true;

				return t.FeatureDefinitions.Contains(Row.CechaAlgorytmu);
			}

			public override string Description
			{
				get
				{
					return "Niewypełniona lub niepoprawna nazwa cechy algorytmu w magazynie '{0}'. Wybierz poprawną cechę algorytmu w magazynie.".TranslateFormat(Row);
				}
			}
		}

		#endregion

		#region Filtr towaru

		/// <summary>
		/// Skompilowany filtr towaru.
		/// </summary>
		private RowCondition _filtrTowaruCondition;

		/// <summary>
		/// Pobiera lub ustawia wprowadzony przez użytkownika filtr towaru.
		/// </summary>
		/// <remarks>
		/// Zmiana wartości filtra towaru powoduje anulowanie skompilowanego filtra towaru.
		/// Filtr zostanie rekompilowany.
		/// </remarks>
		[AttributeInheritance]
		public new string FiltrTowaru
		{
			get
			{
				if (this.Module.Handel.Config.Ogólne.KontekstMagazynu)
				{
					return base.FiltrTowaru;
				}
				else
				{
					return string.Empty;
				}
			}
			set
			{
				if (!this.FiltrTowaru.Equals(value))
				{
					base.FiltrTowaru = value;
					this._filtrTowaruCondition = null;
				}
			}
		}

		#endregion

		#region Cecha algorytmu

		Table tabelaCechyAlgorytmu()
		{
			switch (Algorytm)
			{
				case AlgorytmMagazynowy.WgCechyDokumentu:
				case AlgorytmMagazynowy.WgCechyDokumentuMalejąco:
					return Module.Handel.DokHandlowe;

				case AlgorytmMagazynowy.WgCechyPozycji:
				case AlgorytmMagazynowy.WgCechyPozycjiMalejąco:
					return Module.Handel.PozycjeDokHan;
			}
			return null;
		}

		#endregion

		#region Soneta.Core.IElementSlownika
		string Soneta.Core.IElementSlownika.Segment { get { return Nazwa; } }
		string Soneta.Core.IElementSlownika.Nazwa { get { return Nazwa; } }
		#endregion
	}
}
