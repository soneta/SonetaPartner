using NUnit.Framework;
using Soneta.Business;
using Soneta.Business.Db;
using Soneta.Config;
using Soneta.Core;
using Soneta.Test;
using System.Diagnostics;

namespace SonetaPartner.Tests.Extensions.KadryIPlace.Engine
{

    public class PlTestBase : TestBase
    {

        static public IRowBuilder<CfgNode> NowyTest()
        {
            return Nowy();
        }

        static public IRowBuilder<CfgNode> Nowy()
        {
            return new RowBuilder<CfgNode>((t, ctx) =>
            {
                return ctx.Session.Get<BusinessModule>().CfgNodes.Root;
            }, BuilderOptions.SetResultIntoContext_No | BuilderOptions.SessionMode_UseSession);
        }

        static public IRowBuilder<CfgNode> Config()
        {
            return new RowBuilder<CfgNode>((t, ctx) =>
            {
                return ctx.Session.Get<BusinessModule>().CfgNodes.Root;
            }, BuilderOptions.SetResultIntoContext_No | BuilderOptions.SessionMode_UseConfigSession);
        }
    }
}
