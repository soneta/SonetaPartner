using System;

using JetBrains.Annotations;

using Soneta.Business;
using Soneta.Types;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Engine
{
    public sealed class ProxyRecord<T> : ISessionable
        where T : Row
    {
        public T Row { get; private set; }

        internal ProxyRecord(T row)
            => Row = row;

        public Session Session
            => Row.Session;


        public ProxyRecord<T> Resync(Session session)
        {
            Row = session.InSession(Row);
            return this;
        }
    }

    public sealed class ProxyRecord<T1, T2> : ISessionable
        where T1 : Row, T2
        where T2 : Row
    {
        public T1 Row { get; private set; }


        internal ProxyRecord(T1 row)
            => Row = row;


        public Session Session
            => Row.Session;


        public ProxyRecord<T1, T2> Resync(Session session)
        {
            Row = session.InSession(Row);
            return this;
        }
    }
}
