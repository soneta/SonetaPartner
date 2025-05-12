using Soneta.Business;
using Soneta.Business.App;
using Soneta.CRM;
using Soneta.Test;
using Soneta.Types;
using Soneta.Zadania;

namespace SonetaPartner.Tests.Assemblers
{
    internal static class ZadanieAssembler
    {
        internal static IRowBuilder<Zadanie> WithName(this IRowBuilder<Zadanie> builder, string value)
            => builder.Enqueue(x => x.Nazwa = value);

        internal static IRowBuilder<Zadanie> WithDateFrom(this IRowBuilder<Zadanie> builder, string value)
            => builder.Enqueue(x => x.DataOd = Date.Parse(value));

        internal static IRowBuilder<Zadanie> WithDateTo(this IRowBuilder<Zadanie> builder, string value)
            => builder.Enqueue(x => x.DataDo = Date.Parse(value));

        internal static IRowBuilder<Zadanie> WithTimeFrom(this IRowBuilder<Zadanie> builder, string value)
            => builder.Enqueue(x => x.CzasOd = Time.Parse(value));

        internal static IRowBuilder<Zadanie> WithTimeTo(this IRowBuilder<Zadanie> builder, string value)
            => builder.Enqueue(x => x.CzasDo = Time.Parse(value));
    }
}
