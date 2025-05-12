using JetBrains.Annotations;
using System;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Settings
{
	public static class Defaults
	{
		//
		// okres obrachunkowy bazy
		//
		public const int Okres = Soneta.Test.TimeDefaults.Okres;
		public const int KolejnyOkres = Okres + 1;

		//
		// domyślna stawka VAT
		//
		public const decimal StawkaVAT = 0.23m;
		public const decimal StawkaVATNiemcy = 0.28m;

		//
		// podatnik VAT
		//
		public const string DanePodatnikaVAT = "Jerzy Nowak";
		public const string CodeQuestionMark = "?";

		//
		// numery rachunkow
		//
		public const string NumerRachunkuWirtualnego = "135724680";
		public const string NumerRachunkuKontrahenta = "PL52101010100010522231000000";
		public const string NumerRachunkuZUS = "PL66114016291090831692069121";
		public const string NumerRachunkuKAS = "PL56109018104587123690000008";
		public const string NumerRachunkuUS = "PL98114000009878816141533170";
		public const string NumerRachunkuOdbiorcy = "PL31160010131846146940000001";

		//
		// dane firmy
		//
		public const string StdCompanyNIP = "111-11-11-111";
		public const string StdCompanyNazwa = "enova365 Wersja Demonstracyjna";
		public const string StdCompanyRegon = "123456785";
		public const string StdCompanyMiejscowosc = "Kraków";
		public const string StdCompanyUlica = "Wadowicka";
		public const string StdCompanyNrDomu = "8A";
		public const string StdCompanyNrLokalu = "";
		public const string StdCompanyKod = "30-415";

		//
		// dane ewidencyjne
		//
		public const string DaneEwidencyjneNazwisko = "Kowalski";
		public const string DaneEwidencyjneImie = "Jan";
		public const string DaneEwidencyjnePesel = "80010148673";
		public static readonly DateTime DaneEwidencyjneUrodzony = new DateTime(1980, 1, 1);

		//
		// dane jednostki nadrzednej
		//
		public const string JednostkaNadrzednaNazwa = "ABC Sp. z o.o, 123456785";
		public const string JednostkaNadrzednaNIP = "111-11-11-128";
		public const string JednostkaNadrzednaMiejscowosc = "Kraków";
		public const string JednostkaNadrzednaUlica = "Nowa";
		public const string JednostkaNadrzednaNrDomu = "1";
		public const string JednostkaNadrzednaNrLokalu = "";
		public const string JednostkaNadrzednaKod = "30-140";

		//
		// kody urzedu skarbowego
		//
		public const string KodUrzeduSkarbowego = "0208";
		public const string KodUrzeduSkarbowego01 = "1210";
		public const string KodUrzeduSkarbowego02 = "1213";
		public const string KodUrzeduSkarbowego03 = "1209";

		//
		// dokument ewidencji
		//
		public const string EwidencjaNazwaDokumentuRMK = "NAZWA";
		public const string EwidencjaNumerDokumentuBO = "BO";
		public const string EwidencjaNumerDokumentu = "NUMER";
		public const string EwidencjaNumerDokumentuPK = "PK";
		public const string EwidencjaNumerDokumentuWB = "WB";
		public const string EwidencjaNumerDokumentuZakup = "ZAKUP";
		public const string EwidencjaNumerDokumentuSprzedaz = "SPRZEDAŻ";
		public const string EwidencjaNumerNabyciaNaliczony = "NABYCIE NALICZONY";
		public const string EwidencjaNumerNabyciaNalezny = "NABYCIE NALEŻNY";
		public const string EwidencjaNumerDokumentuSad = "SAD";
		public const string EwidencjaNumerFakturaImportowa = "FI";
		public const string EwidencjaOpisDokumentu = "OPIS";
		public const string EwidencjaOpisDokumentuBO = "BO OPIS";
		public const string EwidencjaOpisDokumentuPK = "PK OPIS";
		public const string EwidencjaOpisDokumentuWB = "WB OPIS";
		public const string EwidencjaOpisDokumentuZakup = "ZAKUP OPIS";
		public const string EwidencjaOpisDokumentuSprzedaz = "SPRZEDAŻ OPIS";
		public const string EwidencjaOpisNabyciaNaliczony = "NABYCIE NALICZONY OPIS";
		public const string EwidencjaOpisNabyciaNalezny = "NABYCIE NALEŻNY OPIS";
		public const string EwidencjaOpisDokumentuSad = "SAD OPIS";
		public const string EwidencjaOpisDokumentuRMK = "RMK Opis";
		public const string EwidencjaOpisFakturaImportowa = "FI Opis";
		public const string EwidencjaNumerDodatkowy = "NR DODATKOWY";
		public const string EwidencjaVATMarzaNumer = "VAT MARŻA";

		public const string OpisAnalitycznyWymiar = "WYM";
		public const string OpisAnalitycznySymbol = "SYMB";
		public const string OpisAnalitycznyOpis = "ANALITYCZNY OPIS";

		//
		// wpłaty/wypłaty
		// ewidencja SP
		// operacja bankowa
		//
		public const string WplataOpis = "WPŁATA";
		public const string WplataKasaOpis = "WPŁATA KP";
		public const string WyplataOpis = "WYPŁATA";
		public const string WyplataKasaOpis = "WYPŁATA KW";

		public const string SymbolNazwaKasa = "KASA 01";
		public const string SymbolNazwaKarta = "KARTA 01";
		public const string SymbolNazwaRachunek = "RACHUNEK 01";
		public const string SymbolRachunekEuro = "BANKEURO";

		public const string OperacjaBankowaOpis = "OPIS OPERACJA BANKOWA";

		//
		// przelew (MPP)
		//
		public const string PrzelewTytulem1 = "TYTUŁEM 1";
		public const string PrzelewTytulem2 = "TYTUŁEM 2";
		public const string PrzelewOpis = "OPIS PRZELEWU";
		public const string PrzelewMPPInvoice = "FA/01/2018";
		public const string PrzelewMPPNumerNIP = "3759645187";

		//
		// środki trwałe
		//
		public const string SrodekNazwaSrodek = "ŚRODEK TRWAŁY";
		public const string SrodekNazwaNiematerialna = "WARTOŚĆ NIEMATERIALNA";
		public const string SrodekNazwaWyposazenie = "WYPOSAŻENIE";
		public const string SrodekOpisSrodek = "ŚRODEK TRWAŁY - OPIS";
		public const string SrodekOpisNiematerialna = "WARTOŚĆ NIEMATERIALNA - OPIS";
		public const string SrodekOpisWyposazenie = "WYPOSAŻENIE - OPIS";

		public const string WyposazenieNazwa = "WYPOSAŻENIE";
		public const string WyposazenieOpis = "WYPOSAŻENIE - OPIS";

		public const string SrodekKlasyfikacja = "0";

		public const string RodzajSrodkaOpisSrodek = "RODZAJ ST - OPIS";
		public const string RodzajSrodkaOpisNiematerialna = "RODZAJ NIEMATERIALNA - OPIS";

		public const string MiejsceUzytkowaniaNazwa = "MIEJSCE UŻYTKOWANIA";
		public const string LokalizacjaNieruchomosci = "LOKALIZACJA";

		//
		// deklaracje podpisy
		//
		public const string DeklaracjaOsobaImie = "Jan";
		public const string DeklaracjaOsobaNazwisko = "Kowalski";
		public const string DeklaracjaOsobaTelefon = "700-100200";
		public const string DeklaracjaOsobaEmail = "jan.kowalski@enovatesty.pl";

		//
		// KEDU
		//
		public const string KEDUNazwa = "KEDU";

		//
		// Kadry
		//
		public const string UmowaTytuł = "TYTUŁ UMOWY";

		//
		// Złe długi
		public const string ZleDlugiNumer = "NUMER";
		public const string ZleDlugiOpis = "OPIS";
	}
}
