using NUnit.Framework;
using Soneta.Deklaracje.PIT;
using Soneta.Kadry;
using Soneta.Kadry.Test;
using Soneta.Place;
using Soneta.Test;
using Soneta.Types;
using SonetaPartner.Tests.Assemblers;
using SonetaPartner.Tests.Extensions.KadryIPlace.Engine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SonetaPartner.Tests.KiPTests
{
    internal class KiPTests : PlTestBase
    {
        [Test]
        public void PIT_2_po_zakonczeniu_umowy_KUP_Test()
        {
            NowyTest()
                .NowyPracownik("001")
                .Zatrudnij("2022-1-1...2023-2-5", 6000.00m)
                .KosztyMnoznik(1.25m)
                .NaliczWyplaty("2023-01", "2023-02-01").Any().ForEach(wb => wb
                    .SprawdzKoszty(300m))
            .Utwórz();
        }

        [TestCase(1, "A", "A1")]
        public void Pracownik_Kod_Test(int d, string p, string kod)
        {

            Config().Enqueue((c, ctx) =>
            {
                var kadry = ctx.Session.Get<KadryModule>();
                kadry.Config.Ogólne.DługośćKoduPracownika = d;
                kadry.Config.Ogólne.PrefiksKoduPracownika = p;
            })
            .Utwórz();

            NowyTest()
            .NowyPracownik("Test123456", "Test123456")
            .Utwórz();

            NowyTest()
            .Enqueue((c, ctx) =>
            {
                var kadry = ctx.Session.Get<KadryModule>();
                Assert.AreEqual(kod, kadry.Pracownicy.WgNazwiska["Test123456"].First().Kod);
            })
            .Utwórz();
        }

        [Test]
        public void Umowa_Zlecenie_200_Zl_Test()
        {
            NowyTest().
            NowyPracownik("Kowalski", "Adam", "001").__Last().ForEach().
            GetChild<Umowa>().Enqueue(
                u =>
                {
                    u.Wydzial = u.Module.Wydzialy.Firma;
                    u.Data = new Date(2021, 03, 1);
                    u.Okres = new YearMonth(2021, 03).ToFromTo();
                    u.Tytul = "Umowa zlecenie 200 zł";
                    u.Element = u.Session.Get<PlaceModule>().DefElementow.WgNazwy[Element.Umowa_zlecenia_20];
                    u.Last.Wartosc = 200m;
                }).GetParent<Pracownik>().
            _NaliczWyplaty(FromTo.Parse("2021-03-01...31"), Date.Parse("2021-03-31")).ForEach(
                bw => bw.SprawdzSume("Zaliczka podatku", 34.0m, e => e.Podatki.ZalFIS)).
            Utwórz();
        }

        [Test]
        public void Naliczanie_umowy_stawkaMinimalna_kwota_Test()
        {
            bool minimalnaStawkaGodz = true;

            Umowa umowa = null;

            NowyTest().
            NowyPracownik().
            DodajUmowe(Element.Umowa_zlecenia_20, "2017-1", 100m).
                Enqueue(u => umowa = u).
                MinimalnaStawkaGodz(minimalnaStawkaGodz).
                DodajZestawienie("2017-1", "10:00").Return().
                Return().
            NaliczWyplaty("2017-1").Any().ForEach(
                bw => bw.SprawdzSume("Wartość 1", 130, e => e.Wartosc)).
            GetChild(_ => umowa).
                Last().Ustaw(200m).Return().
                Return().
            NaliczWyplaty("2017-2").Any().ForEach(
                bw => bw.SprawdzSume("Wartość 2", 70, e => e.Wartosc)).
            Utwórz();
        }

        [Test]
        public void Skladka_zdrowotna_korekta_ZUS_Test()
        {
            Config()
                .Enqueue(_ => _.Session.GetPlace().Config.SkładkiZUS.Ogólne.UproszczonyAlgorytmZdrowotnej = true)
            .Utwórz();

            NowyTest()
                .NowyPracownik("001")
                .Enqueue(p => p.Last.Podatki.Koszty50Procent = Percent.Parse("100%"))
                .Zatrudnij("2021-1-1...", 8682.00m)
                .NaliczWyplaty("2021-9", "2021-9-30", "2021-9", 0, null, "2023-1-31").Any().ForEach(wb => wb
                    .SprawdzSume("Składka zdrowotna 9% (1)", null, 593.06m, e => e.Podatki.Zdrowotna.Prac)
                    .SprawdzSume("Składka zdrowotna 7,75% (1)", null, 580.61m, e => e.Podatki.ZdrowotneDoOdliczenia)
                    .SprawdzDoWyplaty(6886.64m)
                    .Zatwierdz())
                .DodajDodatek(Element.Korekta_składek_ZUS, "2021-9-1...2021-9-30")
                .NaliczDodatek("2021-9", "2021-9-30", "2021-9", 0, null, "2023-1-31").Any().ForEach(wb => wb
                    .SprawdzSume("Składka zdrowotna 9% (2)", null, 0, e => e.Podatki.Zdrowotna.Prac)
                    .SprawdzSume("Składka zdrowotna 7,75% (2)", null, 0, e => e.Podatki.ZdrowotneDoOdliczenia)
                    .SprawdzDoWyplaty(0))
            .Build();
        }

        [Test]
        public void PIT_11_25_Okres_naliczania_Test()
        {
            NowyTest()
            .NowyPracownik("001")
            .Zatrudnij("2019-1-1...2019-9-30", 5000)
            .NaliczWyplaty("2019-8").Any().ForEach(wb => wb
                .SprawdzSume("Zaliczka podatku", null, 376, e => e.Podatki.ZalFIS)
                .SprawdzDoWyplaty(3550.19m))
            .Utwórz();

            NowyTest()
            .Pracownik("001")
            .NowyPIT<PIT11_25>(2019)
                .Enqueue(_ => Assert.IsTrue(_.Okres == FromTo.Parse("2019-1-1...2019-9-30"), _.Okres.ToString()))
                .Enqueue(_ => Assert.AreEqual(_.NumerFormularza, 1))
                .Enqueue(_ => Assert.AreEqual(_.CelZłożenia, PIT.CelZłożenia.ZłożenieFormularza))
                .SprawdzPole("E.1aPrzychód", 5000m)
                .Return()
            .Utwórz();

            NowyTest()
            .Pracownik("001")
            .NowyPIT<PIT11_25>(2019)
                .Enqueue(_ => Assert.IsTrue(_.Okres == FromTo.Parse("2019-1-1...2019-9-30"), _.Okres.ToString()))
                .Enqueue(_ => Assert.AreEqual(_.NumerFormularza, 1))
                .Enqueue(_ => Assert.AreEqual(_.CelZłożenia, PIT.CelZłożenia.KorektaFormularza))
                .SprawdzPole("E.1aPrzychód", 5000m)
                .Return()
            .Build();
        }

        [Test]
        public void OkresWypowiedzeniaNaOkresProbny_Test()
        {
            NowyTest().
            NowyPracownik("001").
                Last().
                    Zatrudnij("2016-5-1...14").
                    TypUmowyOPrace(TypUmowyOPrace.NaOkresPróbny).
                    Enqueue(ph => {
                        ph.Etat.OkresWypowiedzenia.DataZlozenia = DateTime.Parse("2016-5-2");
                        Assert.AreEqual(3, ph.Etat.OkresWypowiedzenia.Dni);
                    }).
            Utwórz();
        }

        [TestCase("2021-12-31", "1,0,0", RodzajPodstawyStażuPracy.StazPracy)]
        [TestCase("2021-12-31", "1,0,0", RodzajPodstawyStażuPracy.StazPracyWFirmie)]
        [TestCase("2022-12-31", "6,0,0", RodzajPodstawyStażuPracy.OkresNauki)]
        public void SprawdzenieStazu_Test(string data, string staz, RodzajPodstawyStażuPracy podstawa)
        {

            NowyTest()
            .NowyPracownik()
                .NowaUkonczonaSzkola("2016-9-1...2020-6-30", TypSzkoły.ŚredniaSzkołaOgólnokształcąca)//4 lata nauki
                    .Return()
                .Zatrudnij("2021-1-1...")//1 rok zatrudnienia
            .SprawdzStazPracy(data, staz, podstawa)
            .Build();
        }

        [Test]
        public void Przekroczenie_progu_podatkowego_2021_Test()
        {
            NowyTest()
                .NowyPracownik("001")
                .Zatrudnij("2021-1-1...", "1/1", 8600.00m)
                .DodajDodatek(Element.premia, "2022-02-01...28", 66554.50m)
                    .Return()
                .NaliczWyplaty(("2022-2"), "2022-02-26").Any().ForEach(wb => wb
                    .SprawdzDoWyplaty(48457.24m)
                .Zatwierdz())
                .NaliczWyplaty("2022-3").Any().ForEach(wb => wb
                    .SprawdzSume("ZalFis 2021", 600, e => e.Podatki2021.ZalFIS)
                    .SprawdzDoWyplaty(6153.06m)
                    .Zatwierdz())
                .NaliczWyplaty("2022-4").Any().ForEach(wb => wb
                    .SprawdzSume("ZalFis 2021", 600, e => e.Podatki2021.ZalFIS)
                    .SprawdzDoWyplaty(6153.06m)
                    .Zatwierdz())
                .NaliczWyplaty("2022-5").Any().ForEach(wb => wb
                    .SprawdzSume("ZalFis 2021", 688, e => e.Podatki2021.ZalFIS)
                    .SprawdzDoWyplaty(6132.06m)
                .Zatwierdz())
            .Build();
        }

        [Test]
        public void Ulga_podatkowa_umowa_i_etat_Test()
        {
            NowyTest()
                .NowyPracownik("001")
                .Enqueue(p => p.Last.Podatki.UlgaMnoznik = 0m)
                .Enqueue(p => p.Last.UmowaUlgi.UlgaMnoznik = 1m)
                .DodajUmowe(Element.Umowa_zlecenia_20, "01.01.2023...01.02.2023", 5000m)
                .NaliczUmowe("2023-1", "2023-02-01", "2023-2", 0).Any().ForEach(wb => wb
                    .SprawdzKoszty(887.40m)
                    .SprawdzUlge(300m)
                    .SprawdzSume("Zaliczka podatku", null, 126m, e => e.Podatki.ZalFIS)
                    .SprawdzDoWyplaty(3911.67m)
                    .Zatwierdz())
                    .Return()
                .Aktualizacja("2023-02-02")
                .Zatrudnij("2023-2-2...", 5000.00m)
                .Enqueue(p => p.Podatki.UlgaMnoznik = 1m)
                .Enqueue(p => p.UmowaUlgi.UlgaMnoznik = 0m)
                    .Return()
                .NaliczWyplaty("2023-2", "2023-02-20").Any().ForEach(wb => wb
                    .SprawdzKoszty(250m)
                    .SprawdzUlge(0m)
                    .SprawdzSume("Zaliczka podatku", null, 462m, e => e.Podatki.ZalFIS)
                    .SprawdzDoWyplaty(3267.88m))
            .Build();
        }
    }
}

