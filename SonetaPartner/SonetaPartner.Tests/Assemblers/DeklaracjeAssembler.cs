using NUnit.Framework;
using Soneta.Deklaracje;
using Soneta.Deklaracje.PIT;
using Soneta.Deklaracje.ZUS;
using Soneta.Kadry;
using Soneta.Test;
using Soneta.Types;

namespace SonetaPartner.Tests.Assemblers
{
	public static class DeklaracjeAssembler
	{
		#region PIT8AR

		static public IRowBuilder<T> NowyPIT<T>(this IRowBuilder<Pracownik> builder, int rok, string data = null) where T : ZgłoszeniowaPIT
		{
			return builder.GetChild<T>().Enqueue(pit => {
				pit.Rok = rok;
				pit.Data = string.IsNullOrEmpty(data) ? new Date(rok, 12, 31) : Date.Parse(data);
				pit.Przelicz();
			});
		}

		static public IRowBuilder<Pracownik> Return<T>(this IRowBuilder<T> builder) where T : ZgłoszeniowaPIT
			=> builder.GetParent<Pracownik>();

		#endregion

		static public IRowBuilder<D> SprawdzPole<D, W>(this IRowBuilder<D> builder, string pole, W wymagana)
			where D : Deklaracja
		{
			return builder.SprawdzPole(pole, pole, wymagana);
		}

		static public IRowBuilder<D> SprawdzPole<D, W>(this IRowBuilder<D> builder, string opis, string pole, W wymagana)
			where D : Deklaracja
		{
			return builder.Enqueue((d, ctx) => {
				string[] ss = pole.Split('.');
                Assert.That(2, Is.EqualTo(ss.Length), "Oczekiwana wartość postaci BLOK.POLE");
				W wartość = (W)d.Bloki[ss[0]][ss[1]];
                Assert.That(wartość, Is.EqualTo(wymagana), $"{opis}, deklaracja '{d.Podmiot}/{Root(d)}', pole '{pole}'.");
			});
		}

		static Deklaracja Root(Deklaracja d)
		{
			while (d != null && !(d is DRA))
				d = d.Deklaracja;
			return d;
		}
	}
}
