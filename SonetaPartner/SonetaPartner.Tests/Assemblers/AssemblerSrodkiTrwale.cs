using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using Soneta.Business;
using Soneta.Core;
using Soneta.SrodkiTrwale;
using Soneta.Types;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Settings;
using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;



namespace SonetaPartner.Tests.Assemblers
{
    public static class AssemblerSrodkiTrwale
    {
        #region SrodekTrwaly(Base)

        public static ProxyRecord<T1, SrodekTrwalyBase> SetDataBO<T1>(this ProxyRecord<T1, SrodekTrwalyBase> row, Date data)
            where T1 : SrodekTrwalyBase
            => row.InTransUI(λ => λ.Row.DataBO = data);


        public static ProxyRecord<T1, SrodekTrwalyBase> SetWartoscBO<T1>(this ProxyRecord<T1, SrodekTrwalyBase> row, Currency wartoscPoczatkowaBilansowaBO, Currency wartoscBilansowaBO01, Currency odpisyBilansoweBO01, Currency wartoscBilansowaBO, Currency odpisyBilansoweBO)
            where T1 : SrodekTrwalyBase
            => row
                .InTransUI(λ => λ.Row.WartoscPoczatkowaBilansowaBO = wartoscPoczatkowaBilansowaBO)
                .InTransUI(λ => λ.Row.WartoscBilansowaBO01 = wartoscBilansowaBO01)
                .InTransUI(λ => λ.Row.OdpisyBilansoweBO01 = odpisyBilansoweBO01)
                .InTransUI(λ => λ.Row.WartoscBilansowaBO = wartoscBilansowaBO)
                .InTransUI(λ => λ.Row.OdpisyBilansoweBO = odpisyBilansoweBO);
        public static ProxyRecord<T1, SrodekTrwalyBase> SetNumerInw<T1>(this ProxyRecord<T1, SrodekTrwalyBase> row, string numerInw = Defaults.CodeQuestionMark)
            where T1 : SrodekTrwalyBase
            => row.InTransUI(λ => λ.Row.NumerInwentarzowy = numerInw);


        public static ProxyRecord<T1, SrodekTrwalyBase> SetNumerFabryczny<T1>(this ProxyRecord<T1, SrodekTrwalyBase> row, string numerfabryczny)
            where T1 : SrodekTrwalyBase
            => row.InTransUI(λ => λ.Row.NumerFabryczny = numerfabryczny);


        public static ProxyRecord<T1, SrodekTrwalyBase> SetNazwa<T1>(this ProxyRecord<T1, SrodekTrwalyBase> row, string nazwa)
            where T1 : SrodekTrwalyBase
            => row.InTransUI(λ => λ.Row.Nazwa = nazwa);


        public static ProxyRecord<T1, SrodekTrwalyBase> SetOpis<T1>(this ProxyRecord<T1, SrodekTrwalyBase> row, string opis)
            where T1 : SrodekTrwalyBase
            => row.InTransUI(λ => λ.Row.Opis = opis);


        public static ProxyRecord<T1, SrodekTrwalyBase> SetKlasyfikacja<T1>(this ProxyRecord<T1, SrodekTrwalyBase> row, [NotNull] ResolverRodzajST rodzajST)
            where T1 : SrodekTrwalyBase
            => row.InTransUI(λ => λ.Row.Last.KRST = rodzajST.Resolve(row.Row));


        public static ProxyRecord<T1, SrodekTrwalyBase> SetOdpowiedzialny<T1>(this ProxyRecord<T1, SrodekTrwalyBase> row, ResolverPracownik odpowiedzialny)
            where T1 : SrodekTrwalyBase
            => row.InTransUI(λ => λ.Row.Last.Odpowiedzialny = odpowiedzialny?.Resolve(λ));


        public static ProxyRecord<T1, SrodekTrwalyBase> SetCentrumKosztow<T1>(this ProxyRecord<T1, SrodekTrwalyBase> row, ResolverCentrumKosztow ck)
            where T1 : SrodekTrwalyBase
            => row.InTransUI(λ => λ.Row.Last.CentrumKosztow = ck?.Resolve(λ));


        public static ProxyRecord<T1, SrodekTrwalyBase> SetRozpoczecieAmortyzacjiWMiesiacuUzytkowania<T1>(this ProxyRecord<T1, SrodekTrwalyBase> row, bool setOn = true)
            where T1 : SrodekTrwalyBase
            => row.InTransUI(λ => λ.Row.RozpoczecieAmortyzacji = setOn);


