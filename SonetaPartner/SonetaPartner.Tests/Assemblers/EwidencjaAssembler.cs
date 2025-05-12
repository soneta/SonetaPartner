using System;
using JetBrains.Annotations;
using Soneta.Business;
using Soneta.Core;
using Soneta.EwidencjaVat;
using Soneta.Ksiega;
using Soneta.Types;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Settings;

namespace SonetaPartner.Tests.Assemblers
{
    public static class AssemblerEwidencja
    {

        #region DokEwidencji (VATEwidencja)

        public static ProxyRecord<ZakupEwidencja, DokEwidencji> NewElementVAT(this ProxyRecord<ZakupEwidencja, DokEwidencji> row
            , Currency? nettoOrBrutto = null
            , RodzajZakupuVAT? rodzaj = null
            , DzialalnoscGospodarcza? dzialalnosc = null
            , OdliczeniaVAT? odliczenia = null
            , GrupaElementuVAT? grupa = null
            , ResolverStawkaVAT stawkaVAT = null
            , bool asBrutto = false
            , Action<ProxyRecord<ElemEwidencjiVATZakup, ElemEwidencjiVAT>> fn = null)
            => row.NewElementVAT(out _, nettoOrBrutto, rodzaj, dzialalnosc, odliczenia, grupa, stawkaVAT, asBrutto, fn);

        public static ProxyRecord<ZakupEwidencja, DokEwidencji> NewElementVAT(this ProxyRecord<ZakupEwidencja, DokEwidencji> row
            , out ProxyRecord<ElemEwidencjiVATZakup, ElemEwidencjiVAT> newRow
            , Currency? nettoOrBrutto = null
            , RodzajZakupuVAT? rodzaj = null
            , DzialalnoscGospodarcza? dzialalnosc = null
            , OdliczeniaVAT? odliczenia = null
            , GrupaElementuVAT? grupa = null
            , ResolverStawkaVAT stawkaVAT = null
            , bool asBrutto = false
            , Action<ProxyRecord<ElemEwidencjiVATZakup, ElemEwidencjiVAT>> fn = null)
        {
            row.InTransUIRes(sess => sess.AddRow(new ElemEwidencjiVATZakup(row.Row)))
                .Box()
                .ConditionallyObj(stawkaVAT, (λ, v) => λ.SetStawkaVAT(v))
                .Conditionally(asBrutto && nettoOrBrutto.HasValue, λ => λ.SetBrutto(nettoOrBrutto.Value))
                .Conditionally(!asBrutto && nettoOrBrutto.HasValue, λ => λ.SetNetto(nettoOrBrutto.Value))
                .ConditionallyVal(grupa, (λ, v) => λ.SetGrupa(v))
                .ConditionallyVal(rodzaj, (λ, v) => λ.SetRodzajZakupu(v))
                .ConditionallyVal(dzialalnosc, (λ, v) => λ.SetDzialalnosc(v))
                .ConditionallyVal(odliczenia, (λ, v) => λ.SetOdliczenia(v))
                .Out(out newRow);

            fn?.Invoke(newRow);
            return row;
        }

        #endregion

        #region DokEwidencji

        public static ProxyRecord<T1, DokEwidencji> SetNumerDokumentu<T1>(this ProxyRecord<T1, DokEwidencji> row, string numerDokumentu = Defaults.EwidencjaNumerDokumentu)
            where T1 : DokEwidencji
            => row.InTransUI(λ => λ.Row.NumerDokumentu = numerDokumentu);

        public static ProxyRecord<T1, DokEwidencji> SetOpis<T1>(this ProxyRecord<T1, DokEwidencji> row, string opis = Defaults.EwidencjaOpisDokumentu)
            where T1 : DokEwidencji
            => row.InTransUI(λ => λ.Row.Opis = opis);

