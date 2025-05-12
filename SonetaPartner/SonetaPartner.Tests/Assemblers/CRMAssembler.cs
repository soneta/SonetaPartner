using JetBrains.Annotations;
using Soneta.Core;
using Soneta.CRM;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Settings;

namespace SonetaPartner.Tests.Assemblers
{
    public static class AssemblerCRM
    {

        #region Kontrahent

        public static ProxyRecord<Kontrahent> SetSposobZaplaty(this ProxyRecord<Kontrahent> row, ResolverFormaPlatnosci formaPlatnosci)
            => row.InTransUI(λ => λ.Row.SposobZaplaty = formaPlatnosci.Resolve(λ));

        public static ProxyRecord<Kontrahent> NewRachunek(this ProxyRecord<Kontrahent> row, string numer = Defaults.NumerRachunkuKontrahenta)
            => row.NewRachunek(out _, numer);

        public static ProxyRecord<Kontrahent> NewRachunek(this ProxyRecord<Kontrahent> row, out ProxyRecord<RachunekBankowyKontrahenta> newRow, string numer = Defaults.NumerRachunkuKontrahenta, OddzialFirmy oddzial = null)
        {
            row.InTransUIRes(sess => sess.AddRow(new RachunekBankowyKontrahenta(row.Row)))
                .Box()
                .ConditionallyObj(numer, (λ, v) => λ.SetNumer(v))
                .ConditionallyObj(oddzial, (λ, v) => λ.SetOddzial(v))
                .Out(out newRow);

            return row;
        }

        #endregion

        #region RachunekBankowyKontrahenta

        public static ProxyRecord<RachunekBankowyKontrahenta> SetNumer(this ProxyRecord<RachunekBankowyKontrahenta> row, string numer = Defaults.NumerRachunkuKontrahenta)
            => row.InTransUI(λ => λ.Row.Rachunek.Numer.Numer = numer);

        public static ProxyRecord<RachunekBankowyKontrahenta> SetOddzial(this ProxyRecord<RachunekBankowyKontrahenta> row, OddzialFirmy oddzial)
            => row.InTransUI(λ => λ.Row.Oddzial = λ.Row.Session.Get(oddzial));

        #endregion

    }
}
