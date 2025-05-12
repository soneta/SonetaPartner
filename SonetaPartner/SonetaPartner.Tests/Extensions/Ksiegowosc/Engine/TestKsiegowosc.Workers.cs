using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Soneta.Business;
using Soneta.Business.Compiler;
using Soneta.Business.Compiler.CodeEditor;
using Soneta.Core;
using Soneta.Deklaracje;
using Soneta.Deklaracje.JPK;
using Soneta.Deklaracje.ZAW;
using Soneta.Delegacje;
using Soneta.EwidencjaVat;
using Soneta.Handel;
using Soneta.Kadry;
using Soneta.Kasa;
using Soneta.KP;
using Soneta.Ksiega;
using Soneta.Ksiega.Forms;
using Soneta.Ksiega.JPK.Procedury;
using Soneta.Ksiega.Kasowe;
using Soneta.Ksiega.ZleDlugi;
using Soneta.Place;
using Soneta.RMK;
using Soneta.Samochodowka;
using Soneta.SrodkiTrwale;
using Soneta.Towary.PSD;
using Soneta.Types;
using Soneta.Windykacja;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors;
using static Soneta.Core.DokEwidencja.Renumeracja;
using static Soneta.Core.DokEwidencji;
using static Soneta.Deklaracje.Deklaracja;
using static Soneta.Kasa.RaportESP;
using static Soneta.Ksiega.KontoBase;
using static Soneta.Ksiega.PozycjaZestKS;
using static Soneta.Ksiega.PozycjaZestKS.WyrazenieOkresuWorker;
using static Soneta.Samochodowka.KosztEP;
using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;
using Wyplata = Soneta.Kasa.Wyplata;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Engine
{
    public partial class TestKsiegowosc
    {

        public ProxyWorker<WyciągBankowyWorker, IList<Wyplata>> WorkerWyciagBankowy(IEnumerable<PrzelewBase> przelewy, Action<WyciągBankowyWorker.Params> fnParams = null, ResolverRachunekBankowyFirmy rachunek = null)
        {
            Context.Set((rachunek ?? SelectorRachunekBankowyFirmy.FirmowyRachunekBankowy).Resolve(Session));

            return new ProxyWorker<WyciągBankowyWorker, IList<Wyplata>>(
                Session,
                new WyciągBankowyWorker
                {
                    Przelewy = Session.InSession(przelewy).ToArray(),
                    Pars = new WyciągBankowyWorker.Params(Context).WithOptional(fnParams)
                },
                λ =>
                {
                    λ.WyciągBankowy();
                    return λ.ZaplatyAsList;
                });
        }

        public ProxyWorker<NaliczaniePrzelewowCore, List<PrzelewBase>> WorkerNaliczaniePrzelewow<T1>(
            IEnumerable<T1> inDocs,
            Action<NaliczaniePrzelewowCore.Params> fnParams = null,
            ResolverRachunekBankowyFirmy rachunek = null)
            => WorkerNaliczaniePrzelewow<T1, PrzelewBase>(inDocs, fnParams, rachunek);

        public ProxyWorker<NaliczaniePrzelewowCore, List<T2>> WorkerNaliczaniePrzelewow<T1, T2>(
          IEnumerable<T1> inDocs,
          Action<NaliczaniePrzelewowCore.Params> fnParams = null,
          ResolverRachunekBankowyFirmy rachunek = null)
        {
            Context.Set((rachunek ?? SelectorRachunekBankowyFirmy.FirmowyRachunekBankowy).Resolve(Session));

            return new ProxyWorker<NaliczaniePrzelewowCore, List<T2>>(
                Session,
                new NaliczaniePrzelewówWorker
                {
                    Pars = new NaliczaniePrzelewowCore.Params(Context)
                    .With(prms =>
                    {
                        prms.DokEwidencji = Session.InSession(inDocs.ToArrayOfType<DokEwidencji>());
                        prms.Rozrachunki = Session.InSession(inDocs.ToArrayOfType<RozrachunekIdx>());
                        prms.Zaplaty = Session.InSession(inDocs.ToArrayOfType<Zaplata>());
                        prms.Zobowiązania = Session.InSession(inDocs.ToArrayOfType<Platnosc>());
                    })
                    .WithOptional(fnParams)
                },
                λ =>
                {
                    ((NaliczaniePrzelewówWorker)λ).Nalicz();

                    if (typeof(T2) == typeof(PaczkaPrzelewow))
                    {
                        return (List<T2>)(object)((NaliczaniePrzelewówWorker)λ).PaczkiAsList;
                    }
                    else if (typeof(T2) == typeof(PrzelewBase))
                    {
                        return (List<T2>)(object)((NaliczaniePrzelewówWorker)λ).PrzelewyAsList;
                    }
                    return new List<T2>();
                });
        }

        public ProxyWorker<NaliczaniePrzelewowCore, List<PrzelewBase>> WorkerNaliczaniePrzelewowEwidencji<T1>(
            IEnumerable<T1> inDocs,
            Action<NaliczaniePrzelewowCore.Params> fnParams = null,
            ResolverRachunekBankowyFirmy rachunek = null)
        {
            Context.Set((rachunek ?? SelectorRachunekBankowyFirmy.FirmowyRachunekBankowy).Resolve(Session));

            return new ProxyWorker<NaliczaniePrzelewowCore, List<PrzelewBase>>(
                Session,
                new NaliczaniePrzelewowEwidencjaWorker
                {
                    Pars = new NaliczaniePrzelewowCore.Params(Context)
                    .With(prms =>
                    {
                        prms.DokEwidencji = Session.InSession(inDocs.ToArrayOfType<DokEwidencji>());
                        prms.Rozrachunki = Session.InSession(inDocs.ToArrayOfType<RozrachunekIdx>());
                    })
                    .WithOptional(fnParams)
                },
                λ =>
                {
                    ((NaliczaniePrzelewowEwidencjaWorker)λ).Nalicz();
                    return λ.PrzelewyAsList;
                });
        }

        public ProxyWorker<BoKontaSchematyGenerator> WorkerBoKontaSchematyGenerator(Action<BoKontaSchematyParams> fnParams = null)
            => new ProxyWorker<BoKontaSchematyGenerator>(
                Session,
                new BoKontaSchematyGenerator(new BoKontaSchematyParams(Context).WithOptional(fnParams)),
                λ => λ.Generuj());
 
        public ProxyWorker<GenerujBoZapisyWorker, ManagerKsiegowan.Rezultat> WorkerGenerujBoZapisy(ResolverOkres okres = null, Action<GenerujBoZapisyParams> fnParams = null)
        {
            var okresObrachunkowy = (okres ?? SelectorOkres.Standardowy).Resolve(Session);
            Context.Set(okresObrachunkowy);

            return new ProxyWorker<GenerujBoZapisyWorker, ManagerKsiegowan.Rezultat>(
                Session,
                new GenerujBoZapisyWorker(new GenerujBoZapisyParams(Context) {Data = okresObrachunkowy.Okres.From}.WithOptional(fnParams)),
                λ => λ.GenerujZapisy());
        }

        public ProxyWorker<GenerujBoSaldaWorker, ManagerKsiegowan.Rezultat> WorkerGenerujBoSalda(ResolverOkres okres = null, Action<GenerujBoSaldaParams> fnParams = null)
        {
            var okresObrachunkowy = (okres ?? SelectorOkres.Standardowy).Resolve(Session);
            Context.Set(okresObrachunkowy);

            return new ProxyWorker<GenerujBoSaldaWorker, ManagerKsiegowan.Rezultat>(
                Session,
                new GenerujBoSaldaWorker(new GenerujBoSaldaParams(Context) {Data = okresObrachunkowy.Okres.From}.WithOptional(fnParams)),
                λ => λ.GenerujSalda());
        }
 
        public ProxyWorker<BoKontaSchematyWorker> WorkerGenerujBoKontaSchematy(ResolverOkres okres = null, Action<BoKontaSchematyParams> fnParams = null)
        {
            var okresObrachunkowy = (okres ?? SelectorOkres.Standardowy).Resolve(Session);
            Context.Set(okresObrachunkowy);

            return new ProxyWorker<BoKontaSchematyWorker>(
                Session,
                new BoKontaSchematyWorker(new BoKontaSchematyParams(Context).WithOptional(fnParams)),
                λ => λ.Generuj());
        }

        public ProxyWorker<KontoBase.KontaSlownikoweWorker> WorkerKontaSlownikowe(ResolverKonto resolverKonto, Action<KontoBase.KontaSlownikoweWorker.Params> fnParams = null)
        {
            var konto = resolverKonto.Resolve(Session);
            Context.Set(konto);

            return new ProxyWorker<KontoBase.KontaSlownikoweWorker>(
                Session,
                new KontoBase.KontaSlownikoweWorker(new KontoBase.KontaSlownikoweWorker.Params(Context).WithOptional(fnParams)) {Konto = konto},
                λ => λ.GenerujKonta());
        }

        public ProxyWorker<KopierPozycjiSchematowKsiegowych, PozycjaSchematuPK> WorkerKopiujPozycjeSchematuPK(PozycjaSchematuPK pozycja)
        {
            return new ProxyWorker<KopierPozycjiSchematowKsiegowych, PozycjaSchematuPK>(
                Session,
                new KopierPozycjiSchematowKsiegowych
                {
                    Pozycja = pozycja
                },
                λ => (PozycjaSchematuPK)λ.Kopiuj());
        }

    }

}
