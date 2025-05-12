using System;
using System.Globalization;
using System.Threading;

using JetBrains.Annotations;

using Soneta.Business;
using Soneta.Test;
using Soneta.Types;
using Action = System.Action;

[assembly: TestAssemblyInitializer(options: Options.Default | Options.LoadUI)]

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Engine
{
    public abstract partial class TestKsiegowosc : TestBase
    {
        public Finder GetFinder(bool confSession = false)
            => new Finder(confSession ? ConfigEditSession : Session);

        public ProxySettings<T1> Settings<T1>()
            where T1 : Module
            => new ProxySettings<T1>(ConfigEditSession.Get<T1>());

        public static Date Day()
            => Soneta.Test.TimeDefaults.Day();

    }
}
