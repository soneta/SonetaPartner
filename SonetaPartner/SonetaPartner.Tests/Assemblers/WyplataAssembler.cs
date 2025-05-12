using NUnit.Framework;
using Soneta.Place;
using Soneta.Test;
using Soneta.Types;
using System;
using System.Linq;

namespace SonetaPartner.Tests.Assemblers
{
	public static class WyplataAssembler
	{
		static public IRowBuilder<Wyplata> SprawdzSume(this IRowBuilder<Wyplata> builder, string opis, decimal wymagana, Func<WypElement, decimal> func)
			=> builder.SprawdzSume(opis, null, null, wymagana, func);

		static public IRowBuilder<Wyplata> SprawdzSume(this IRowBuilder<Wyplata> builder, string opis, Func<WypElement, bool> filterE, decimal wymagana, Func<WypElement, decimal> func = null)
			=> builder.SprawdzSume(opis, null, filterE, wymagana, func);

		static public IRowBuilder<Wyplata> SprawdzSume(this IRowBuilder<Wyplata> builder, string opis, Func<Wyplata, bool> filterW, Func<WypElement, bool> filterE, decimal wymagana, Func<WypElement, decimal> func = null)
			=> builder.Enqueue(wypłata => {
				if (filterW == null || filterW(wypłata))
				{
					decimal suma = 0;
					int c = 0;
					foreach (WypElement e in wypłata.Elementy)
						if (filterE?.Invoke(e) ?? true)
						{
							suma += func != null ? func(e) : e.Wartosc;
							c++;
						}
					if (wymagana != decimal.MinValue)
						Assert.AreEqual(wymagana, suma, $"{wypłata}, {opis}, elementy: {c}");
					else if (c > 0)
						Assert.Fail($"Nieoczekiwany element {wypłata}, {opis}");
				}
			});

		static public IRowBuilder<Wyplata> SprawdzKoszty(this IRowBuilder<Wyplata> builder, decimal wymagana)
			=> builder.SprawdzSume("Koszty pit", null, null, wymagana, e => e.Podatki.Koszty);

		static public IRowBuilder<Wyplata> SprawdzUlge(this IRowBuilder<Wyplata> builder, decimal wymagana)
			=> builder.SprawdzSume("Ulga pit", null, null, wymagana, e => e.Podatki.Ulga);

		static public IRowBuilder<Wyplata> SprawdzDoWyplaty(this IRowBuilder<Wyplata> builder, decimal wymagana)
			=> builder.SprawdzSume("Do wypłaty elementów", null, null, wymagana, e => e.DoWypłaty);

		static public IRowBuilder<Wyplata> Zatwierdz(this IRowBuilder<Wyplata> builder)
			=> builder.Enqueue(wypłata => wypłata.Zatwierdzona = true);
	}
}
