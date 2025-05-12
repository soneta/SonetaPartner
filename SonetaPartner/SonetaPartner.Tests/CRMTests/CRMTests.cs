using Soneta.Test;
using NUnit.Framework;
using System.Linq;
using Soneta.Zadania;
using Soneta.CRM;
using Soneta.Handel;
using System;
using FluentAssertions;
using Soneta.Business;
using Soneta.Zadania.Podmioty_Zadania;
using Soneta.Business.Db;
using Soneta.Core;
using Soneta.Business.App;
using SonetaPartner.Tests.Extensions.CRM.Engine;
using Soneta.Magazyny;
using Soneta.Towary;
using Soneta.PracaZdalna;
using Soneta.Types;
using SonetaPartner.Tests.Assemblers;
using Soneta.Zadania.Workers.Poczta;


namespace SonetaPartner.Tests.CRMTests
{
    internal class CRMTests : TaskBase
    {

        private KontaktOsobaWizytowka wizytowka;
        private WizytowkaFirmyPrzeksztalcWorker worker;

        [SetUp]
        public void Setup()
        {

            InConfigTransaction(() =>
            {
                var invoiceDefinition = Session.GetHandel().DefDokHandlowych.WgSymbolu["FV"];
                var magazine = Session.GetMagazyny().Magazyny.WgSymbol["F"];

                var hire = AddConfig(new DefZadania()
                {
                    Rodzaj = RodzajZadania.Zadanie,
                    Nazwa = "Wypożyczenie",
                    Symbol = "W",
                    InicjujNazwe = true,
                    OpisHTML = true,
                    DefinicjaDokHandlowego = invoiceDefinition,
                    Magazyn = magazine
                }); ;
                AddFullRight(hire);

                var order = AddConfig(new DefZadania()
                {
                    Rodzaj = RodzajZadania.Zadanie,
                    Nazwa = "Zlecenie",
                    Symbol = "Z",
                    InicjujNazwe = true,
                    OpisHTML = true,
                    DefinicjaDokHandlowego = invoiceDefinition,
                    Magazyn = magazine
                });
                AddFullRight(order);
            });
            SaveDisposeConfig();

            InUITransaction(() =>
            {
                var contractor = Session.GetCRM().Kontrahenci.WgKodu["Abc"];
                Assert.IsNotNull(contractor, "Nie odnaleziono kontrahenta Abc");

                var representative = Session.GetCRM().KontaktyOsoby.WgKontrahent[contractor].FirstOrDefault();
                Assert.IsNotNull(representative, "Nie odnaleziono przedstawiciela Paszyński");

                var taskLeader = Session.GetBusiness().Operators.ByName["Administrator"];
                Assert.IsNotNull(taskLeader, "Nie odnaleziono operatora");

                var taskDefinition = Session.GetZadania().DefZadan.WgSymbolu["ZAD"];
                Assert.IsNotNull(taskDefinition, "Nie znaleziono definicji");

                var task = Add(new Zadanie()
                {
                    Definicja = taskDefinition,
                    Nazwa = "Zadanie Test",
                    Kontrahent = contractor,
                    Opis = "Test",
                    Prowadzacy = taskLeader,
                    Przedstawiciel = representative,
                    DataOd = DateTime.Today.AddDays(1),
                    DataDo = DateTime.Today.AddDays(8),
                    Koszt = 100,
                    Przychod = 200
                });

                var projectDefinition = Session.GetZadania().DefProjektow.WgSymbolu["PRO"];
                Assert.IsNotNull(projectDefinition, "Nie odnaleziono definicji PRO");

                Add(new Projekt()
                {
                    Definicja = projectDefinition,
                    Nazwa = "Projekt",
                    Koszt = 200,
                    Przychod = 300
                });
            });
            SaveDispose();

        }

