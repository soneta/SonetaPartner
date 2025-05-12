using System;
using Soneta.Test;
using Soneta.Types;
using Soneta.Business;
using Soneta.Kadry;


namespace SonetaPartner.Tests.Assemblers
{

    #region UmowaAssembler

    public static class UmowaAssembler {

        static public IRowBuilder<Umowa> DodajUmowe(this IRowBuilder<Pracownik> builder, string element, string okres, Currency? stawka = null, string wydział = null) 
            => builder.GetChild<Umowa>().Ustaw(element, okres, stawka, wydział);        

        static public IRowBuilder<Umowa> Ustaw(this IRowBuilder<Umowa> builder, string element, string okres, string wydział = null) {                        
            return builder.Enqueue(u => {
                var ft = FromTo.Parse(okres);
                u.Wydzial = string.IsNullOrEmpty(wydział) ? u.Module.Wydzialy.Firma : u.Module.Wydzialy.WgKodu[wydział];                
                u.Element = Tools.GetDefinicjaElementu(u, element);
                u.Data = ft.From;
                u.Okres = ft;
                u.Tytul = u.Element.Nazwa;
            });
        }

        static public IRowBuilder<Umowa> Ustaw(this IRowBuilder<Umowa> builder, string element, string okres, Currency? stawka = null, string wydział = null) {

            return builder.Ustaw(element, okres, wydział).Last().Ustaw(stawka).Return();
        }

        static public IRowBuilder<Umowa> MinimalnaStawkaGodz(this IRowBuilder<Umowa> builder, bool minimalnaStawkaGodz) {

            return builder.Enqueue(u => {
                if (u.MinimalnaStawkaGodz != minimalnaStawkaGodz)
                    u.MinimalnaStawkaGodz = minimalnaStawkaGodz;
            });
        }

        static public IRowBuilder<Pracownik> Return(this IRowBuilder<Umowa> builder) {
            return builder.GetParent<Pracownik>();
        }
    }

    #endregion

    #region UmowaHistoriaAssemler

    public static class UmowaHistoriaAssemler {

        static public IRowBuilder<UmowaHistoria> Last(this IRowBuilder<Umowa> builder) {
            return builder.GetChild(umowa => umowa.Last);
        }

        static public IRowBuilder<Umowa> Return(this IRowBuilder<UmowaHistoria> builder) {
            return builder.GetParent<Umowa>();
        }

        static public IRowBuilder<UmowaHistoria> Ustaw(this IRowBuilder<UmowaHistoria> builder, Currency? stawka = null) {

            return builder.Enqueue(uh => {
                if (stawka != null)
                    uh.Wartosc = (Currency)stawka;

            });
        }
    }

    #endregion
}