        public static ProxyRecord<T1, DokEwidencji> SetDataWplywu<T1>(this ProxyRecord<T1, DokEwidencji> row, Date dataWplywu)
            where T1 : DokEwidencji
            => row.InTransUI(λ => λ.Row.DataWplywu = dataWplywu);

        public static ProxyRecord<T1, DokEwidencji> SetWartosc<T1>(this ProxyRecord<T1, DokEwidencji> row, Currency wartosc)
            where T1 : DokEwidencji
            => row.InTransUI(λ => λ.Row.Wartosc = wartosc);

        public static ProxyRecord<T1, DokEwidencji> SetPodmiot<T1>(this ProxyRecord<T1, DokEwidencji> row, [NotNull] ResolverPodmiot podmiot)
            where T1 : DokEwidencji
            => row.InTransUI(λ => λ.Row.Podmiot = podmiot.Resolve(row));

        public static ProxyRecord<T1, DokEwidencji> SetDefinicja<T1>(this ProxyRecord<T1, DokEwidencji> row, [NotNull] ResolverDefinicjaDokumentu definicja)
            where T1 : DokEwidencji
            => row.InTransUI(λ => λ.Row.Definicja = definicja.Resolve(row));

        public static ProxyRecord<T1, DokEwidencji> SetZatwierdzona<T1>(this ProxyRecord<T1, DokEwidencji> row, StanEwidencji stan = StanEwidencji.Wprowadzony)
            where T1 : DokEwidencji
            => row.InTransUI(λ => λ.Row.Stan = stan);

        public static ProxyRecord<T1, DokEwidencji> NewElementOpisuEwidencji<T1>(this ProxyRecord<T1, DokEwidencji> row
            , string wymiar = Defaults.OpisAnalitycznyWymiar
            , string symbol = Defaults.OpisAnalitycznySymbol
            , Currency? kwota = null
            , Currency? kwotaDodatkowa = null
            , ResolverCentrumKosztow centrumKosztow = null
            , ResolverPozycjaBudzetu pozycjaBudzetu = null
            , Date? data = null
            , Amount? ilosc = null
            , bool wgKwotyDodatkowej = false
            , string opis = Defaults.OpisAnalitycznyOpis)
            where T1 : DokEwidencji
            => row.NewElementOpisuEwidencji(out _, wymiar, symbol, kwota, kwotaDodatkowa, centrumKosztow, pozycjaBudzetu, data, ilosc, wgKwotyDodatkowej, opis);

        public static ProxyRecord<T1, DokEwidencji> NewElementOpisuEwidencji<T1>(this ProxyRecord<T1, DokEwidencji> row
            , out ProxyRecord<ElemOpisuAnalitycznego> newRow
            , string wymiar = Defaults.OpisAnalitycznyWymiar
            , string symbol = Defaults.OpisAnalitycznySymbol
            , Currency? kwota = null
            , Currency? kwotaDodatkowa = null
            , ResolverCentrumKosztow centrumKosztow = null
            , ResolverPozycjaBudzetu pozycjaBudzetu = null
            , Date? data = null
            , Amount? ilosc = null
            , bool wgKwotyDodatkowej = false
            , string opis = Defaults.OpisAnalitycznyOpis)
            where T1 : DokEwidencji
        {
            row.InTransUIRes(sess => sess.AddRow(new ElementOpisuEwidencji(row.Row)))
                .Box()
                .ConditionallyObj(wymiar, (λ, v) => λ.SetWymiar(v))
                .ConditionallyObj(symbol, (λ, v) => λ.SetSymbol(v))
                .ConditionallyVal(data, (λ, v) => λ.SetData(v))
                .ConditionallyVal(kwota, (λ, v) => λ.SetKwota(v))
                .ConditionallyVal(kwotaDodatkowa, (λ, v) => λ.SetKwotaDodatkowa(v))
                .ConditionallyVal(ilosc, (λ, v) => λ.SetIlosc(v))
                .ConditionallyObj(opis, (λ, v) => λ.SetOpis(v))
                .ConditionallyObj(centrumKosztow, (λ, v) => λ.SetCentrumKosztow(v))
                .ConditionallyObj(pozycjaBudzetu, (λ, v) => λ.SetPozycjaBudzetu(v))
                .Conditionally(wgKwotyDodatkowej, λ => λ.SetWgKwotyDodatkowej(wgKwotyDodatkowej))
                .Out(out newRow);

            return row;
        }

