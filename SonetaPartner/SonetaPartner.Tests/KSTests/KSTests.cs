using FluentAssertions;
using NUnit.Framework;
using Soneta.Business;
using Soneta.Core;
using Soneta.EwidencjaVat;
using Soneta.Kasa;
using Soneta.Kasa.Extensions;
using Soneta.Ksiega;
using Soneta.Ksiega.PKRozrachunkow;
using Soneta.SrodkiTrwale;
using Soneta.Test;
using Soneta.Types;
using SonetaPartner.Tests.Assemblers;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Settings;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Wrappers;
using System.Linq;

namespace SonetaPartner.Tests.KSTests
{
    internal class KSTests : TestKsiegowosc
    {
        [Test]
        public void GenerowaniePrzelewuMPPDlaDokumentuONumerzeZawierajacymCiagZnakow_Test()
        {
            ((ResolverKontrahent) SelectorKontrahent.Abc)
                .Resolve(Session)
                .Box()
                .SetSposobZaplaty(SelectorFormaPlatnosci.PrzelewMPP)
                .NewRachunek(out var rachunek, "88999900005555566666777770");

            var kontrahentABC = GetFinder().Kontrahent("Abc");

            kontrahentABC.SposobZaplaty.Nazwa.Should().Be("Przelew MPP");
            kontrahentABC.Rachunki.Count.Should().Be(1);

            NewZakupEwidencja(SelectorPodmiot.Abc, "FS/VAT/0018", "zakup materiałów")
                .NewElementVAT(220, stawkaVAT: SelectorStawkaVAT.Podstawowa)
                .SetZatwierdzona()
                .GoSave()
                .Out(out var zakup);

            var zobowiazanie = zakup.Row.Platnosci.GetFirst().ExpectType<Zobowiazanie>();
            zobowiazanie.Podmiot.Kod.Should().Be("Abc");
            zobowiazanie.SposobZaplaty.Nazwa.Should().Be("Przelew MPP");
            zobowiazanie.Kwota.Should().Be(270.60.Zloty());
            zobowiazanie.KwotaMPP.Should().Be(50.60.Zloty());

            WorkerNaliczaniePrzelewowEwidencji(GetFinder().DokumentyEwidencji(), prms => prms.ZEwidencjiZrodlowej = true)
                .ExecuteResult()
                .Out(out var przelew);

            var przelewPierwszy = przelew[0];
            var opis = przelew[0].Opis.Split(';');

            przelewPierwszy.Typ2.Should().Be(TypPrzelewu2.PrzelewMPP);
            przelewPierwszy.Podmiot.Kod.Should().Be("Abc");
            przelewPierwszy.RachunekOdbiorcy.PełnyNRB.Should().Be("88 9999 0000 5555 5666 6677 7770");
            przelewPierwszy.Kwota.Should().Be(270.60.Zloty());

            opis[0].Should().Be("1111111128");
            opis[1].Should().Be("FS/VAT/0018");
            opis[2].Should().Be("Z dnia " + Date.Today);
        }

