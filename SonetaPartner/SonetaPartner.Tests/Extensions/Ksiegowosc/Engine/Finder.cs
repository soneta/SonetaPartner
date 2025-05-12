using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using JetBrains.Annotations;

using Soneta.Business;
using Soneta.Business.Db;
using Soneta.Core;
using Soneta.CRM;
using Soneta.Delegacje;
using Soneta.Deklaracje;
using Soneta.Deklaracje.VAT;
using Soneta.EwidencjaVat;
using Soneta.Handel;
using Soneta.Kadry;
using Soneta.Kasa;
using Soneta.Ksiega;
using Soneta.Ksiega.Podzielniki;
using Soneta.Magazyny;
using Soneta.Place;
using Soneta.SrodkiTrwale;
using Soneta.Towary;
using Soneta.Types;
using Soneta.Samochodowka;
using Soneta.Waluty;
using Soneta.Windykacja;
using Soneta.Zadania;
using Soneta.Zadania.Budzetowanie;
using static Soneta.Deklaracje.Deklaracja;

using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;
using static Soneta.SrodkiTrwale.DokumentST;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Settings;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Engine
{
    public class Finder
    {
        private readonly Session _session;

        public Finder(Session session)
            => _session = session;

        #region modules

        private BusinessModule BusinessModule()
            => _session.Get<BusinessModule>();

        private CoreModule CoreModule()
            => _session.Get<CoreModule>();

        private CRMModule CRMModule()
            => _session.Get<CRMModule>();

        private DelegacjeModule DelegacjeModule()
            => _session.Get<DelegacjeModule>();

        private DeklaracjeModule DeklaracjeModule()
            => _session.Get<DeklaracjeModule>();

        private EwidencjaVatModule EwidencjaVatModule()
            => _session.Get<EwidencjaVatModule>();

        private HandelModule HandelModule()
            => _session.Get<HandelModule>();

        private KadryModule KadryModule()
            => _session.Get<KadryModule>();

        private KasaModule KasaModule()
            => _session.Get<KasaModule>();

        private KsiegaModule KsiegaModule()
            => _session.Get<KsiegaModule>();

        private PlaceModule PlaceModule()
            => _session.Get<PlaceModule>();

        private SrodkiTrwaleModule SrodkiTrwaleModule()
            => _session.Get<SrodkiTrwaleModule>();

        private TowaryModule TowaryModule()
            => _session.Get<TowaryModule>();

        private WalutyModule WalutyModule()
            => _session.Get<WalutyModule>();

        private WindykacjaModule WindykacjaModule()
            => _session.Get<WindykacjaModule>();

        private ZadaniaModule ZadaniaModule()
            => _session.Get<ZadaniaModule>();

        private SamochodowkaModule SamochodowkaModule()
            => _session.Get<SamochodowkaModule>();

        #endregion

        #region standard rows

        public ZUSCentrala StdZUS()
            => CRMModule().ZUSY.ZUSCentrala;

        public UrzadSkarbowyCentrala StdKAS()
            => CRMModule().ZUSY.USCentrala;

        public RachunekBankowyFirmy StdRachunekBankowyFirmy()
            => (RachunekBankowyFirmy) EwidencjaSP(Soneta.Kasa.EwidencjaSP.RachunekBankowy);

        public UrzadCelny StdUrzadCelny()
            => CRMModule().UrzedyCelne.WgKodu["KRAKÓW"];

        public OkresObrachunkowy StdOkresObrachunkowy()
            => OkresObrachunkowy(Defaults.Okres);

        public CentrumKosztow StdCentrumKosztow()
            => CoreModule().CentraKosztow.Firma;

        public OddzialFirmy StdCentrala()
            => CoreModule().OddzialyFirmy.Centrala;

        public DefinicjaStawkiVat StdStawkaVATBrak()
            => CoreModule().DefStawekVat.Brak;

        public DefinicjaStawkiVat StdStawkaVATNiePodlega()
            => CoreModule().DefStawekVat.NiePodlega;

        public DefinicjaStawkiVat StdStawkaVATPodstawowa()
            => CoreModule().DefStawekVat.Podstawowa_23;

        public DefinicjaStawkiVat StdStawkaVATZero()
            => CoreModule().DefStawekVat.Zero;

        public DefinicjaStawkiVat StdStawkaVATZwolniona()
            => CoreModule().DefStawekVat.Zwolniona;

        public DefinicjaStawkiVat StdStawkaVAT8Procent()
            => CoreModule().DefStawekVat[DefinicjaStawkiVat.Op8];

        public DefinicjaStawkiVat StdStawkaVAT5Procent()
            => CoreModule().DefStawekVat[DefinicjaStawkiVat.Op5];

        public KrajTbl StdKrajPoland()
            => CoreModule().KrajeTbl.Poland;

        public Kontrahent StdKontrahentIncydentalny()
            => CRMModule().Kontrahenci.Incydentalny;

        public Waluta StdWalutaEUR()
            => WalutyModule().Waluty.EUR;

        public Waluta StdWalutaPLN()
            => WalutyModule().Waluty.PLN;

        #endregion

        #region rows

        public FeatureDefinition Cecha([NotNull] string nazwa)
        {
            if (string.IsNullOrEmpty(nazwa))
                throw new ArgumentException(nameof(nazwa));

            return BusinessModule()
                .FeatureDefs
                .Cast<FeatureDefinition>()
                .Where(f => f.Name == nazwa)
                .InCollection();
        }

        public CentrumKosztow CentrumKosztow([NotNull] string nazwa)
        {
            if (string.IsNullOrEmpty(nazwa))
                throw new ArgumentException(nameof(nazwa));

            return CoreModule()
                .CentraKosztow
                .WgNazwy[nazwa]
                .ReturnChecked(() => $"Finder/CentrumKosztow/{nazwa}");
        }

        public DefinicjaDokumentu DefinicjaDokumentu([NotNull] string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException(nameof(symbol));

            return CoreModule()
                .DefDokumentow
                .WgSymbolu[symbol]
                .ReturnChecked(() => $"Finder/DefinicjaDokumentu/{symbol}");
        }

        public DefDokHandlowego DefinicjaHandlowa(string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException(nameof(symbol));

            return HandelModule()
                .DefDokHandlowych
                .WgSymbolu[symbol]
                .ReturnChecked(() => $"Finder/DefinicjaHandlowa/{symbol}");
        }

        public DefXmlNag DefinicjaNagXml(SelektorDefXml selector, Func<DefXmlNag, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentException(nameof(predicate));

            return CoreModule()
                .DefsXmlNag
                .WgSelektor[selector]
                .Where(predicate)
                .FirstOrDefault()
                .ReturnChecked(() => $"Finder/DefinicjaNagXml/{selector}");
        }

        public DefinicjaPowstaniaObowiazkuVAT DefinicjaPowstaniaObowiazku(Guid guid)
        {
            if (guid == Guid.Empty)
                throw new ArgumentException(nameof(guid));

            return EwidencjaVatModule()
                .DefinicjePOVAT[guid]
                .ReturnChecked(() => $"Finder/DefinicjaPowstaniaObowiazku/{guid}");
        }

        public DefinicjaSlownika DefinicjaSlownika(Guid guid)
            => KsiegaModule()
                .DefSlownikow[guid]
                .ReturnChecked(() => $"Finder/DefinicjaSlownika/{guid}");
      
        public DefinicjaStawkiVat DefinicjaStawkiVATWgProcent(decimal procent, ResolverKraj kraj = null)
          => CoreModule()
              .DefStawekVat
              .WgStawkaKraj
              .FirstOrDefault(df => df.Stawka.Status == StatusStawkiVat.Opodatkowana &&
                df.Stawka.Kraj == (kraj?.Resolve(df) ?? StdKrajPoland()) &&
                df.Stawka.Procent == procent)
              .ReturnChecked(() => $"Finder/DefinicjaStawkiVATWgProcent/{procent}");

        public EwidencjaSP EwidencjaSP(Guid guid)
            => KasaModule()
                .EwidencjeSP[guid]
                .ReturnChecked(() => $"Finder/EwidencjaSP/{guid}");

        public EwidencjaSP EwidencjaSP(string symbol)
            => KasaModule()
                .EwidencjeSP
                .WgSymbolu[symbol]
                .GetNext()
                .ReturnChecked(() => $"Finder/EwidencjaSP/{symbol}");

        public FormaPlatnosci FormaPlatnosci(Guid guid)
            => KasaModule()
                .FormyPlatnosci[guid]
                .ReturnChecked(() => $"Finder/FormaPlatnosci/{guid}");

        public FormaPrawna FormaPrawna(string kod)
            => CRMModule()
                .FormyPrawne.WgKodu[kod]
                .ReturnChecked(() => $"Finder/FormaPrawna/{kod}");

        public FormaPrawna FormaPrawna(Guid guid)
            => CRMModule()
                .FormyPrawne[guid]
                .ReturnChecked(() => $"Finder/FormaPrawna/{guid}");

        public KontoBase Konto(string symbol, OddzialFirmy firma = null)
            => KsiegaModule()
                .Konta
                .WgOkres[StdOkresObrachunkowy(), firma, symbol]
                .ReturnChecked(() => $"Finder/Konto/{symbol}/{firma}");

        public KontoBase Konto(OkresObrachunkowy okres, string symbol, OddzialFirmy firma = null)
            => KsiegaModule()
                .Konta
                .WgOkres[okres, firma, symbol]
                .ReturnChecked(() => $"Finder/Konto/{okres}/{symbol}/{firma}");

        public Kontrahent Kontrahent([NotNull] string kod)
        {
            if (string.IsNullOrEmpty(kod))
                throw new ArgumentException(nameof(kod));

            return CRMModule()
                .Kontrahenci
                .WgKodu[kod]
                .ReturnChecked(() => $"Finder/Kontrahent/{kod}");
        }

        public KrajTbl KrajWgKod2(string kod2)
        {
            if (string.IsNullOrEmpty(kod2))
                throw new ArgumentException(nameof(kod2));

            return CoreModule()
                .KrajeTbl
                .WgKodu2[kod2]
                .ReturnChecked(() => $"Finder/KrajWgKod2/{kod2}");
        }

        public KrajTbl KrajWgKod3(string kod3)
        {
            if (string.IsNullOrEmpty(kod3))
                throw new ArgumentException(nameof(kod3));

            return CoreModule()
                .KrajeTbl
                .WgKodu3[kod3]
                .ReturnChecked(() => $"Finder/KrajWgKod3/{kod3}");
        }

        public OkresObrachunkowy OkresObrachunkowy(int year, bool wCheck = true)
            => OkresObrachunkowy(new Date(year, 12, 31), true, wCheck);

        private OkresObrachunkowy OkresObrachunkowy(Date data, bool fYear, bool wCheck)
        {
            var okres = KsiegaModule().OkresyObrach[data];

            if (okres != null && !okres.Okres.Contains(data))
                okres = null;
            if (okres == null && !wCheck)
                return null;

            if (fYear)
                return okres
                    .ReturnChecked(() => $"Finder/OkresObrachunkowy/{data.Year}")
                    .ReturnCondition(obrachunkowy => obrachunkowy.Okres.Contains(data), obrachunkowy => $"Finder/OkresObrachunkowy/{data.Year}/{obrachunkowy.Okres}");

            return okres
                .ReturnChecked(() => $"Finder/OkresObrachunkowy/{data}");
        }

        public PozycjaBudzProj PozycjaBudzetu(string nazwaProjektu, string symbolPozycji)
        {
            if (string.IsNullOrEmpty(nazwaProjektu))
                throw new ArgumentException(nameof(nazwaProjektu));
            if (string.IsNullOrEmpty(symbolPozycji))
                throw new ArgumentException(nameof(symbolPozycji));

            var budzet = Projekt(nazwaProjektu).BudzetPodstawowy;

            return ZadaniaModule()
                .PozycjeBudzProj
                .wgSymbol[symbolPozycji, budzet]
                .InCollection()
                .ReturnChecked(() => $"Finder/PozycjaBudzetu/{nazwaProjektu}/{symbolPozycji}");
        }

        public Pracownik Pracownik(string kod)
        {
            if (string.IsNullOrEmpty(kod))
                throw new ArgumentException(nameof(kod));

            return KadryModule()
                .Pracownicy
                .WgKodu[kod]
                .ReturnChecked(() => $"Finder/Pracownik/{kod}");
        }

        public ProceduraVAT ProceduraVAT(TypProceduryVAT typ, string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException(nameof(symbol));

            return CoreModule()
                .ProceduryVAT
                .WgTypu[typ, symbol]
                .ReturnChecked(() => $"Finder/ProceduraVAT/{typ}/{symbol}");
        }

        public Projekt Projekt(string nazwa)
        {
            if (string.IsNullOrEmpty(nazwa))
                throw new ArgumentException(nameof(nazwa));

            return ZadaniaModule()
                .Projekty
                .WgNazwy[nazwa]
                .InCollection()
                .ReturnChecked(() => $"Finder/Projekt/{nazwa}");
        }

        public RodzajST RodzajST(TypSrodkaTrwalego typST, string krst)
        {
            if (string.IsNullOrEmpty(krst))
                throw new ArgumentException(nameof(krst));

            switch (typST)
            {
                case TypSrodkaTrwalego.Wyposażenie:
                case TypSrodkaTrwalego.ŚrodekTrwały:
                    return SrodkiTrwaleModule()
                        .KRST
                        .WgTypPodstawa[TypSrodkaTrwalego.ŚrodekTrwały, PodstawaKST.Rozporzadzenie2016, krst]
                        .ReturnChecked(() => $"Finder/RodzajST/{typST}/{krst}");
                case TypSrodkaTrwalego.WartośćNiematerialnaIPrawna:
                    return SrodkiTrwaleModule()
                        .KRST
                        .WgTypPodstawa[TypSrodkaTrwalego.WartośćNiematerialnaIPrawna, PodstawaKST.Brak, krst]
                        .ReturnChecked(() => $"Finder/RodzajST/{typST}/{krst}");
            }

            throw TestException.MakeEnumOutOfRange(typST, "typ środka trwałego");
        }      

        public SposobZaplaty SposobZaplaty(Guid guid)
            => KasaModule()
                .SposobyZaplaty[guid]
                .ReturnChecked(() => $"Finder/SposobZaplaty/{guid}");

        public SposobZaplaty SposobZaplaty([NotNull] string nazwa)
        {
            if (string.IsNullOrEmpty(nazwa))
                throw new ArgumentException(nameof(nazwa));

            return KasaModule()
                .SposobyZaplaty
                .WgNazwy[nazwa]
                .ReturnChecked(() => $"Finder/SposobZaplaty/{nazwa}");
        }

        public Towar Towar(string kod)
        {
            if (string.IsNullOrEmpty(kod))
                throw new ArgumentException(nameof(kod));

            return TowaryModule()
                .Towary
                .WgKodu[kod]
                .ReturnChecked(() => $"Finder/Towar/{kod}");
        }

        public Waluta Waluta(string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException(nameof(symbol));

            return WalutyModule()
                .Waluty
                .WgSymbolu[symbol]
                .ReturnChecked(() => $"Finder/Waluta/{symbol}");
        }

        public ZestawienieKS ZestawienieKsiegowe(TypDefinicjiZestawienia typDefinicji)
            => KsiegaModule()
                .ZestawieniaKS.ZnajdzZestawienieWgTypuDefinicji(typDefinicji, null)
                .ReturnChecked(() => $"Finder/ZestawienieKsiegowe/{typDefinicji}");

        public RodzajPO RodzajPO(string nazwa)
        {
            if (string.IsNullOrEmpty(nazwa))
                throw new ArgumentException(nameof(nazwa));

            return SrodkiTrwaleModule()
                .RodzajePO
                .WgNazwaPrzedmiotuOpodatkowania[nazwa]
                .GetNext()
                .ReturnChecked(() => $"Finder/RodzajPO/{nazwa}");
        }

        #endregion

        #region collections

        public IEnumerable<DokEwidencji> DokumentyEwidencji(Func<DokEwidencji, bool> selector = null)
            => CoreModule()
                .DokEwidencja
                .Cast<DokEwidencji>()
                .Where(λ => selector == null || selector(λ));


        public IEnumerable<DokEwidencji> DokumentyEwidencji(TypDokumentu typ, Func<DokEwidencji, bool> selector = null)
            => CoreModule()
                .DokEwidencja
                .WgTyp[typ]
                .Where(λ => selector == null || selector(λ));

        public IEnumerable<KontoBase> KontaWgLike(string like)
            => KsiegaModule()
                .Konta
                .WgOkres[StdOkresObrachunkowy()][new FieldCondition.Like(nameof(KontoBase.Symbol), like)];

        public IEnumerable<KontoBase> KontaWgLike(OkresObrachunkowy okres, string like)
            => KsiegaModule()
                .Konta
                .WgOkres[okres][new FieldCondition.Like(nameof(KontoBase.Symbol), like)];

        public IEnumerable<Platnosc> Platnosci()
            => KasaModule()
                .Platnosci
                .Cast<Platnosc>();

        public IEnumerable<PrzelewBase> Przelewy()
            => KasaModule()
                .Przelewy
                .Cast<PrzelewBase>();

        public IEnumerable<RozrachunekIdx> Rozrachunki()
            => KasaModule()
                .RozrachunkiIdx
                .Cast<RozrachunekIdx>();

        public IEnumerable<SrodekTrwalyBase> SrodkiTrwale()
            => SrodkiTrwaleModule()
                .SrodkiTrwale
                .Cast<SrodekTrwalyBase>();

        public IEnumerable<SchematKsiegowy> SchematyKsiegowe(OkresObrachunkowy okres = null, TypDokumentu? typDokumentu = null)
        {
            if (okres == null)
                okres = StdOkresObrachunkowy();

            var schematy = KsiegaModule().SchematyKsiegowe.WgOkres;
            return typDokumentu != null ? schematy[okres, typDokumentu.Value] : schematy[okres];
        }

        public DefinicjaCeny DefinicjaCeny(string nazwa)
        {
            if (string.IsNullOrEmpty(nazwa))
                throw new ArgumentException(nameof(nazwa));

            return TowaryModule()
                .DefinicjeCen
                .WgNazwy[nazwa]
                .ReturnChecked(() => $"Finder/DefinicjaCeny/{nazwa}");
        }

        #endregion

    }
}
