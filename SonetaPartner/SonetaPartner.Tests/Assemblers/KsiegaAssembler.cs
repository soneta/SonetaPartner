using System;

using JetBrains.Annotations;
using Soneta.Business;
using Soneta.Core;
using Soneta.EwidencjaVat;
using Soneta.Kasa;
using Soneta.Ksiega;
using Soneta.RealEstate.Models;
using Soneta.RealEstate.Models.Database;
using Soneta.RMK;
using Soneta.SrodkiTrwale;
using Soneta.Types;
using Soneta.Zadania;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Settings;
using static Soneta.EwidencjaVat.ElemEwidencjiVATAkcyzy;

namespace SonetaPartner.Tests.Assemblers
{
    public static class AssemblerKsiega
    {
        #region ZestawienieKsiegowe

        public static ProxyRecord<DekretBase> NewZapis(this ProxyRecord<DekretBase> row, ResolverKonto konto, StronaKsiegowania strona, Currency kwotaOperacji, Currency? kwotaZapisu = null, ResolverElementKsiegowalny resolverEk = null)
            => row.NewZapis(out _, konto, strona, kwotaOperacji, kwotaZapisu, resolverEk);

        public static ProxyRecord<DekretBase> NewZapis(this ProxyRecord<DekretBase> row, out ProxyRecord<ZapisKsiegowy> newRow, ResolverKonto konto, StronaKsiegowania strona, Currency kwotaOperacji, Currency? kwotaZapisu = null, ResolverElementKsiegowalny resolverEk = null)
        {
            row
                .InTransUIRes(sess => sess.AddRow(new Zapis(row.Row)))
                .Box()
                .Out(out var outRow)
                .ConditionallyObj(resolverEk, (λ, resv) => λ.SetElementKsiegowalny(resv.Resolve(outRow.Row)))
                .SetKonto(konto)
                .ConditionallyVal(kwotaZapisu, (λ, v) => λ.SetKwota(strona, kwotaOperacji, v))
                .Conditionally(kwotaZapisu == null, λ => λ.SetKwota(strona, kwotaOperacji))
                .Out(out newRow);

            return row;
        }

        public static ProxyRecord<PozycjaZestKS> GetPozycjaByTyp(this ProxyRecord<ZestawienieKS> row, TypPozycjiZestawienia typ)
            => row.Row.SubPozycje.InCollection(fnSelector: λ => λ.Typ == typ).Box();

        public static ZestawienieKS.Wynik ExecuteOblicz(this ProxyRecord<ZestawienieKS> row, ResolverOkres okres = null, Action<ZestawieniaKS.Params> fnParams = null)
        {
            var okresObrachunkowy = (okres ?? SelectorOkres.Standardowy).Resolve(row);
            return row.Row.ObliczZestawienie(okresObrachunkowy, ZestawieniaKS.Params.Create(FromTo.All, true, TypObrotu.Księgowy).WithOptional(fnParams));
        }

        #endregion

        #region PozycjaZestKS

        public static ProxyRecord<PozycjaZestKS> SetWyrazenie(this ProxyRecord<PozycjaZestKS> row, string wyrazenie)
            => row.InTransUI(λ => λ.Row.Wyrazenie = wyrazenie);

        #endregion

        #region Zapis

        public static ProxyRecord<ZapisKsiegowy> SetKonto(this ProxyRecord<ZapisKsiegowy> row, [NotNull] ResolverKonto konto)
            => row.InTransUI(λ => λ.Row.Konto = konto.Resolve(row.Row.Dekret.Okres));


        public static ProxyRecord<ZapisKsiegowy> SetKwota(this ProxyRecord<ZapisKsiegowy> row, StronaKsiegowania strona, Currency kwotaOperacji)
            => row.InTransUI(λ => λ.Row.SetKwota(strona, kwotaOperacji));