        [Test]
        public void KsiegowanieWgKont_Test()
        {
            GetFinder()
                .Konto("700-01")
                .Out(out var konto7);

            NewKontoAnalityczne(konto7, "materiały", "materiały")
                .Out(out var materialy);

            materialy.Row.Kod.Should().Be("700-01-materiały");

            NewKontoAnalityczne(konto7, "produkty", "produkty")
                .Out(out var produkty);

            produkty.Row.Kod.Should().Be("700-01-produkty");

            NewKontoAnalityczne(konto7, "towary", "towary")
                .GoSave()
                .Out(out var towary);

            towary.Row.Kod.Should().Be("700-01-towary");

            NewSprzedazEwidencja(SelectorKontrahent.Drynda, "SP/T/0001", "sprzedaż")
                .SetDefinicja("SPU")
                .SetWartosc(3300m)
                .GoSave()
                .Out(out var sprzedaz);

            sprzedaz.Resync(Session);

            sprzedaz
                .NewElementOpisuEwidencji(wymiar: "7", symbol: materialy.Row.Kod, 1000m)
                .NewElementOpisuEwidencji(wymiar: "7", symbol: produkty.Row.Kod, 1500m)
                .NewElementOpisuEwidencji(wymiar: "7", symbol: towary.Row.Kod, 800m)
                .GoSave();

            sprzedaz.Resync(Session);

            sprzedaz
                .ExecutePredekretuj(Context, out var result);

            var zapisy = result.GetZapisy();

            zapisy.Should().HaveCount(4);
            zapisy.Should().Contain(λ => λ.Konto.Symbol == "200-Drynda" && λ.KwotaOperacji.Value == 3300m && λ.Strona == Soneta.Core.StronaKsiegowania.Winien);
            zapisy.Should().Contain(λ => λ.Konto.Symbol == materialy.Row.Kod && λ.KwotaOperacji.Value == 1000m && λ.Strona == Soneta.Core.StronaKsiegowania.Ma);
            zapisy.Should().Contain(λ => λ.Konto.Symbol == produkty.Row.Kod && λ.KwotaOperacji.Value == 1500m && λ.Strona == Soneta.Core.StronaKsiegowania.Ma);
            zapisy.Should().Contain(λ => λ.Konto.Symbol == towary.Row.Kod && λ.KwotaOperacji.Value == 800m && λ.Strona == Soneta.Core.StronaKsiegowania.Ma);

            GetFinder(true)
                .SchematyKsiegowe()
                .Where(s => s.Nazwa.Contains("Polecenie księgowania"))
                .First()
                .Box()
                .SetBlokada()
                .Out(out var schemat1);

            schemat1.Row.Blokada.Should().Be(true);

            NewPKEwidencja("PK/WF1", "przeksięgowanie kont 700-01 na konto 860-01")
                .GoSave();

            NewSchematKsiegowyPKEwdencja("PK WF")
                .SetTrybEdycji(TrybEdycjiSchematu.Zaawansowany)
                .SetWarunek("Zawiera(Ewidencja.Opis,\"700-01\")")
                .SetOpis("Zamknięcie kont 700-01")
                .Out(out var schemat2)
                .GoSave();

            schemat2.Resync(ConfigEditSession);

            NewPozycjaSchematuPKEwdencja(schemat2.Row)
                .SetTrybEdycji(TrybEdycjiSchematu.Zaawansowany)
                .SetPrzedmiotKs("KontaWgFiltra(\"700-01-*\")")
                .SetStrona("{StronaKsiegowania.Winien}")
                .SetKonto("{false,KontoWgSymbolu(Kontekst.Symbol)}")
                .SetKwotaOperacji("Obroty(Kontekst.Symbol,\"Ma\")")
                .SetOpis("przeksięgowanie na WF")
                .Out(out var poz1);

            WorkerKopiujPozycjeSchematuPK(poz1.Row)
                .Execute(out var poz2);

            poz2.Box()
                .SetStrona("{StronaKsiegowania.Ma}")
                .SetKonto("{true,KontoWgSymbolu(\"860-01\"),(Kontekst.Symbol.Substring(7,Kontekst.Symbol.Length-7))}")
                .GoSave();

            GetFinder()
                .DokumentyEwidencji(TypDokumentu.PKEwidencja)
                .InCollection<PKEwidencja>()
                .Box()
                .ExecutePredekretuj(Context, out var rezultat)
                .GoSave();

            var wrapper = new WrapperDekrety(rezultat);

            wrapper.DCount.Should().Be(1);
            wrapper.ZCount.Should().Be(6);

            wrapper.Select("700-01-materiały", StronaKsiegowania.Winien, 1000m).Should().NotBeEmpty();
            wrapper.Select("860-01-materiały", StronaKsiegowania.Ma, 1000m).Should().NotBeEmpty();
            wrapper.Select("700-01-produkty", StronaKsiegowania.Winien, 1500m).Should().NotBeEmpty();
            wrapper.Select("860-01-produkty", StronaKsiegowania.Ma, 1500m).Should().NotBeEmpty();
            wrapper.Select("700-01-towary", StronaKsiegowania.Winien, 800m).Should().NotBeEmpty();
            wrapper.Select("860-01-towary", StronaKsiegowania.Ma, 800m).Should().NotBeEmpty();
        }