        public static ProxyRecord<T1, DokEwidencji> NewDekret<T1>(this ProxyRecord<T1, DokEwidencji> row, ResolverOkres okres = null, Action<ProxyRecord<DekretBase>> fn = null)
            where T1 : DokEwidencji
            => row.NewDekret(out _, okres, fn);

        public static ProxyRecord<T1, DokEwidencji> NewDekret<T1>(this ProxyRecord<T1, DokEwidencji> row, out ProxyRecord<DekretBase> newRow, ResolverOkres okres = null, Action<ProxyRecord<DekretBase>> fn = null)
            where T1 : DokEwidencji
        {
            row
                .InTransUIRes(sess => sess.AddRow(new Dekret((okres ?? SelectorOkres.Standardowy).Resolve(row), row.Row)))
                .Box()
                .Out(out newRow);

            fn?.Invoke(newRow);
            return row;
        }

        public static ProxyRecord<T1, DokEwidencji> ExecutePredekretuj<T1>(this ProxyRecord<T1, DokEwidencji> row, Context context, out ManagerKsiegowan.Rezultat rezultat, ResolverOkres okres = null)
            where T1 : DokEwidencji
        {
            context.Set((okres ?? SelectorOkres.Standardowy).Resolve(row));

            rezultat = new ManagerKsiegowan(context) { Ewidencja = row.Row }.Predekretuj();
            return row;
        }

        public static ProxyRecord<T1, DokEwidencji> ExecutePredekretuj<T1>(this ProxyRecord<T1, DokEwidencji> row, Context context, ResolverOkres okres = null)
            where T1 : DokEwidencji
            => row.ExecutePredekretuj(context, out _, okres);

        public static ProxyRecord<T1, DokEwidencji> SetOddzial<T1>(this ProxyRecord<T1, DokEwidencji> row, OddzialFirmy oddzial)
            where T1 : DokEwidencji
            => row.InTransUI(λ => λ.Row.Oddzial = λ.Row.Session.Get(oddzial));

        #endregion

        #region ElemOpisuAnalitycznego

        public static ProxyRecord<ElemOpisuAnalitycznego> SetWymiar(this ProxyRecord<ElemOpisuAnalitycznego> row, string wymiar = Defaults.OpisAnalitycznyWymiar)
            => row.InTransUI(λ => λ.Row.Wymiar = wymiar);

        public static ProxyRecord<ElemOpisuAnalitycznego> SetSymbol(this ProxyRecord<ElemOpisuAnalitycznego> row, string symbol = Defaults.OpisAnalitycznySymbol)
            => row.InTransUI(λ => λ.Row.Symbol = symbol);

        public static ProxyRecord<ElemOpisuAnalitycznego> SetData(this ProxyRecord<ElemOpisuAnalitycznego> row, Date data)
            => row.InTransUI(λ => λ.Row.Data = data);

        public static ProxyRecord<ElemOpisuAnalitycznego> SetKwota(this ProxyRecord<ElemOpisuAnalitycznego> row, Currency kwota)
            => row.InTransUI(λ => λ.Row.Kwota = kwota);

        public static ProxyRecord<ElemOpisuAnalitycznego> SetKwotaDodatkowa(this ProxyRecord<ElemOpisuAnalitycznego> row, Currency kwota)
            => row.InTransUI(λ => λ.Row.KwotaDodatkowa = kwota);

