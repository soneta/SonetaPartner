using System;
using System.Collections.Generic;
using Soneta.CRM;
using Soneta.Handel;
using Soneta.Towary;
using Soneta.Types;

namespace SonetaPartner.Tests.Extensions.Magazyn.Engine
{
	class Input : ICloneable
	{
		Date _data;
		bool _jakZamknięcie = true;
		Kontrahent _kontrahent;
		Magazyn[] _magazyny = new Magazyn[0];
		OkresMagazynowy _okres;
		Towar _towar;
		List<DefDokHandlowego> _wykluczone;

		internal bool JakZamknięcie
		{
			get { return _jakZamknięcie; }
			set
			{
				TryChange(
				  v => _jakZamknięcie = v,
				  nameof(JakZamknięcie),
				  new[] { _jakZamknięcie, value });
			}
		}

		internal Kontrahent Kontrahent
		{
			get { return _kontrahent; }
			set
			{
				TryChange(
				  v => _kontrahent = v,
				  nameof(Kontrahent),
				  new[] { _kontrahent, value });
			}
		}

		internal Magazyn[] Magazyny
		{
			get { return _magazyny; }
			set
			{
				TryChange(
				  v => _magazyny = v,
				  nameof(Magazyny),
				  new[] { _magazyny, value ?? new Magazyn[0] });
			}
		}

		internal OkresMagazynowy Okres
		{
			get { return _okres; }

			set
			{
				TryChange(v => _okres = v, nameof(Okres), new[] { _okres, value });
				if (!_okres.Okres.Contains(_data))
				{
					TryChange(
					  v => _data = v,
					  nameof(Okres),
					  new[] { _data, value.Okres.To });
				}
			}
		}

		internal Towar Towar
		{
			get { return _towar; }
			set
			{
				TryChange(v => _towar = v, nameof(Towar), new[] { _towar, value });
			}
		}

		internal List<DefDokHandlowego> WykluczoneDefinicje
		{
			get { return _wykluczone; }
			set
			{
				TryChange(
				  v => _wykluczone = v,
				  nameof(WykluczoneDefinicje),
				  new[] { _wykluczone, value });
			}
		}

		object ICloneable.Clone() => MemberwiseClone();

		internal event EventHandler Changed;

		void Change<TValue>(Action<TValue> set, string sender, TValue value)
		{
			set(value);
			Changed?.Invoke(sender, EventArgs.Empty);
		}

		void TryChange<TValue>(Action<TValue> set, string sender, TValue[] values)
		{
			if (!Equals(values[0], values[1]))
			{
				Change(set, sender, values[1]);
			}
		}
	}
}