        [Test]
        public void DodanieWypozyczeniaTest_Test()
        {
            var typUrzadzeniaDefinition = Session.GetZadania().TypyUrzadzen.wgSymbol["U"];
            Assert.IsNotNull(typUrzadzeniaDefinition, "Nie odnaleziono nowej definicji typu urządzenia");
            var wypozyczenieDefinition = Session.GetZadania().DefZadan.WgSymbolu["WYPOŻ"];
            var fv = new DokumentHandlowy();
            var towar = Session.GetTowary().Towary.WgKodu["MONTAZ"];
            Assert.IsNotNull(towar, "Nie odnaleziono Towaru");

            InUITransaction(() =>
            {
                var model = Add(new ModelUrz()
                {
                    Symbol = "M1",
                    Nazwa = "Model 1",
                    TypUrzadzenia = typUrzadzeniaDefinition
                });

                var urzadzenie = Add(new Urzadzenie()
                {
                    ModelUrz = model,
                    Nazwa = "Urzadzenie 1",
                    DataGwarancji = DateTime.Today.AddYears(2),
                    PrzegladOkres = 2,
                    TowarUsluga = towar,
                    Stawka = 50,
                    DataSprzedazy = DateTime.Today

                });

                var wypozyczenie = Add(new Zadanie()
                {
                    Definicja = wypozyczenieDefinition,
                    Nazwa = "Wypozyczenie",
                    Urzadzenie = urzadzenie,
                    DataOd = DateTime.Today.AddDays(-1),
                    DataDo = DateTime.Today.AddYears(2),
                    DataZamkniecia = DateTime.Today.AddYears(2),
                });
            });
            SaveDispose();

            var urzadzenieDodanie = Session.GetZadania().Urzadzenia.WgNazwy["Urzadzenie 1"].FirstOrDefault();
            var modelDodanie = Session.GetZadania().ModeleUrz.WgSymbol["M1"].FirstOrDefault();
            var wypozyczenieDodanie = Session.GetZadania().Zadania.WgNazwy["Wypozyczenie"].FirstOrDefault();
            Assert.AreEqual(modelDodanie, urzadzenieDodanie.ModelUrz, "Niepoprawny model urządzenia");
            Assert.AreEqual("Wypozyczenie", wypozyczenieDodanie.Nazwa, "Brak wypozyczenia");
        }

        [Test]
        public void DodanieNowejKampaniiTest_Test()
        {
            var defKampanii = Session.GetZadania().DefKampanii.WgSymbolu["KAM"];
            InUITransaction(() =>
            {
                Add(new Kampania()
                {
                    Nazwa = "Kampania",
                    Definicja = defKampanii,
                    Koszt = 200,
                    Przychod = 300
                });
            });
            SaveDispose();

            var defProjektu = Session.GetZadania().DefProjektow.WgSymbolu["PRO"];
            var kampania = Session.GetZadania().Kampanie.WgNazwy["Kampania"].GetFirst();

            kampania.PrzychodPlan.ToString().Should().Be("0,00 PLN");
            InUITransaction(() =>
            {
                Add(new Projekt()
                {
                    Nazwa = "Projekt",
                    Definicja = defProjektu,
                    Koszt = 100,
                    Przychod = 200
                });
            });
            SaveDispose();

            var projekt = Session.GetZadania().Projekty.WgNazwy["Projekt"].GetFirst();
            var kamp = Session.GetZadania().Kampanie.WgNazwy["Kampania"].GetFirst();

            InUITransaction(() => { projekt.Kampania = kamp; });
            SaveDispose();

            var kampZProjektem = Session.GetZadania().Kampanie.WgNazwy["Kampania"].GetFirst();
            kampZProjektem.PrzychodPlan.ToString().Should().Be("0,00 PLN");

            var defZadania = Session.GetZadania().DefZadan.WgSymbolu["ZAD"];
            defZadania.Nazwa.Should().Be("Zadanie");

            InUITransaction(() =>
            {
                var zadanie = new Zadanie()
                {
                    Definicja = defZadania,
                    Koszt = 50,
                    Przychod = 75
                };
                zadanie.Nazwa = "Zadanie";
                Add(zadanie);
            });
            SaveDispose();

            var zad = Session.GetZadania().Zadania.WgNazwy["Zadanie"].GetFirst();
            var pro = Session.GetZadania().Projekty.WgNazwy["Projekt"].GetFirst();

            InUITransaction(() => { zad.Projekt = pro; });
            SaveDispose();

            var kampzZadaniem = Session.GetZadania().Kampanie.WgNazwy["Kampania"].GetFirst(); ;
            kampzZadaniem.PrzychodPlan.ToString().Should().Be("75,00 PLN");
            kampzZadaniem.KosztPlan.ToString().Should().Be("50,00 PLN");
        }

