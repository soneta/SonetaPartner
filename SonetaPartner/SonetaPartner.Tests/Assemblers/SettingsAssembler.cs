using System;
using Soneta.Core;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;

namespace SonetaPartner.Tests.Assemblers
{ 
    public static class AssemblerSettings
    {
        #region Core
        public static ProxySettings<CoreModule> Ogolne_CentraKosztow_Zablokowane(this ProxySettings<CoreModule> settings,
         string nazwa, bool setOn = true)
         => settings
             .InTransUI(λ => λ.Module.CentraKosztow.WgNazwy[nazwa].Zablokowane = setOn);
        #endregion
    }
}
