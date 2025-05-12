using System.Linq;
using Soneta.Business;
using Soneta.CRM;
using Soneta.PracaZdalna;
using Soneta.CRM.Config;
using Soneta.Test;
using Soneta.Core;
using Soneta.Kadry;
using Soneta.Zadania;

namespace SonetaPartner.Tests.Extensions.CRM.Engine
{
    public class TaskBase : TestBase
    {

        protected IRowBuilder<T> NewRow<T>() where T : Row
            => new RowBuilder<T>();

        protected DefZadania GetTaskDefinitionZAD()
            => GetTaskDefinitionBySymbol("ZAD");

        protected DefZadania GetTaskDefinitionBySymbol(string symbol)
            => !string.IsNullOrWhiteSpace(symbol)
                ? Session.GetZadania().DefZadan.WgSymbolu[symbol]
                : null;


    }


}