        public static ProxyRecord<ZapisKsiegowy> SetKwota(this ProxyRecord<ZapisKsiegowy> row, StronaKsiegowania strona, Currency kwotaOperacji, Currency kwotaZapisu)
            => row.InTransUI(λ => λ.Row.SetKwota(strona, kwotaOperacji, kwotaZapisu));


        public static ProxyRecord<ZapisKsiegowy> SetElementKsiegowalny(this ProxyRecord<ZapisKsiegowy> row, [NotNull] IElementKsiegowalny ksiegowalny)
            => row.InTransUI(λ => λ.Row.ElementKsiegowalny = ksiegowalny);

        #endregion

        #region OkresObrachunkowy

        public static ProxyRecord<OkresObrachunkowy> SetOkres(this ProxyRecord<OkresObrachunkowy> row, FromTo okres)
            => row.InTransUI(λ => λ.Row.Okres = okres);

        public static ProxyRecord<OkresObrachunkowy> SetSymbol(this ProxyRecord<OkresObrachunkowy> row, string symbol)
            => row.InTransUI(λ => λ.Row.Symbol = symbol);

        public static ProxyRecord<OkresObrachunkowy> SetOpis(this ProxyRecord<OkresObrachunkowy> row, string opis)
            => row.InTransUI(λ => λ.Row.Opis = opis);

        #endregion

        #region Konto

        public static ProxyRecord<KontoBase> SetSegment(this ProxyRecord<KontoBase> row, string segment)
            => row.InTransUI(λ => λ.Row.Segment = segment);

        public static ProxyRecord<KontoBase> SetTyp(this ProxyRecord<KontoBase> row, TypKonta typ)
            => row.InTransUI(λ => λ.Row.Typ = typ);

        public static ProxyRecord<KontoBase> SetNazwa(this ProxyRecord<KontoBase> row, string nazwa)
            => row.InTransUI(λ => λ.Row.Nazwa = nazwa);

        public static ProxyRecord<KontoBase> NewKontoAnalityczne(this ProxyRecord<KontoBase> row, string segment, string nazwa = null)
            => NewKontoAnalityczne(row, out _, segment, nazwa);

        public static ProxyRecord<KontoBase> NewKontoAnalityczne(this ProxyRecord<KontoBase> row, out ProxyRecord<KontoBase> newRow, string segment, string nazwa = null)
        {
            row
                .InTransUIRes(sess => sess.AddRow(new KontoAnalityczne(row.Row.Okres, row.Row)))
                .Box()
                .SetSegment(segment)
                .SetNazwa(nazwa ?? segment)
                .Out(out newRow);

            return row;
        }

        public static ProxyRecord<KontoBase> NewDefinicjaAnalityki(this ProxyRecord<KontoBase> row, ResolverDefinicjaSlownika resolverDefinicjaSlownika = null, Action<ProxyRecord<DefinicjaAnalityki>> fn = null)
            => NewDefinicjaAnalityki(row, out _, resolverDefinicjaSlownika, fn);

        public static ProxyRecord<KontoBase> NewDefinicjaAnalityki(this ProxyRecord<KontoBase> row, out ProxyRecord<DefinicjaAnalityki> newRow, ResolverDefinicjaSlownika resolverDefinicjaSlownika = null, Action<ProxyRecord<DefinicjaAnalityki>> fn = null)
        {
            row
                .InTransUIRes(sess => sess.AddRow(new DefinicjaAnalityki(row.Row)))
                .Box()
                .ConditionallyObj(resolverDefinicjaSlownika, (λ, resv) => λ.SetDefinicjaSlownika(resolverDefinicjaSlownika))
                .Out(out newRow);

            fn?.Invoke(newRow);
            return row;
        }

        public static ProxyRecord<DefinicjaAnalityki> NewWyjatekDefinicjiAnalityki(this ProxyRecord<DefinicjaAnalityki> definicja, out ProxyRecord<WyjatekDefinicjiAnalityki> newRow)
        {
            definicja
                .InTransUIRes(sess => sess.AddRow(new WyjatekDefinicjiAnalityki(definicja.Row)))
                .Box()
                .Out(out newRow);

            return definicja;
        }