        [Test]
        public void KontrahentConfig_Test()
        {
            InUIConfigTransaction(() =>
            {
                var prefix = CRMModule.GetInstance(ConfigSession).Config.Ogólne;
                prefix.PrefiksKoduBanku = "B";
                prefix.PrefiksKoduUrzędu = "U";
                prefix.PrefiksKoduZUS = "Z";
            });
            SaveDisposeConfig();

            InUITransaction(() =>
            {
                Add(new Bank() { Nazwa = "Bank" });
                Add(new UrzadSkarbowy() { Nazwa = "Urzad" });
                Add(new OddziałZUS() { Nazwa = "ZUS" });
            });
            SaveDispose();

            var bank = Session.GetCRM().Banki.WgNazwy["Bank"];
            var urzad = Session.GetCRM().UrzedySkarbowe.WgNazwy["Urzad"];
            var zus = Session.GetCRM().OddzialyZUS.WgNazwy["ZUS"];

            Assert.AreEqual("B00001", bank.Kod, "Nieoprawny kod banku");
            Assert.AreEqual("U00001", urzad.Kod, "Nieoprawny kod urzędu");
            Assert.AreEqual("Z00001", zus.Kod, "Nieoprawny kod ZUS");
        }

        [Test]
        public void CreateTask_AssignInvoiceToTask_And_AssignRelatedItems_Test()
        {
            InUITransaction(() =>
            {
                var task = Session.GetZadania().Zadania.WgNazwy["Zadanie Test"].FirstOrDefault();
                Assert.IsNotNull(task, "Nie znaleziono zadania");

                var hireDefinition = Session.GetZadania().DefZadan.WgSymbolu["W"];
                Assert.IsNotNull(hireDefinition, "Nie znaleziono definicji");

                var taskRelated = new Zadanie() { Definicja = hireDefinition, Nazwa = "Wypożyczenie test" };
                Add(taskRelated);
                taskRelated.Nadrzedne = task;

                var relatedSubject = Session.GetCRM().Kontrahenci.WgKodu["Aspen"];
                Assert.IsNotNull(relatedSubject, "Nie znaleziono kontrahenta");

                var addRelatedSubject = new PodmiotZadanie();
                addRelatedSubject.Kontrahent = relatedSubject;
                addRelatedSubject.Zadanie = task;
                Add(addRelatedSubject);

                var employee = Session.GetPracaHybrydowa().Kadry.Pracownicy.WgKodu["006"];
                Assert.IsNotNull(employee, "Nie znaleziono pracownika");

                var addEmployee = new ZasobCRM();
                addEmployee.Zadanie = task;
                addEmployee.Zasob = employee;
                Add(addEmployee);

                var fv = new DokumentHandlowy();
                Session.AddRow(fv);
                fv.Definicja = Session.GetHandel().DefDokHandlowych.WgSymbolu["FV"];
                fv.Kontrahent = Session.GetCRM().Kontrahenci.WgKodu["ABC"];
                fv.Magazyn = Session.GetMagazyny().Magazyny.WgSymbol["F"];
                PozycjaDokHandlowego pozycjaFV = new PozycjaDokHandlowego(fv);
                HandelModule.GetInstance(Session).PozycjeDokHan.AddRow(pozycjaFV);
                pozycjaFV.Towar = Session.GetTowary().Towary.WgNazwy["Montaż wiązań narciarskich"].GetFirst();

                var addDocument = Add(new DokumentCRM());
                addDocument.Host = task;
                addDocument.Dokument = fv;
                addDocument.RodzajDokCRM = RodzajDokCRM.DokHandlowy;

                Assert.AreEqual(task.DokumentyCRM.Count(), 1, "Błędna ilość dokumentów CRM");
                Assert.AreEqual(task.ZadaniaPowiazane.Count(), 1, "Błędna ilość zadań powiązanych");
            });
            SaveDispose();
        }
        