        [Test]
        public void AnalitykaPlanuKont_Wyjatki_Test()
        {
            var analityka = GetFinder().KontaWgLike("580").ToList().First();

            analityka
                .Box()
                .NewDefinicjaAnalityki(DefinicjaSlownika.Towary)
                .NewDefinicjaAnalityki()
                .NewDefinicjaAnalityki();

            var definicjaZWyjatkami = analityka.DefinicjeAnalityk.Where(x => x.DefinicjaSlownika == null).Last();

            definicjaZWyjatkami
                .Box()
                .NewWyjatekDefinicjiAnalityki(out var wyjatek1)
                .NewWyjatekDefinicjiAnalityki(out var wyjatek2);

            wyjatek1
                .SetDefinicjaSlownika(DefinicjaSlownika.Magazyny)
                .SetWarunekZaawansowany("[Segment] = '01'");

            wyjatek2
                .SetDefinicjaSlownika(DefinicjaSlownika.EwidencjeSP)
                .SetWarunekZaawansowany("[Segment] = '02'");

            WorkerKontaSlownikowe("580", fnParams => fnParams.WybranyElementSlownika = GetFinder().Towar("Bikini"))
                .ExecuteAndSave();

            var bikini = GetFinder().KontaWgLike("580-BIKINI").ToList().First();

            bikini
                .Box()
                .NewKontoAnalityczne("01", "magazyny")
                .NewKontoAnalityczne("02", "ewidencje")
                .NewKontoAnalityczne("03", "inne");

            WorkerKontaSlownikowe("580-BIKINI-01")
                .ExecuteAndSave();

            WorkerKontaSlownikowe("580-BIKINI-02", fnParams => fnParams.WybranyElementSlownika = GetFinder().StdRachunekBankowyFirmy())
                .ExecuteAndSave();

            var bikini03 = GetFinder().KontaWgLike("580-BIKINI-03").ToList().First();

            bikini03
                .Box()
                .NewKontoAnalityczne("AD/1", "AD/1")
                .GoSave();

            var analityka580 = GetFinder().KontaWgLike("580*").ToList();

            analityka580.Count.Should().Be(8);
            analityka580.Where(x => x.Kod == "580-BIKINI").First().Should().NotBeNull();
            analityka580.Where(x => x.Kod == "580-BIKINI-01").First().Should().NotBeNull();
            analityka580.Where(x => x.Kod == "580-BIKINI-01-F").First().Should().NotBeNull();
            analityka580.Where(x => x.Kod == "580-BIKINI-02").First().Should().NotBeNull();
            analityka580.Where(x => x.Kod == "580-BIKINI-02-BANK").First().Should().NotBeNull();
            analityka580.Where(x => x.Kod == "580-BIKINI-03").First().Should().NotBeNull();
            analityka580.Where(x => x.Kod == "580-BIKINI-03-AD/1").First().Should().NotBeNull();
        }

