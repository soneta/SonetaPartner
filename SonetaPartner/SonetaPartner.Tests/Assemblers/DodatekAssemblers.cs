using Soneta.Test;
using Soneta.Types;
using Soneta.Business;
using Soneta.Kadry;
using Soneta.Place;

namespace SonetaPartner.Tests.Assemblers
{

    public static class DodatekAssembler {

        static public IRowBuilder<Dodatek> DodajDodatek(this IRowBuilder<Pracownik> builder) {

            return builder.GetChild<Dodatek>();
        }

        static public IRowBuilder<Dodatek> DodajDodatek(this IRowBuilder<Pracownik> builder, string definicja, string okres, decimal? podstawa = null) {

            return builder.DodajDodatek().Last().Ustaw(definicja, okres, podstawa).Return();
        }

        public static IRowBuilder<T> Return<T>(this IRowBuilder<Dodatek> builder) where T : Row => builder.GetParent<T>();

        public static IRowBuilder<Pracownik> Return(this IRowBuilder<Dodatek> builder) => Return<Pracownik>(builder);

    }

    public static class DodHistoriaAssemler {

        static public IRowBuilder<DodHistoria> Last(this IRowBuilder<Dodatek> builder) {
            return builder.GetChild(pracownik => pracownik.Last);
        }

        static public IRowBuilder<Dodatek> Return(this IRowBuilder<DodHistoria> builder) {
            return builder.GetParent<Dodatek>();
        }

        static public IRowBuilder<DodHistoria> Ustaw(this IRowBuilder<DodHistoria> builder, string element = null, string okres = null, Currency? podstawa = null) {
            return builder.Enqueue(dh => {
                if (element != null)
                    dh.Element = dh.Session.Get<PlaceModule>().DefElementow.WgNazwy[element];
                if (okres != null)
                    dh.Okres = FromTo.Parse(okres);
                else if (dh.Okres == FromTo.Empty)
                    dh.Okres = FromTo.All;
                if (podstawa != null)
                    dh.Podstawa = (Currency)podstawa;
            });
        }

    }
}
