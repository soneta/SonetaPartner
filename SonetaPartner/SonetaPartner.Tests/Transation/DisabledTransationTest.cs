using FluentAssertions;
using NUnit.Framework;
using Soneta.Business;
using Soneta.Test;
using Soneta.Towary;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace SonetaPartner.Tests.Transation
{
	internal class DisabledTransationTest: TestBase
	{
		//wyłączenie transakcyjności - trzeba ręcznie usunąć po teście dane dodane do bazy
		protected override bool EnableDbTransation => false;

		//metoda usuwająca dodane dane
		public override void TestTearDown()
		{
			base.TestTearDown();

			InTransaction(() => {
				var towary = Session.GetTowary().Towary.WgNazwy
					.Where(t => t.Nazwa.StartsWith("xxxxxx"));
				foreach (var tower in towary)
					tower.Delete();
			});
			SaveDispose();
		}

		[Test]
		public void Concurrent_Business_ProducerConsumer()
		{
			_ = Login;

			var collection = new BlockingCollection<Session>();

			var tasks = new Task[3];

			tasks[0] = Concurrent.RunTask(() => Producer(10, collection));
			tasks[1] = Concurrent.RunTask(() => Producer(20, collection));
			tasks[2] = Concurrent.RunTask(() => Producer(30, collection));
			Concurrent.RunTask(() => {
				Task.WaitAll(tasks);
				collection.CompleteAdding();
			});

			foreach (var session in collection.GetConsumingEnumerable())
			{
				session.Save();
				session.Dispose();
			}
			SaveDispose();

			var nazwy = Session.GetTowary().Towary.WgNazwy
				.Where(t => t.Nazwa.StartsWith("xxxxxx"))
				.Select(t => t.Nazwa)
				.ToList();

			nazwy.Should().BeEquivalentTo(
				"xxxxxx10", "xxxxxx11", "xxxxxx12",
				"xxxxxx20", "xxxxxx21", "xxxxxx22",
				"xxxxxx30", "xxxxxx31", "xxxxxx32");
		}

		private void Producer(int StartIdx, BlockingCollection<Session> collection)
		{
			var session = Login.CreateSession(false, false, "Thread Producer " + StartIdx);
			try
			{
				using (var transaction = session.Logout(true))
				{
					for (int i = 0; i < 3; ++i)
					{
						string s = $"xxxxxx{StartIdx + i}";
						session.AddRow(new Towar
						{
							Nazwa = s,
							Kod = s
						});
					}

					transaction.Commit();
				}

				collection.Add(session);
			}
			catch
			{
				session.Dispose();
				throw;
			}
		}
	}
}