        [Test]
        public void GenerujBilansOtwarcia_Test()
        {
            var view = Session.Get<CoreModule>().DokEwidencja.CreateView();
            view.Context = Context;

            new SessionCsvReader { DataSource = view }.Read(ImportData);

            GetFinder().DokumentyEwidencji(TypDokumentu.SprzedażEwidencja)
                .Cast<SprzedazEwidencja>()
                .ProcessAsList(λ => λ.Box().SetZatwierdzona().ExecutePredekretuj(Context));

            GetFinder().DokumentyEwidencji(TypDokumentu.ZakupEwidencja)
                .Cast<ZakupEwidencja>()
                .ProcessAsList(λ => λ.Box().SetZatwierdzona().ExecutePredekretuj(Context));

            NewOkresObrachunkowy(FromTo.Year(Defaults.KolejnyOkres), $"{Defaults.KolejnyOkres}", TypOkresuObrachunkowego.KS)
               .SetOpis($"ROK OBRACHUNKOWY {Defaults.KolejnyOkres}")
               .GoSave()
               .Out(out var okres);

            WorkerGenerujBoKontaSchematy(okres.Row)
                .ExecuteAndSave();

            var bos = GenerujBilansOtwarciaSalda(Defaults.KolejnyOkres, GenerujBoSaldaParams.TrybGenerowania.Razem);

            bos.Errors.Cast<object>().Should().BeEmpty();
            bos.Dekrety.Cast<object>().Should().HaveCount(1);

            var zapisyBos = bos.GetZapisy();
            zapisyBos.Should().HaveCount(2);
            zapisyBos.Should().Contain(λ => λ.Konto.Symbol == "401-01" && λ.StronaRozliczenia == StronaKsiegowania.Winien && λ.KwotaZapisu == 210.Zloty());
            zapisyBos.Should().Contain(λ => λ.Konto.Symbol == "701-01" && λ.StronaRozliczenia == StronaKsiegowania.Ma && λ.KwotaZapisu == 7453.51.Zloty());

            var boz = GenerujBilansOtwarciaZapisy(Defaults.KolejnyOkres, GenerujBoSaldaParams.TrybGenerowania.Razem, "2?2-*");

            boz.Errors.Cast<object>().Should().BeEmpty();
            boz.Dekrety.Cast<object>().Should().HaveCount(1);

            var zapisyBoz = boz.GetZapisy();
            zapisyBoz.Should().HaveCount(7);
            zapisyBoz.Should().Contain(λ => λ.Lp == 1 && λ.Konto.Symbol == "202-Klon" && λ.StronaRozliczenia == StronaKsiegowania.Ma && λ.KwotaZapisu == 135.30.Zloty());
            zapisyBoz.Should().Contain(λ => λ.Lp == 2 && λ.Konto.Symbol == "202-Kobra" && λ.StronaRozliczenia == StronaKsiegowania.Ma && λ.KwotaZapisu == 123.Zloty());
            zapisyBoz.Should().Contain(λ => λ.Lp == 3 && λ.Konto.Symbol == "222-01" && λ.StronaRozliczenia == StronaKsiegowania.Ma && λ.KwotaZapisu == 230.Zloty());
            zapisyBoz.Should().Contain(λ => λ.Lp == 4 && λ.Konto.Symbol == "222-01" && λ.StronaRozliczenia == StronaKsiegowania.Ma && λ.KwotaZapisu == 230.Zloty());
            zapisyBoz.Should().Contain(λ => λ.Lp == 5 && λ.Konto.Symbol == "222-01" && λ.StronaRozliczenia == StronaKsiegowania.Ma && λ.KwotaZapisu == 283.82.Zloty());
            zapisyBoz.Should().Contain(λ => λ.Lp == 6 && λ.Konto.Symbol == "222-01" && λ.StronaRozliczenia == StronaKsiegowania.Ma && λ.KwotaZapisu == 690.Zloty());
            zapisyBoz.Should().Contain(λ => λ.Lp == 7 && λ.Konto.Symbol == "222-01" && λ.StronaRozliczenia == StronaKsiegowania.Ma && λ.KwotaZapisu == 280.49.Zloty());

        }

        [Test]
        public void ZestawienieCIT8_Test()
        {
   
            var zestawienie = GetFinder(true).ZestawienieKsiegowe(TypDefinicjiZestawienia.CIT8).Box();
            InUIConfigTransaction(() =>
            {

                zestawienie.GetPozycjaByTyp(TypPozycjiZestawienia.Przychody).SetWyrazenie("ObrotyMa(\"7??-*\")");
                zestawienie.GetPozycjaByTyp(TypPozycjiZestawienia.Koszty).SetWyrazenie("ObrotyWn(\"4??-*\")");
            });
            SaveDisposeConfig();

            NewPKEwidencja(data: (11, 1).AsDemoDate())
                .NewDekret(fn: λ =>
                {
                    λ.NewZapis("700-01", StronaKsiegowania.Ma, 10000.51m);
                    λ.NewZapis("400-01", StronaKsiegowania.Winien, 3335.01m);
                    λ.NewZapis("301-01", StronaKsiegowania.Winien, 6665.50m);
                })
                .GoSave();

            zestawienie
                .Resync(Session)
                .ExecuteOblicz()
                .Out(out var wynik);

            wynik.Pozycje[TypPozycjiZestawienia.Przychody][TypKolumnyZestawienia.Wartość].Wartosc.Should().Be(10000.51m);
            wynik.Pozycje[TypPozycjiZestawienia.Koszty][TypKolumnyZestawienia.Wartość].Wartosc.Should().Be(3335.01m);
        }