        [Test]
        public void UpdateTime_Should_SetDateAndTimeForNow_Test()
        {
            var task = NewTask("10.08.2023", "", "8:00", "0:00");

            task.Definicja.AktualizujCzas.Should().BeTrue();

            task.DataDo.Should().Be(Date.MaxValue);
            task.CzasDo.Should().Be(TimeSec.Zero);
            task.StanZadania.Nazwa.Should().Be("Do realizacji");
            task.Aktywny.Should().BeTrue();

            ChangeTaskState(task);

            task.StanZadania.Nazwa.Should().Be("Zrealizowane");
            task.Aktywny.Should().BeFalse();

            task.DataDo.Should().Be(Date.Today);
            task.CzasDo.Should().Be(new TimeSec(Time.Now.Hours, Time.Now.Minutes, 0));
        }

        private Zadanie NewTask(string dateFrom, string dateTo, string timeFrom, string timeTo)
            => NewRow<Zadanie>()
                .WithName("Zadanie")
                .WithDateFrom(dateFrom)
                .WithTimeFrom(timeFrom)
                .WithDateTo(dateTo)
                .WithTimeTo(timeTo)
                .Utwórz(ctx => ctx.Set(GetTaskDefinitionZAD()));

        private void ChangeTaskState(Zadanie zadanie)
        {
            InUITransaction(() =>
            {
                zadanie.StanZadania = zadanie.GetListStanZadania().Where(x => x.Nazwa is "Zrealizowane").FirstOrDefault();
            });
        }

        [Test]
        public void Duplicate_OperatorWithoutCallendar_OperatorGetsDuplicatedWithoutException_Test()
        {
            EditInConfigSession((ses, ctx) =>
            {
                var duplicateOperatorWorker =
               new DuplicateOperatorWorker { Operator = ses[Login.Operator] as Operator };

                duplicateOperatorWorker.Duplicate();
            });
        }

        [Test]
        public void NowaWiadomoscZSzablonem_NoTemplatesAvailable_WiadomoscRoboczaReturned_Test()
        {
            Context.Set(Session.GetCRM().Kontrahenci.WgKodu["Abc"]);
            Context.Set(new CurrentObject(Context) { Value = Session.GetCRM().Kontrahenci.WgKodu["Abc"] });

            var worker =
                Context.CreateObject(null, typeof(EmailElementWyslijEmailWorker), null) as EmailElementWyslijEmailWorker;
            Assert.AreEqual(typeof(WiadomoscRobocza), worker?.NowaWiadomoscZSzablonem().GetType());
        }

        [Test]
        public void KopiowanieDefinicjiZadania_Test()
        {
            InConfigTransaction(() =>
            {
                var def = ZadaniaModule.GetInstance(ConfigEditSession).DefZadan.GetFirstGranted() as DefZadania;
                var copy = def.Copy() as DefZadania;
                Assert.True(copy.Stany.All(x => def.Stany.Select(y => y.Nazwa).Contains(x.Nazwa)));
                Assert.True(copy.Priorytety.All(x => def.Priorytety.Select(y => y.Nazwa).Contains(x.Nazwa)));
                Assert.True(copy.Nazwa != def.Nazwa);
                Assert.True(copy.Symbol != def.Symbol);
            });
        }

        [Test]
        public void ModelUrzadzeniaCreation_Test()
        {
            InUIConfigTransaction(() =>
            {
                var typUrzadzenia = AddConfig(new TypUrzadzenia()
                {
                    Symbol = "TU1",
                    Nazwa = "Typ Urzadzenia 1",
                });

                AddFullRight(typUrzadzenia);
            });
            SaveDisposeConfig();

            var definition = Session.GetZadania().TypyUrzadzen.wgSymbol["TU1"];
            Assert.IsNotNull(definition, "Nie odnaleziono nowej definicji typu urządzenia");

            InUITransaction(() =>
            {
                var model = Add(new ModelUrz()
                {
                    Symbol = "M1",
                    Nazwa = "Model 1",
                    TypUrzadzenia = definition
                });
                var urzadzenie = Add(new Urzadzenie()
                {
                    ModelUrz = model,
                    Nazwa = "Urzadzenie 1"
                });
            });
            SaveDispose();

            var urzadzenieAdded = Session.GetZadania().Urzadzenia.WgNazwy["Urzadzenie 1"].FirstOrDefault();
            var modelAdded = Session.GetZadania().ModeleUrz.WgSymbol["M1"].FirstOrDefault();
            Assert.AreEqual(modelAdded, urzadzenieAdded.ModelUrz, "Niepoprawny model urządzenia");
        }
        