        public static ProxyRecord<ElemOpisuAnalitycznego> SetWgKwotyDodatkowej(this ProxyRecord<ElemOpisuAnalitycznego> row, bool wgKwotyDodatkowej)
            => row.InTransUI(λ => λ.Row.BudzetProjWgKwotyDodatkowej = wgKwotyDodatkowej);

        public static ProxyRecord<ElemOpisuAnalitycznego> SetOpis(this ProxyRecord<ElemOpisuAnalitycznego> row, string opis)
            => row.InTransUI(λ => λ.Row.Opis = opis);

        public static ProxyRecord<ElemOpisuAnalitycznego> SetIlosc(this ProxyRecord<ElemOpisuAnalitycznego> row, Amount ilosc)
            => row.InTransUI(λ => λ.Row.Ilosc = ilosc);

        public static ProxyRecord<ElemOpisuAnalitycznego> SetCentrumKosztow(this ProxyRecord<ElemOpisuAnalitycznego> row, ResolverCentrumKosztow centrum)
            => row.InTransUI(λ => λ.Row.CentrumKosztow = centrum?.Resolve(λ));

        public static ProxyRecord<ElemOpisuAnalitycznego> SetPozycjaBudzetu(this ProxyRecord<ElemOpisuAnalitycznego> row, ResolverPozycjaBudzetu pozycjaBudzetu)
            => row.InTransUI(λ => λ.Row.PozycjaBudzProj = pozycjaBudzetu?.Resolve(λ));

        #endregion

        #region ElemEwidencjiVAT

        public static ProxyRecord<T1, ElemEwidencjiVAT> SetNetto<T1>(this ProxyRecord<T1, ElemEwidencjiVAT> row, Currency netto)
            where T1 : ElemEwidencjiVAT
            => row.InTransUI(λ => λ.Row.Netto = netto);

        public static ProxyRecord<T1, ElemEwidencjiVAT> SetBrutto<T1>(this ProxyRecord<T1, ElemEwidencjiVAT> row, Currency brutto)
            where T1 : ElemEwidencjiVAT
            => row.InTransUI(λ => λ.Row.Brutto = brutto);

        public static ProxyRecord<T1, ElemEwidencjiVAT> SetStawkaVAT<T1>(this ProxyRecord<T1, ElemEwidencjiVAT> row, ResolverStawkaVAT stawkaVAT)
            where T1 : ElemEwidencjiVAT
            => row.InTransUI(λ => λ.Row.DefinicjaStawki = stawkaVAT.Resolve(row));

        public static ProxyRecord<T1, ElemEwidencjiVAT> SetGrupa<T1>(this ProxyRecord<T1, ElemEwidencjiVAT> row, GrupaElementuVAT grupa)
            where T1 : ElemEwidencjiVAT
            => row.InTransUI(λ => λ.Row.Grupa = grupa);

        public static ProxyRecord<T1, ElemEwidencjiVAT> SetRodzajZakupu<T1>(this ProxyRecord<T1, ElemEwidencjiVAT> row, RodzajZakupuVAT rodzaj)
            where T1 : ElemEwidencjiVAT
            => row.InTransUI(λ => λ.Row.Rodzaj = rodzaj);

        public static ProxyRecord<ElemEwidencjiVATZakup, ElemEwidencjiVAT> SetDzialalnosc(this ProxyRecord<ElemEwidencjiVATZakup, ElemEwidencjiVAT> row, DzialalnoscGospodarcza dzialalnosc)
            => row.InTransUI(λ => λ.Row.DzialalnoscGosp = dzialalnosc);

        public static ProxyRecord<ElemEwidencjiVATZakup, ElemEwidencjiVAT> SetOdliczenia(this ProxyRecord<ElemEwidencjiVATZakup, ElemEwidencjiVAT> row, OdliczeniaVAT odliczenia)
            => row.InTransUI(λ => λ.Row.Odliczenia = odliczenia);

        #endregion

    }
}