        [Test]
        public void GenerowaniePKRozrachunkow_Test()
        {
            var sprzedaz = NewSprzedazEwidencja(SelectorPodmiot.Abc, wartosc: new Currency(200.0))
               .NewDekret(fn: λ =>
               {
                   λ.NewZapis("200", StronaKsiegowania.Winien, 200, resolverEk: SelectorElementKsiegowalny.PlatnoscEwidencji);
                   λ.NewZapis("701-01", StronaKsiegowania.Ma, 200);
               })
               .GoSave();

            sprzedaz.Row.Platnosci.Cast<Platnosc>().Should().Contain(λ => λ.Należność == 200);

            var wrk = new PKRozrachunkowWorker();
            wrk.Param = new PKRozrachunkowWorker.Params(Context);
            wrk.Rozrachunki = GetFinder().Rozrachunki().ToArray();

            wrk.Param.Numer = "PK1";
            wrk.Param.Opis = "przeksięgowanie rozrachunku";
            wrk.Generuj();

            var pk = (PKEwidencja) GetFinder()
                .DokumentyEwidencji()
                .FirstOrDefault(x => x.Typ == TypDokumentu.PKEwidencja);

            var platnosci = pk.Platnosci.Cast<Platnosc>().ToList();
            platnosci.Should().HaveCount(2);
            platnosci.Should().Contain(λ => λ.Zobowiązanie == 200 && λ.KwotaRozliczona == 200 && λ.Podmiot.Kod == "Abc");
            platnosci.Should().Contain(λ => λ.Należność == 200 && λ.Podmiot.Kod == "Abc");

            pk.Box()
                .NewDekret(fn: λ =>
                {
                    λ.NewZapis("202", StronaKsiegowania.Winien, 200, resolverEk: SelectorElementKsiegowalny.NaleznoscEwidencji);
                    λ.NewZapis("200", StronaKsiegowania.Ma, 200, resolverEk: SelectorElementKsiegowalny.ZobowiazanieEwidencji);
                })
                .GoSave();

            var zapisy = pk.Dekrety.InCollection<DekretBase>().Zapisy.ToList();

            zapisy.Should().HaveCount(2);
            zapisy.Should().Contain(x => x.Konto.Symbol == "200-Abc" && x.StanRozliczenia == StanRozliczenia.Calkowicie);
            zapisy.Should().Contain(x => x.Konto.Symbol == "202-Abc" && x.StanRozliczenia == StanRozliczenia.Nierozliczony);
        }

        [Test]
        public void GenerowaniePlanuKontDlaZablokowaneElementuSlownika_Test()
        {
            var glowneCentrumKosztow = GetFinder(true).StdCentrumKosztow().Nazwa;
            var symbolKonta = "000";

            NewKontoSyntetyczne(symbolKonta, "TEST")
                .NewDefinicjaAnalityki(DefinicjaSlownika.CentraKosztow, λ => λ.SetGenerujAnalityke());

            GetFinder().Konto(symbolKonta).Should().NotBeNull();

            WorkerKontaSlownikowe(symbolKonta).ExecuteAndSave();

            GetFinder().KontaWgLike($"{symbolKonta}-*").ToList().Should().HaveCount(7);
            GetFinder().KontaWgLike($"{symbolKonta}-{glowneCentrumKosztow}").Should().NotBeEmpty();

            Settings<CoreModule>().Ogolne_CentraKosztow_Zablokowane(glowneCentrumKosztow);

            NewOkresObrachunkowy(FromTo.Year(Defaults.KolejnyOkres))
                .GoSave()
                .Out(out var kolejnyOkres);

            WorkerGenerujBoKontaSchematy(kolejnyOkres.Row).ExecuteAndSave();

            GetFinder().KontaWgLike(kolejnyOkres.Row, $"{symbolKonta}-*").ToList().Should().HaveCount(6);
            GetFinder().KontaWgLike(kolejnyOkres.Row, $"{symbolKonta}-{glowneCentrumKosztow}").Should().BeEmpty();
        }

