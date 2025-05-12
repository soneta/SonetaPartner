using Soneta.Kasa;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;

namespace SonetaPartner.Tests.Assemblers
{
    public static class AssemblerKasa
    {
        #region Przelew

        public static ProxyRecord<T1, PrzelewBase> SetZatwierdzony<T1>(this ProxyRecord<T1, PrzelewBase> row, bool setOn = true)
            where T1 : PrzelewBase
            => row.InTransUI(λ => λ.Row.Zatwierdzony = setOn);

        #endregion
    }
}
