using Soneta.Business;
using Soneta.Types;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonetaPartner.Tests.Assemblers
{
	public static class AssemblerRow
	{
		#region update

		public static ProxyRecord<T1, T2> Update<T1, T2>(this ProxyRecord<T1, T2> row, Date? data = null)
			where T1 : Row, T2
			where T2 : Row, IRowWithHistory
			=> row.InTransUI(λ => λ.Row.Historia.Update(data ?? TestKsiegowosc.Day()));

		#endregion
	}
}