        [Test]
        public void SplitPaymentZakup_Test()
        {
            GetFinder()
                .Kontrahent("Klon")
                .Box()
                .SetSposobZaplaty(SelectorFormaPlatnosci.Przelew)
                .NewRachunek("PL78123456781111222233334444")
                .GoSave();

            NewZakupEwidencja(SelectorPodmiot.Klon, numer: "ZK/00883/21", opis: "zakup mpp")
                .NewElementVAT(14500, stawkaVAT: SelectorStawkaVAT.Podstawowa, grupa: GrupaElementuVAT.MPP)
                .SetZatwierdzona()
                .GoSave();

            var zobowiazanie = GetFinder().Platnosci().InCollection<Zobowiazanie>();
            zobowiazanie.Kwota.Value.Should().Be(17835);
            zobowiazanie.SposobZaplaty.Nazwa.Should().Be("Przelew MPP");
            zobowiazanie.KwotaMPP.Value.Should().Be(3335);

            WorkerNaliczaniePrzelewow(GetFinder().DokumentyEwidencji(), prms =>
            {
                prms.ZEwidencjiZrodlowej = true;
            })
                .ExecuteResult()
                .InCollection<Przelew>()
                .Out(out var przelew);

            przelew.Typ2.Should().Be(TypPrzelewu2.PrzelewMPP);
            przelew.Podmiot.Kod.Should().Be("Klon");
            przelew.Rachunek.Rachunek.Numer.Pełny.Should().Be("PL 78 1234 5678 1111 2222 3333 4444");
            przelew.Kwota.Value.Should().Be(17835);
            przelew.MPP.Opis.Should().Be("Z dnia " + Date.Today);

            przelew
                .Box()
                .SetZatwierdzony()
                .GoSave();

            NewRaportESP(SelectorEwidencjaSP.FirmowyRachunekBankowy);
            WorkerWyciagBankowy(GetFinder().Przelewy())
                .Execute(out var wyplaty)
                .GoSave();

            wyplaty.Should().HaveCount(1);

            wyplaty
                .InCollection(fnSelector: λ => λ.Podmiot.Kod == "Klon")
                .With(λ =>
                {
                    λ.Kwota.Value.Should().Be(17835);
                    λ.KwotaMPP.Value.Should().Be(3335);
                    λ.NumeryDokumentow.Should().Be("ZK/00883/21");
                    λ.Opis.Should().Be("ZK/00883/21; Z dnia " + Date.Today);
                    λ.Rozliczana.Should().BeTrue();
                    λ.StanRozliczenia.Should().Be(StanRozliczenia.Calkowicie);
                    λ.RozliczoneDokumenty.Should().Be("ZK/00883/21");
                });
        }

        [Test]
        public void OkresObrachunkowy_Test()
        {
            NewKontoSyntetyczne(segment: "999", nazwa: "Test").GoSave();

            var year = Defaults.Okres + 1;

            NewOkresObrachunkowy(FromTo.Year(year), $"{year}", TypOkresuObrachunkowego.KS)
               .SetOpis($"ROK OBRACHUNKOWY {year}")
               .GoSave()
               .Out(out var okres);

            Session
                .Get<KsiegaModule>()
                .OkresyObrach
                .WgOkresu
                .Select(o => o.Symbol)
                .ToArray()
                .Should()
                .Equal($"{Defaults.Okres}", $"{Defaults.Okres + 1}");


            if (okres.Row.SchematyKsiegowe.Count == 0)
            {
                WorkerBoKontaSchematyGenerator(λ =>
                {
                    λ.Okres = ConfigEditSession.InSession(okres.Row);
                    λ.SetPoprzedniOkres(GetFinder(true).StdOkresObrachunkowy());
                    λ.OtherSession = ConfigEditSession;
                })
                    .ExecuteAndSave();
            }

            Session
                .Get<KsiegaModule>()
                .Konta
                .CreateView()
                .Setup(view => view.Condition = new FieldCondition.Like("Symbol", "999"))
                .Cast<KontoBase>()
                .ToList()
                .Should()
                .HaveCount(2);

            Session
                .Get<KsiegaModule>()
                .SchematyKsiegowe
                .CreateView()
                .Setup(view => view.Condition = new FieldCondition.Like("Nazwa", "Sprzedaż Ewidencja"))
                .Cast<SchematSprzedazy>()
                .ToList()
                .Should()
                .HaveCount(2);
        }