        #endregion

        #region SchematKsiegowy

        public static ProxyRecord<SchematKsiegowy> SetBlokada(this ProxyRecord<SchematKsiegowy> row, bool setOn = true)
            => row.InTransUI(λ => λ.Row.Blokada = setOn);

        public static ProxyRecord<SchematPK> SetNazwa(this ProxyRecord<SchematPK> row, string nazwa)
            => row.InTransUI(λ => λ.Row.Nazwa = nazwa);

        public static ProxyRecord<SchematPK> SetTrybEdycji(this ProxyRecord<SchematPK> row, TrybEdycjiSchematu tryb)
            => row.InTransUI(λ => λ.Row.TrybEdycji = tryb);

        public static ProxyRecord<SchematPK> SetWarunek(this ProxyRecord<SchematPK> row, MemoText warunek)
            => row.InTransUI(λ => λ.Row.Warunek = warunek);

        public static ProxyRecord<SchematPK> SetOpis(this ProxyRecord<SchematPK> row, MemoText opis)
            => row.InTransUI(λ => λ.Row.Opis = opis);

        public static ProxyRecord<PozycjaSchematuPK> SetTrybEdycji(this ProxyRecord<PozycjaSchematuPK> row, TrybEdycjiSchematu tryb)
            => row.InTransUI(λ => λ.Row.TrybEdycji = tryb);

        public static ProxyRecord<PozycjaSchematuPK> SetOpis(this ProxyRecord<PozycjaSchematuPK> row, MemoText opis)
            => row.InTransUI(λ => λ.Row.Opis = opis);

        public static ProxyRecord<PozycjaSchematuPK> SetStrona(this ProxyRecord<PozycjaSchematuPK> row, MemoText strona)
            => row.InTransUI(λ => λ.Row.Strona = strona);

        public static ProxyRecord<PozycjaSchematuPK> SetKonto(this ProxyRecord<PozycjaSchematuPK> row, MemoText konto)
            => row.InTransUI(λ => λ.Row.Konto = konto);

        public static ProxyRecord<PozycjaSchematuPK> SetPrzedmiotKs(this ProxyRecord<PozycjaSchematuPK> row, MemoText kontekst)
            => row.InTransUI(λ => λ.Row.Kontekst = kontekst);

        public static ProxyRecord<PozycjaSchematuPK> SetKwotaOperacji(this ProxyRecord<PozycjaSchematuPK> row, MemoText kwota)
            => row.InTransUI(λ => λ.Row.KwotaOperacji = kwota);

        #endregion

        #region DefinicjaAnalityki

        public static ProxyRecord<DefinicjaAnalityki> SetDefinicjaSlownika(this ProxyRecord<DefinicjaAnalityki> row, ResolverDefinicjaSlownika resolverDefinicjaSlownika)
            => row.InTransUI(λ => λ.Row.DefinicjaSlownika = resolverDefinicjaSlownika.Resolve(λ));

        public static ProxyRecord<DefinicjaAnalityki> SetGenerujAnalityke(this ProxyRecord<DefinicjaAnalityki> row, bool setOn = true)
            => row.InTransUI(λ => λ.Row.GenerujAnalityke = setOn);

        #endregion

        #region WyjatekDefinicjiAnalityki

        public static ProxyRecord<WyjatekDefinicjiAnalityki> SetDefinicjaSlownika(this ProxyRecord<WyjatekDefinicjiAnalityki> row, ResolverDefinicjaSlownika resolverDefinicjaSlownika)
            => row.InTransUI(λ => λ.Row.DefinicjaSlownika = resolverDefinicjaSlownika.Resolve(λ));

        public static ProxyRecord<WyjatekDefinicjiAnalityki> SetWarunekZaawansowany(this ProxyRecord<WyjatekDefinicjiAnalityki> row, string warunek)
            => row.InTransUI(λ => λ.Row.WarunekTxt = warunek);

        #endregion
    }
}
