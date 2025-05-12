using System;
using Soneta.Test;
using Soneta.Types;
using Soneta.Business;
using Soneta.Kalend;
using Soneta.Place;
using Soneta.Kadry;
using NUnit.Framework;
using Soneta.CRM;
using Soneta.HR;
using Soneta.Core;
using Soneta.Tools;
using System.Linq;

namespace SonetaPartner.Tests.Assemblers
{

    #region PracownikAssembler

    public static class PracownikAssembler {

        static public IRowBuilder<Pracownik> Pracownik<T>(this IRowBuilder<T> builder, string kod) where T : Row {
            return builder.GetChild((row, ctx) => ctx.Session.Get<KadryModule>().Pracownicy.WgKodu[kod]);
        }

        static public IRowBuilder<Pracownik> NowyPracownik<T>(this IRowBuilder<T> builder, string imię, string nazwisko, string kod = null) where T : Row
            => builder.NowyPracownik<T, PracownikFirmy>(imię, nazwisko, kod);

        static public IRowBuilder<Pracownik> NowyPracownik<T, P>(this IRowBuilder<T> builder, string imię, string nazwisko, string kod = null, string kodW = null) where T : Row where P : Pracownik, new() {
            return builder.
                GetChild<Pracownik>((pracownik, ctx) => ctx.Session.AddRow(new P())).
                Enqueue(nowy => {
                    if (kod != null)
                        nowy.Kod = kod;
                    nowy.Last.Imie = imię;
                    nowy.Last.Nazwisko = nazwisko;
                    if (kodW != null)
                        nowy.Wlasciciel = nowy.Session.Get<KadryModule>().Pracownicy.WgKodu[kodW];
                });
        }

        static public IRowBuilder<Pracownik> NowyPracownik<T>(this IRowBuilder<T> builder, string kod = null) where T : Row {
            return builder.NowyPracownik("Jan", "Nowak", kod);
        }

        static public IRowBuilder<Pracownik> Zatrudnij(this IRowBuilder<Pracownik> builder, string okres, Currency? stawka = null, RodzajStawkiZaszeregowania? rodzajStawki = null) {
            return builder.Last().Zatrudnij(FromTo.Parse(okres), Fraction.One, stawka, rodzajStawki).Return();
        }

        static public IRowBuilder<Pracownik> Zatrudnij(this IRowBuilder<Pracownik> builder, string okres, string wymiar, Currency? stawka = null, RodzajStawkiZaszeregowania? rodzajStawki = null) {
            return builder.Last().Zatrudnij(FromTo.Parse(okres), Fraction.Parse(wymiar), stawka, rodzajStawki).Return();
        }

        static public IRowBuilder<Pracownik> KosztyMnoznik(this IRowBuilder<Pracownik> builder, decimal mnożnik)
            => builder.Last().KosztyMnożnik(mnożnik).Return();

        static public IRowBuilder<Pracownik> SprawdzStazPracy(this IRowBuilder<Pracownik> builder, string _data, string _staż, RodzajPodstawyStażuPracy rodzaj = RodzajPodstawyStażuPracy.StazPracy)
            => builder.Enqueue(p => {
                var current = p.StażPracy(Date.Parse(_data), rodzaj);
                Assert.AreEqual(StazPracy.Parse(_staż), current, $"Staż pracy na dzień '{_data}'.");
            });

    }

	#endregion
}

#region PracHistoriaAssemler

public static class PracHistoriaAssemler {

        static public IRowBuilder<PracHistoria> Last(this IRowBuilder<Pracownik> builder) {
            return builder.GetChild(pracownik => pracownik.Last);
        }

        static public IRowBuilder<PracHistoria> Aktualizacja(this IRowBuilder<Pracownik> builder, Date data) {
            return builder.GetChild(pracownik => pracownik[data]);
        }

        static public IRowBuilder<PracHistoria> Aktualizacja(this IRowBuilder<Pracownik> builder, string data) {
            return builder.Aktualizacja(Date.Parse(data));
        }

        static public IRowBuilder<Pracownik> Return(this IRowBuilder<PracHistoria> builder) {
            return builder.GetParent<Pracownik>();
        }

        static public IRowBuilder<PracHistoria> Zatrudnij(this IRowBuilder<PracHistoria> builder, string okres, Currency? stawka = null, RodzajStawkiZaszeregowania? rodzajStawki = null) {
            return builder.Zatrudnij(okres == null ? FromTo.Empty : FromTo.Parse(okres), Fraction.One, stawka, rodzajStawki);
        }

        static public IRowBuilder<PracHistoria> Zatrudnij(this IRowBuilder<PracHistoria> builder, FromTo? okres, Fraction? wymiar = null, Currency? stawka = null, RodzajStawkiZaszeregowania? rodzajStawki = null, TypStawkiZaszeregowania? typStawki = null) {
            return builder.Enqueue(ph => {
                if (okres != null && okres!=FromTo.Empty) {
                    ph.Etat.Okres = (FromTo)okres;
                    if (!ph.Etat.IsReadOnlyWydzial())
                        ph.Etat.Wydzial = ph.Module.Wydzialy.Firma;
                    ph.Etat.Stanowisko = "ST";
                }
                if (wymiar != null)
                    ph.Etat.Zaszeregowanie.Wymiar = (Fraction)wymiar;
                if (rodzajStawki != null)
                    ph.Etat.Zaszeregowanie.RodzajStawki = (RodzajStawkiZaszeregowania)rodzajStawki;
                if (stawka != null)
                    ph.Etat.Zaszeregowanie.Stawka = (Currency)stawka;
                if (typStawki != null)
                    ph.Etat.Zaszeregowanie.TypStawki = (TypStawkiZaszeregowania)typStawki;
            });
        }

        static public IRowBuilder<PracHistoria> TypUmowyOPrace(this IRowBuilder<PracHistoria> builder, TypUmowyOPrace typUmowyOPrace) {
            return builder.Enqueue(ph => ph.Etat.TypUmowy = typUmowyOPrace);
        }

        static public IRowBuilder<PracHistoria> KosztyMnożnik(this IRowBuilder<PracHistoria> builder, decimal mnożnik)
            => builder.Enqueue(ph=>ph.Podatki.KosztyMnoznik = mnożnik);

        #region Obsolete

        static public IRowBuilderCollection<TPracownik, PracHistoria> __Last<TPracownik>(this IRowBuilder<TPracownik> builder) where TPracownik : Pracownik {
            return builder.GetChildren<PracHistoria>(pracownik => new PracHistoria[] { pracownik.Last });
        }

        #endregion
    }

    #endregion