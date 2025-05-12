using Soneta.Business.App;
using Soneta.Handel;
using Soneta.Test;
using Soneta.Types;

namespace SonetaPartner.Tests.Assemblers
{
	static class ConfigAssembler
	{
		internal static IRowBuilder<DefDokHandlowego> UstawRabatOperatora(this IRowBuilder<DefDokHandlowego> builder, decimal rabat)
		{
			return builder.Enqueue(
				d =>
				{
					var @operator = (Operator)d.Session[d.Session.Login.Operator];
					d.Module.Config.Operatorzy[@operator].MaksymalnyProcentRabatu = new Percent(rabat);
				});
		}
	}
}