        [Test]
        public void UtworzenieSrodkaTrwalego_Test()
        {
            var komputer = NewSrodekTrwaly(nazwa: "komputer As322", opis: "komputer", rodzaj: "4")
                .SetNumerFabryczny("1237yt")
                .SetCentrumKosztow(SelectorCentrumKosztow.KosztySprzedazy)
                .SetOdpowiedzialny("007")
                .SetRozpoczecieAmortyzacjiWMiesiacuUzytkowania(false)
                .SetAmortyzacja(MetodaAmortyzacji.Liniowa, new Percent(2, 100))
                .SetPozycjaTerminarzaInwentarza("instalacja", new Date(2021, 6, 17), Date.Empty)
                .GoSave()
                .Resync(Session);

            komputer.NewOT(λ => λ
                    .SetData(new Date(2021, 6, 1))
                    .SetDataOperacji(new Date(2021, 6, 1))
                    .SetWartosc(4900)
                    .InTransUI((ot) => {
                        new ZatwierdzOdtwierdzDokumentSTWorker { Dokument = ot.Row }.Zatwierdz();
                    })
                    ).GoSave()
                    .Resync(Session);

            var wsw = new WartosciSrodkaWorker();
            wsw.SrodekHistoria = komputer.Row[Day()];
            wsw.Data = Day();
            wsw.WartoscPoczatkowaBilansowa.Should().Be(new Currency(4900.00, "PLN"));
            wsw.WartoscPoczatkowaPodatkowa.Should().Be(new Currency(4900.00, "PLN"));

            var drukarka = NewSrodekTrwaly(nazwa: "drukarka HP234", opis: "drukarka", rodzaj: "4")
                .SetNumerFabryczny("HP123")
                .SetCentrumKosztow(SelectorCentrumKosztow.KosztyWydzialowe)
                .SetOdpowiedzialny("008")
                .SetRozpoczecieAmortyzacjiWMiesiacuUzytkowania(false)
                .SetAmortyzacja(MetodaAmortyzacji.Degresywna, new Percent(15, 100))
                .SetDataBO(new Date(2021, 06, 01))
                .SetWartoscBO(3600, 3000, 500, 2900, 100)
                .GoSave();


            wsw.SrodekHistoria = drukarka.Row[Day()];
            wsw.Data = Day();
            wsw.WartoscPoczatkowaBilansowa.Should().Be(new Currency(3600.00, "PLN"));
            wsw.WartoscPoczatkowaPodatkowa.Should().Be(new Currency(3600.00, "PLN"));
        }


        private ManagerKsiegowan.Rezultat GenerujBilansOtwarciaSalda(
            ResolverOkres resolverOkres,
            GenerujBoSaldaParams.TrybGenerowania trybGenerowania = GenerujBoSaldaParams.TrybGenerowania.WgSyntetyk,
            string filtrKont = null)
            => WorkerGenerujBoSalda(resolverOkres, prms =>
            {
                prms.Tryb = trybGenerowania;
                prms.Konto = filtrKont ?? string.Empty;
            })
                .Execute(true)
                .Result;

