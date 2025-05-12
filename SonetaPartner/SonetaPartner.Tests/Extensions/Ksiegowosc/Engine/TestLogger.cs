using Soneta.Kasa;
using System.Collections.Generic;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Engine
{
	public sealed class TestLogger : ILogOnDemand
	{
		public List<string> Logs { get; } = new List<string>();


		bool ILogOnDemand.Enabled
		{
			get => true;
			set { }
		}


		public bool HasEntries
			=> Logs.Count > 0;


		void ILogOnDemand.WriteLine(string str)
			=> Logs.Add(str);
	}
}
