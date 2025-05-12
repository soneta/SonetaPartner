using JetBrains.Annotations;
using Soneta.Core;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Selectors;

namespace SonetaPartner.Tests.Assemblers
{
	public static class CoreAssembler
	{
		public static ProxyRecord<RelProceduraVAT> SetProceduraVAT(this ProxyRecord<RelProceduraVAT> row, ResolverProceduraVAT proceduraVAT)
			=> row.InTransUI(λ => λ.Row.Procedura = proceduraVAT?.Resolve(λ));

	}
}
