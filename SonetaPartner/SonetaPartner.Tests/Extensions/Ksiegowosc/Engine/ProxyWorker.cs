using System;
using JetBrains.Annotations;
using Soneta.Business;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Engine
{
    public abstract class ProxyWorkerBase<T> : ISessionable
    {
        private TestLogger _logger;

        public Session Session { get; }
        public T Worker { get; }

        public TestLogger Logger
            => _logger = _logger ?? new TestLogger();

        protected ProxyWorkerBase([NotNull] Session session, [NotNull] T worker)
        {
            Session = session;
            Worker = worker;
        }
    }

    public sealed class ProxyWorker<T> : ProxyWorkerBase<T>
    {
        private readonly Action<T> _executor;
        private readonly bool _openTrans;

        public ProxyWorker([NotNull] Session session, [NotNull] T worker, [NotNull] Action<T> executor, bool openTrans = false)
            : base(session, worker)
        {
            _executor = executor;
            _openTrans = openTrans;
        }

        public ProxyWorker<T> ExecuteAndSave(bool wSave = true)
        {
            Execute();

            if (wSave)
                this.GoSave();

            return this;
        }

        private void Execute()
        {
            if (_openTrans)
            {
                Session.InTransUI(sess => _executor(Worker));
                return;
            }

            _executor(Worker);
        }
    }

    public sealed class ProxyWorker<TW, TR> : ProxyWorkerBase<TW>
    {
        private readonly Func<TW, TR> _executor;
        private readonly bool _openTrans;

        public TR Result { get; private set; }

        public ProxyWorker(Session session, [NotNull] TW worker, [NotNull] Func<TW, TR> executor, bool openTrans = false)
            : base(session, worker)
        {
            _executor = executor;
            _openTrans = openTrans;
        }

        public ProxyWorker<TW, TR> Execute(out TR result, bool wSave = false)
        {
            result = Result = CallExecute();

            if (wSave)
                this.GoSave();

            return this;
        }

        public ProxyWorker<TW, TR> Execute(bool wSave = false)
            => Execute(out _, wSave);

        public TR ExecuteResult(bool wSave = false)
        {
            Result = CallExecute();

            if (wSave)
                this.GoSave();

            return Result;
        }

        private TR CallExecute()
            => _openTrans ? Session.InTransUIRes(sess => _executor(Worker)) : _executor(Worker);
    }
}
