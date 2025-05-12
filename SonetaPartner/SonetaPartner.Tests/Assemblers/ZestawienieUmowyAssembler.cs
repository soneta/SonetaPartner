using Soneta.Test;
using Soneta.Test.Experimental;
using Soneta.Types;
using Soneta.Kalend;
using NUnit.Framework;
using Soneta.Kadry;

namespace SonetaPartner.Tests.Assemblers
{

    public static class ZestawienieUmowyAssembler {

        static public IRowBuilder<ZestawienieUmowy> DodajZestawienie(this IRowBuilder<Umowa> builder, string okres, string czas = null, string ilość = null)  {
            return builder.
                GetChild<ZestawienieUmowy>().
                Ustaw(okres, czas, ilość);
        }

        static public IRowBuilder<ZestawienieUmowy> Ustaw(this IRowBuilder<ZestawienieUmowy> builder, string okres, string czas = null, string ilość = null) {
            return builder.Enqueue(zp => {
                zp.Okres = FromTo.Parse(okres);
                if (czas != null)
                    zp.Czas = Time.Parse(czas);
                if (ilość != null)
                    zp.Ilość = int.Parse(ilość);
            });
        }

        static public IRowBuilder<Umowa> Return(this IRowBuilder<ZestawienieUmowy> buider) {
            return buider.GetParent<Umowa>();
        }
    }
}