        public static ProxyRecord<T1, SrodekTrwalyBase> SetAmortyzacja<T1>(this ProxyRecord<T1, SrodekTrwalyBase> row, MetodaAmortyzacji metoda, Percent sBilansowa, Percent? sPodatkowa = null)
            where T1 : SrodekTrwalyBase
            => row
                .InTransUI(λ => λ.Row.Last.Bilansowa.Metoda = metoda)
                .InTransUI(λ => λ.Row.Last.Bilansowa.Stawka = sBilansowa)
                .InTransUI(λ => λ.Row.Last.Podatkowa.Stawka = sPodatkowa ?? sBilansowa);

        public static ProxyRecord<T1, SrodekTrwalyBase> NewOT<T1>(
            this ProxyRecord<T1, SrodekTrwalyBase> row,
            out ProxyRecord<OT, DokumentST> newRow,
            OddzialFirmy oddzial,
            Action<ProxyRecord<OT, DokumentST>> fn = null
            )
            where T1 : SrodekTrwalyBase
        {
            row.InTransUIRes(sess => sess.AddRow(new OT(row.Row) { Oddzial = row.Session.Get(oddzial)}))
                .Box()
                .Out(out newRow);

            fn?.Invoke(newRow);
            return row;
        }

        public static ProxyRecord<T1, SrodekTrwalyBase> NewOT<T1>(this ProxyRecord<T1, SrodekTrwalyBase> row, Action<ProxyRecord<OT, DokumentST>> fn = null, OddzialFirmy oddzial = null)
            where T1 : SrodekTrwalyBase
            => row.NewOT(out _, oddzial, fn );

        #endregion

        #region Terminarz inwentarza

        public static ProxyRecord<T1, SrodekTrwalyBase> SetPozycjaTerminarzaInwentarza<T1>(this ProxyRecord<T1, SrodekTrwalyBase> row, string nazwa, Date dataPlanowana, Date dataRealizacji, string opis = "")
            where T1 : SrodekTrwalyBase
        {
            row.NewPozycjaTerminarzaInwentarza(out var pozycja);
            pozycja
                .InTransUI(λ => λ.Row.Nazwa = nazwa)
                .InTransUI(λ => λ.Row.DataPlanowana = dataPlanowana)
                .InTransUI(λ => λ.Row.DataRealizacji = dataRealizacji)
                .InTransUI(λ => λ.Row.Opis = opis);

            return row;
        }

        public static ProxyRecord<T1, SrodekTrwalyBase> NewPozycjaTerminarzaInwentarza<T1>(
            this ProxyRecord<T1, SrodekTrwalyBase> row,
            out ProxyRecord<PozycjaTerminarzaInw> newRow,
            Action<ProxyRecord<PozycjaTerminarzaInw>> fn = null)
            where T1 : SrodekTrwalyBase
        {
            var ic = new InwentarzContext(Context.Empty.Clone(row.Session));
            ic.Inwentarz = row.Row;
            row.InTransUIRes(sess => sess.AddRow(new PozycjaTerminarzaInw(ic)))
                .Box()
                .Out(out newRow);

            fn?.Invoke(newRow);
            return row;
        }

        #endregion

        #region DokumentST (generic)

        public static ProxyRecord<T1, DokumentST> SetData<T1>(this ProxyRecord<T1, DokumentST> row, Date data)
            where T1 : DokumentST
            => row.InTransUI(λ => λ.Row.Data = data);

        public static ProxyRecord<T1, DokumentST> SetDataOperacji<T1>(this ProxyRecord<T1, DokumentST> row, Date data)
            where T1 : DokumentST
            => row.InTransUI(λ => λ.Row.DataOperacji = data);

        #endregion

        #region DokumentST (specific)

        public static ProxyRecord<T1, DokumentST> SetWartosc<T1>(this ProxyRecord<T1, DokumentST> row, decimal wBilansowa, decimal? wPodatkowa = null)
            where T1 : PojedynczyDokumentST
            => row
                .InTransUI(λ => λ.Row.Pozycja.WartoscBilansowa = wBilansowa)
                .InTransUI(λ => λ.Row.Pozycja.WartoscPodatkowa = wPodatkowa ?? wBilansowa);

        #endregion

        #region szkolenie

		public static ProxyRecord<T1, SrodekTrwalyBase> SetSezonowosc<T1>(this ProxyRecord<T1, SrodekTrwalyBase> row, RodzajSezonowosci sezonowosc)
			where T1 : SrodekTrwalyBase
			=> row.InTransUI(λ => λ.Row.Last.Sezonowosc.Rodzaj = sezonowosc);

		#endregion
	}
}

