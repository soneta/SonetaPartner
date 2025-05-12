using System;
using System.Collections.Generic;

using Soneta.Test;
using Soneta.Test.Experimental;

using Soneta.Types;
using Soneta.Business;
using Soneta.Kadry;
using Soneta.Kalend;
using Soneta.Place;
using Soneta.Config;
using NUnit.Framework;

namespace SonetaPartner.Tests.Assemblers
{

    public static class UkończonaSzkołaAssembler {

        static public IRowBuilder<UkonczonaSzkola> NowaUkonczonaSzkola(this IRowBuilder<Pracownik> builder, string okres, TypSzkoły typSzkoły) {

            return builder.GetChild<UkonczonaSzkola>().Enqueue(s => {
                s.Nazwa = okres;
                s.Okres = FromTo.Parse(okres);
                s.Szkoła = typSzkoły;
            });
        }

        static public IRowBuilder<Pracownik> Return(this IRowBuilder<UkonczonaSzkola> buider) {
            return buider.GetParent<Pracownik>();
        }
    }
}