        private ManagerKsiegowan.Rezultat GenerujBilansOtwarciaZapisy(
            ResolverOkres resolverOkres,
            GenerujBoSaldaParams.TrybGenerowania trybGenerowania = GenerujBoSaldaParams.TrybGenerowania.WgSyntetyk,
            string filtrKont = null, bool kopiujCechyZZapisu = false, bool kopiujOpisZZapisu = false)
            => WorkerGenerujBoZapisy(resolverOkres, prms =>
            {
                prms.Tryb = trybGenerowania;
                prms.Konto = filtrKont ?? string.Empty;
                prms.KopiujCechyZZapisu = kopiujCechyZZapisu;
                prms.KopiujOpisZZapisu = kopiujOpisZZapisu;
            })
                .Execute(true)
                .Result;


        private const string ImportData =
    @"Class;Definicja:Symbol;NumerDokumentu;Opis;Podmiot:Class;Podmiot:Kod;Platnosci:Class;Platnosci:Kwota;Platnosci:Opis;NagEwidencjiVAT.Elementy:Class;NagEwidencjiVAT.Elementy:DefinicjaStawki:Kod;NagEwidencjiVAT.Elementy:Netto;NagEwidencjiVAT.Elementy:VAT;OpisAnalityczny:Class;OpisAnalityczny:Wymiar;OpisAnalityczny:Symbol;OpisAnalityczny:Kwota;;;
SprzedazEwidencja;SPU;SP/12345;sprzedaż towarów ;Kontrahent;ABC;;;;;;;;;;;;;;
;;;;;;Naleznosc;1230;Opis;;;;;;;;;;;
;;;;;;;;;ElemEwidencjiVATSprzedaz;23%;1000;230;;;;;;;
;;;;;;;;;;;;;ElementOpisuEwidencji;7;701-01;1000;;;
SprzedazEwidencja;SPU;SP/12346;sprzedaż towarów ;Kontrahent;Drynda;;;;;;;;;;;;;;
;;;;;;Naleznosc;1230;Opis;;;;;;;;;;;
;;;;;;;;;ElemEwidencjiVATSprzedaz;23%;1000;230;;;;;;;
;;;;;;;;;;;;;ElementOpisuEwidencji;7;701-01;1000;;;
SprzedazEwidencja;SPU;SP/12347;sprzedaż towarów ;Kontrahent;Fiszbin;;;;;;;;;;;;;;
;;;;;;Naleznosc;1517,82;Opis;;;;;;;;;;;
;;;;;;;;;ElemEwidencjiVATSprzedaz;23%;1234;283,82;;;;;;;
;;;;;;;;;;;;;ElementOpisuEwidencji;7;701-01;1234;;;
SprzedazEwidencja;SPU;SP/12348;sprzedaż towarów ;Kontrahent;Zefir;;;;;;;;;;;;;;
;;;;;;Naleznosc;3690;Opis;;;;;;;;;;;
;;;;;;;;;ElemEwidencjiVATSprzedaz;23%;3000;690;;;;;;;
;;;;;;;;;;;;;ElementOpisuEwidencji;7;701-01;3000;;;
SprzedazEwidencja;SPU;SP/12349;sprzedaż towarów ;Kontrahent;Seno;;;;;;;;;;;;;;
;;;;;;Naleznosc;1500;Opis;;;;;;;;;;;
;;;;;;;;;ElemEwidencjiVATSprzedaz;23%;1219,51;280,49;;;;;;;
;;;;;;;;;;;;;ElementOpisuEwidencji;7;701-01;1219,51;;;
ZakupEwidencja;ZR;ZAK/11123;zakup towarów;Kontrahent;Kobra;;;;;;;;;;;;;;
;;;;;;Zobowiazanie;123;Opis;;;;;;;;;;;
;;;;;;;;;ElemEwidencjiVATZakup;23%;100;23;;;;;;;
;;;;;;;;;;;;;ElementOpisuEwidencji;4;401-01;100;;;
ZakupEwidencja;ZR;ZAK/11124;zakup towarów;Kontrahent;Klon;;;;;;;;;;;;;;
;;;;;;Zobowiazanie;135,3;Opis;;;;;;;;;;;
;;;;;;;;;ElemEwidencjiVATZakup;23%;110;25,3;;;;;;;
;;;;;;;;;;;;;ElementOpisuEwidencji;4;401-01;110;;;
";
    }
}