        [Test]
        public void CreateProject_AssignContractorFromRepresentative_And_AddRelatedItems_Test()
        {
            InTransaction(() =>
            {
                var projectDefinition = Session.GetZadania().DefProjektow.WgSymbolu["PRO"];
                Assert.IsNotNull(projectDefinition, "Nie odnaleziono definicji PRO");

                var contractor = Session.GetCRM().Kontrahenci.WgKodu["Abc"];
                Assert.IsNotNull(contractor, "Nie odnaleziono kontrahenta Abc");

                var representative = Session.GetCRM().KontaktyOsoby.WgKontrahent[contractor].FirstOrDefault();
                Assert.IsNotNull(representative, "Nie odnaleziono przedstawiciela Paszyński");

                var toImplementation = projectDefinition.Stany.ToList().Where(x => x.Nazwa.StartsWith("Do realizacji")).FirstOrDefault();
                var realized = projectDefinition.Stany.ToList().Where(x => x.Nazwa.StartsWith("Zrealizowane")).FirstOrDefault();
                var employee = Session.GetPracaHybrydowa().Kadry.Pracownicy.WgKodu["006"];

                var deviceRelated = Add(new Urzadzenie() { Nazwa = "Urządzenie zd1", Identyfikator = "123" });

                var emailMessage = Add(new WiadomoscEmail
                {
                    Od = "testfrom@soneta.pl",
                    Do = "testto@soneta.pl",
                    Temat = "Temat1",
                    Tresc = "Tresc1"
                });

                var project = Session.GetZadania().Projekty.WgNazwy["Projekt"].GetFirst();
                project.Przedstawiciel = representative;
                project.DataOd = DateTime.Today.AddDays(1);
                project.DataDo = DateTime.Today.AddDays(18);

                Assert.AreEqual(contractor.Nazwa, project.Kontrahent.Nazwa, "Kontrahent nie jest przypisany");

                var addEmployee = new ZasobCRM();
                addEmployee.Projekt = project;
                addEmployee.Zasob = employee;
                Add(addEmployee);

                var addDevice = new ZasobCRM();
                addDevice.Projekt = project;
                addDevice.Zasob = deviceRelated;
                Add(addDevice);

                var addEmail = new ElementEmail();
                addEmail.Element = project;
                addEmail.WiadomoscEmail = emailMessage;
                Add(addEmail);

                Assert.AreEqual(employee.Kod, project.ZasobyCRM.Where(x => x.KodZasobu == "006").FirstOrDefault().KodZasobu, "Pracownik jest inny niż przypisywany");
                Assert.AreEqual(deviceRelated.Nazwa, project.ZasobyCRM.Where(x => x.NazwaZasobu == "Urządzenie zd1").FirstOrDefault().NazwaZasobu, "Urządzenie jest inne niż przypisywane");
                Assert.AreEqual(1, project.WiadomosciPowiazane.Count, "Brak wiadomości powiązanej");
            });
            SaveDispose();
        }

        private void AddFullRight(DefZadania defZadania)
        {
            var module = BusinessModule.GetInstance(defZadania);
            foreach (Entitle entitle in module.Entitles)
            {
                if (entitle.Name == Session.Login.Operator.Name)
                {
                    foreach (Row r in module.Rights.WgSource[defZadania, entitle])
                        r.Delete();

                    module.Rights.AddRow(new Right(entitle, defZadania, false));
                    break;
                }
            }
        }

        private void AddFullRight(TypUrzadzenia typUrzadzenia)
        {
            var businessModule = BusinessModule.GetInstance(typUrzadzenia);
            var entitle = businessModule.Entitles.ByName[Session.Login.Operator.Name];
            var rights = businessModule.Rights.WgSource[typUrzadzenia, entitle];

            foreach (var item in rights)
                item.Delete();

            businessModule.Rights.AddRow(new Right(entitle, typUrzadzenia, false));
        }
    }
}
