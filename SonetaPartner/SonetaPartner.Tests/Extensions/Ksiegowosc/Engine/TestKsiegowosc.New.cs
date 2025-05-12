using JetBrains.Annotations;
using Soneta.Business;
using Soneta.Core;
using Soneta.EwidencjaVat;
using Soneta.Kasa;
using Soneta.Ksiega;
using Soneta.SrodkiTrwale;
using Soneta.Types;
using SonetaPartner.Tests.Assemblers;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Settings;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Engine
{
    public partial class TestKsiegowosc
    {

        public ProxyRecord<SrodekTrwaly, SrodekTrwalyBase> NewSrodekTrwaly(string numerInw = Defaults.CodeQuestionMark, string nazwa = Defaults.SrodekNazwaSrodek, string opis = Defaults.SrodekOpisSrodek, string rodzaj = Defaults.SrodekKlasyfikacja)
            => Session
                .InTransUIRes(sess => sess.AddRow(new SrodekTrwaly()))
                .Box()
                .ConditionallyObj(numerInw, (λ, v) => λ.SetNumerInw(v))
                .ConditionallyObj(nazwa, (λ, v) => λ.SetNazwa(v))
                .ConditionallyObj(opis, (λ, v) => λ.SetOpis(v))
                .ConditionallyObj(rodzaj, (λ, v) => λ.SetKlasyfikacja(v));

        public ProxyRecord<RaportESP> NewRaportESP([NotNull] ResolverEwidencjaSP ewidencjaSP, FromTo? okres = null)
            => Session
                .InTransUIRes(sess => sess.AddRow(new RaportESP(ewidencjaSP.Resolve(sess), okres ?? FromTo.Empty)))
                .Box();
                public ProxyRecord<KontoBase> NewKontoSyntetyczne(string segment, string nazwa = null, TypKonta typ = TypKonta.Aktywa, ResolverOkres okres = null, OddzialFirmy firma = null, OddzialFirmy oddzial = null)
            => Session.InTransUIRes(sess => sess.AddRow(new KontoSyntetyczne((okres ?? SelectorOkres.Standardowy).Resolve(Session), firma) { OddzialFirmyInitiator = oddzial }))
                .Box()
                .SetSegment(segment)
                .SetNazwa(nazwa ?? segment)
                .Conditionally(typ != TypKonta.Aktywa, λ => λ.SetTyp(typ));

        public ProxyRecord<ZakupEwidencja, DokEwidencji> NewZakupEwidencja([NotNull] ResolverPodmiot podmiot, string numer = Defaults.EwidencjaNumerDokumentuZakup, string opis = Defaults.EwidencjaOpisDokumentuZakup, Currency? wartosc = null, Date? data = null, OddzialFirmy oddzial = null)
            => Session
                .InTransUIRes(sess => sess.AddRow(new ZakupEwidencja()))
                .Box()
                .SetPodmiot(podmiot)
                .ConditionallyVal(data, (λ, v) => λ.SetDataWplywu(v))
                .ConditionallyObj(numer, (λ, v) => λ.SetNumerDokumentu(v))
                .ConditionallyObj(opis, (λ, v) => λ.SetOpis(v))
                .ConditionallyVal(wartosc, (λ, v) => λ.SetWartosc(v))
                .ConditionallyObj(oddzial, (λ, v) => λ.SetOddzial(v));

        public ProxyRecord<KontoBase> NewKontoAnalityczne(KontoBase nadrzedne, string segment, string nazwa = null, TypKonta typ = TypKonta.Aktywa, ResolverOkres okres = null)
            => Session.InTransUIRes(sess => sess.AddRow(new KontoAnalityczne((okres ?? SelectorOkres.Standardowy).Resolve(Session), nadrzedne)))
                .Box()
                .SetSegment(segment)
                .SetNazwa(nazwa ?? segment)
                .Conditionally(typ != TypKonta.Aktywa, λ => λ.SetTyp(typ));
        public ProxyRecord<SprzedazEwidencja, DokEwidencji> NewSprzedazEwidencja([NotNull] ResolverPodmiot podmiot, string numer = Defaults.EwidencjaNumerDokumentuSprzedaz, string opis = Defaults.EwidencjaOpisDokumentuSprzedaz, Currency? wartosc = null, Date? data = null, OddzialFirmy oddzial = null)
            => Session
                .InTransUIRes(sess => sess.AddRow(new SprzedazEwidencja()))
                .Box()
                .SetPodmiot(podmiot)
                .ConditionallyVal(data, (λ, v) => λ.SetDataWplywu(v))
                .ConditionallyObj(numer, (λ, v) => λ.SetNumerDokumentu(v))
                .ConditionallyObj(opis, (λ, v) => λ.SetOpis(v))
                .ConditionallyVal(wartosc, (λ, v) => λ.SetWartosc(v))
                .ConditionallyObj(oddzial, (λ, v) => λ.SetOddzial(v));

        public ProxyRecord<PKEwidencja, DokEwidencji> NewPKEwidencja(string numer = Defaults.EwidencjaNumerDokumentuPK, string opis = Defaults.EwidencjaOpisDokumentuPK, Date? data = null, ResolverDefinicjaDokumentu definicja = null, OddzialFirmy oddzial = null)
        => Session
                   .InTransUIRes(sess => sess.AddRow(new PKEwidencja()))
                   .Box()
                   .ConditionallyObj(definicja, (λ, v) => λ.SetDefinicja(v))
                   .ConditionallyVal(data, (λ, v) => λ.SetDataWplywu(v))
                   .ConditionallyObj(numer, (λ, v) => λ.SetNumerDokumentu(v))
                   .ConditionallyObj(opis, (λ, v) => λ.SetOpis(v))
                   .ConditionallyObj(oddzial, (λ, v) => λ.SetOddzial(v));

        public ProxyRecord<SchematPK> NewSchematKsiegowyPKEwdencja(string nazwa, ResolverOkres okres = null)
            => ConfigEditSession
                .InTransUIRes(sess => sess.AddRow(new SchematPK(okres: (okres ?? SelectorOkres.Standardowy).Resolve(sess))))
                .Box()
                .SetNazwa(nazwa);

        public ProxyRecord<PozycjaSchematuPK> NewPozycjaSchematuPKEwdencja(SchematPK schemat)
            => ConfigEditSession
                .InTransUIRes(sess => sess.AddRow(new PozycjaSchematuPK(schemat)))
                .Box();


        public ProxyRecord<OkresObrachunkowy> NewOkresObrachunkowy(FromTo? okres = null, string symbol = null, TypOkresuObrachunkowego typOkresu = TypOkresuObrachunkowego.KS)
        {
            return ConfigEditSession
                .InTransUIRes(sess => sess.AddRow(NewOkres()))
                .Box()
                .ConditionallyVal(okres, (λ, v) => λ.SetOkres(v))
                .ConditionallyObj(symbol, (λ, v) => λ.SetSymbol(v));

            OkresObrachunkowy NewOkres()
            {
                switch (typOkresu)
                {
                    case TypOkresuObrachunkowego.Ryczałt:
                        return new OkresObrachunkowyRyczałt();
                    case TypOkresuObrachunkowego.KPiR:
                        return new OkresObrachunkowyKPiR();
                    default:
                        return new OkresObrachunkowyKS();
                }
            }
        }

    }
}
