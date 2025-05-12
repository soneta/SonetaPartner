using JetBrains.Annotations;

using Soneta.Business;
using Soneta.Core;
using Soneta.Deklaracje;
using Soneta.EwidencjaVat;
using Soneta.Handel;
using Soneta.Import;
using Soneta.Kasa;
using Soneta.Ksiega;
using Soneta.Place;
using Soneta.SrodkiTrwale;
using Wyplata = Soneta.Kasa.Wyplata;
using WyplataPlace = Soneta.Place.Wyplata;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Engine
{
    public static class ExtensionBoxer
    {
        
        public static ProxyRecord<T1> Box<T1>(this T1 row) where T1 : Row
            => new ProxyRecord<T1>(row);


        #region → DokEwidencji

        public static ProxyRecord<PKEwidencja, DokEwidencji> Box(this PKEwidencja row)
            => new ProxyRecord<PKEwidencja, DokEwidencji>(row);

        public static ProxyRecord<ZakupEwidencja, DokEwidencji> Box(this ZakupEwidencja row)
            => new ProxyRecord<ZakupEwidencja, DokEwidencji>(row);


        public static ProxyRecord<SprzedazEwidencja, DokEwidencji> Box(this SprzedazEwidencja row)
            => new ProxyRecord<SprzedazEwidencja, DokEwidencji>(row);

        #endregion

        #region → ElemEwidencjiVAT

        public static ProxyRecord<ElemEwidencjiVATZakup, ElemEwidencjiVAT> Box(this ElemEwidencjiVATZakup row)
            => new ProxyRecord<ElemEwidencjiVATZakup, ElemEwidencjiVAT>(row);

        #endregion

        #region → Przelew

        public static ProxyRecord<Przelew, PrzelewBase> Box(this Przelew row)
            => new ProxyRecord<Przelew, PrzelewBase>(row);

        #endregion

        #region → SrodekTrwalyBase

        public static ProxyRecord<SrodekTrwaly, SrodekTrwalyBase> Box(this SrodekTrwaly row)
            => new ProxyRecord<SrodekTrwaly, SrodekTrwalyBase>(row);

        #endregion

        #region → DokumentST

        public static ProxyRecord<OT, DokumentST> Box(this OT row)
            => new ProxyRecord<OT, DokumentST>(row);

        #endregion

        #region (flatten): ElemOpisuAnalitycznego, DekretBase, ZapisKsiegowy, PozycjaZestKS, Konto

        public static ProxyRecord<ElemOpisuAnalitycznego> Box(this ElementOpisuEwidencji row)
            => new ProxyRecord<ElemOpisuAnalitycznego>(row);

        public static ProxyRecord<DekretBase> Box(this Dekret row)
            => new ProxyRecord<DekretBase>(row);

        public static ProxyRecord<ZapisKsiegowy> Box(this Zapis row)
            => new ProxyRecord<ZapisKsiegowy>(row);

        public static ProxyRecord<PozycjaZestKS> Box(this PozycjaZestKS row)
            => new ProxyRecord<PozycjaZestKS>(row);

        public static ProxyRecord<KontoBase> Box(this KontoSyntetyczne row)
            => new ProxyRecord<KontoBase>(row);

        public static ProxyRecord<KontoBase> Box(this KontoAnalityczne row)
            => new ProxyRecord<KontoBase>(row);

        #endregion
    }
}
