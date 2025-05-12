using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using JetBrains.Annotations;

using Soneta.Business;
using Soneta.Deklaracje;
using Soneta.Kasa;
using Soneta.Ksiega;
using Soneta.Towary;
using Soneta.Types;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Engine;
using SonetaPartner.Tests.Extensions.Ksiegowosc.Settings;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Engine
{
    public static class ExtensionMethods
    {
        #region checks

        public static T1 ReturnChecked<T1>(this T1 obj, Func<string> fnGetMessage)
            where T1 : class
        {
            if (obj == null)
                throw new TestException(fnGetMessage());
            return obj;
        }

        public static T1 ReturnCondition<T1>(this T1 obj, Func<T1, bool> fnChecker, Func<T1, string> fnGetMessage)
            where T1 : class
        {
            if (!fnChecker(obj))
                throw new TestException(fnGetMessage(obj));
            return obj;
        }

        #endregion

        #region code control

        public static T1 With<T1>(this T1 obj, [InstantHandle] [NotNull] Action<T1> fnAction)
        {
            fnAction(obj);
            return obj;
        }

        public static T1 WithOptional<T1>(this T1 obj, [InstantHandle] Action<T1> fnAction)
        {
            fnAction?.Invoke(obj);
            return obj;
        }

        public static T1 Out<T1>(this T1 obj, out T1 variable)
        {
            variable = obj;
            return obj;
        }

        public static T1 ExpectType<T1>(this object obj)
            where T1 : class
        {
            if (obj is T1 t1)
                return t1;

            throw new TestException($"Expected object type '{typeof(T1).FullName}'. Actually '{(obj != null ? obj.GetType().FullName : "(null)")}'.");
        }

        public static T1 Conditionally<T1>(this T1 obj, bool condition, Action<T1> fnAction)
        {
            if (condition)
                fnAction(obj);
            return obj;
        }

        public static T1 ConditionallyVal<T1, T2>(this T1 obj, T2? condition, Action<T1, T2> fnAction)
            where T2 : struct
        {
            if (condition != null)
                fnAction(obj, condition.Value);
            return obj;
        }

        public static T1 ConditionallyObj<T1, T2>(this T1 obj, T2 condition, Action<T1, T2> fnAction)
            where T2 : class
        {
            if (condition != null)
                fnAction(obj, condition);
            return obj;
        }

        public static List<T2> ExtractFromWorker<T1, T2>(this object workerResult, Func<T1, IEnumerable<T2>> fnExtract)
        {
            switch (workerResult)
            {
                case T1 worker:
                    return new List<T2>(fnExtract(worker));
                case T2 single:
                    return new List<T2> {single};
                case null:
                    return new List<T2>();
                default:
                    throw new TestException($"ExtractFromWorker: result's type is neither {typeof(T1)} or {typeof(T2)}.");
            }
        }

        #endregion

        #region collections

        public static IEnumerable<T1> Nnc<T1>(this IEnumerable<T1> enumerable)
            => enumerable ?? Enumerable.Empty<T1>();

        public static T1 InCollection<T1>(this IEnumerable enumerable, bool skipOtherTypes = false, bool allowMany = false, bool allowNothing = false, Func<T1, bool> fnSelector = null)
            where T1 : class
        {
            if (enumerable == null)
                throw new ArgumentException(nameof(enumerable));

            T1 found = null;
            foreach (var elem in enumerable)
            {
                if (elem == null)
                    throw new TestException($"W kolekcji '{enumerable.GetType().Name}' znaleziono element (null).");

                if (!(elem is T1 elemT1))
                {
                    if (!skipOtherTypes)
                        throw new TestException($"W kolekcji '{enumerable.GetType().Name}' znaleziono element typu '{elem.GetType().Name}'.");
                    continue;
                }

                if (fnSelector != null && !fnSelector(elemT1))
                    continue;
                if (found != null && !allowMany)
                    throw new TestException($"W kolekcji '{enumerable.GetType().Name}' znaleziono więcej niż 1 element.");

                found = elemT1;
            }

            if (found == null && !allowNothing)
                throw new TestException($"W kolekcji '{enumerable.GetType().Name}' nie znaleziono elementów typu '{typeof(T1).Name}'.");

            return found;
        }

        public static T1 InCollection<T1>(this IEnumerable<T1> enumerable, bool allowMany = false, bool allowNothing = false, Func<T1, bool> fnSelector = null)
            where T1 : class
        {
            if (enumerable == null)
                throw new ArgumentException(nameof(enumerable));

            T1 found = null;
            foreach (var elem in enumerable)
            {
                if (fnSelector != null && !fnSelector(elem))
                    continue;
                if (found != null && !allowMany)
                    throw new TestException($"W kolekcji '{enumerable.GetType().FullName}' znaleziono więcej niż 1 element.");

                found = elem ?? throw new TestException($"W kolekcji '{enumerable.GetType().FullName}' znaleziono element (null).");
            }

            if (found == null && !allowNothing)
                throw new TestException($"W kolekcji '{enumerable.GetType().FullName}' nie znaleziono elementów typu '{typeof(T1).FullName}'.");

            return found;
        }

        public static List<T1> ProcessAsList<T1>([NotNull] this IEnumerable<T1> enumerable, [CanBeNull] Action<T1> fnAction)
        {
            if (enumerable == null)
                throw new ArgumentException(nameof(enumerable));

            var list = enumerable.ToList();

            if (fnAction != null)
                foreach (var elem in list)
                    fnAction.Invoke(elem);

            return list;
        }

        public static T1[] ToArrayOfType<T1>(this IEnumerable anyCollection)
            => anyCollection is IEnumerable<T1> typed ? typed.ToArray() : null;

        #endregion

        #region session, transactions

        public static T1 InTransUI<T1>(this T1 sProvider, Action<T1> fnAction)
            where T1 : ISessionable
        {
            using (var transaction = sProvider.Session.Logout(true))
            {
                fnAction(sProvider);
                transaction.CommitUI();
            }

            return sProvider;
        }

        public static T2 InTransUIRes<T1, T2>(this T1 sProvider, Func<Session, T2> fnAction)
            where T1 : ISessionable
        {
            T2 result;

            using (var transaction = sProvider.Session.Logout(true))
            {
                result = fnAction(sProvider.Session);
                transaction.CommitUI();
            }

            return result;
        }

        public static T2 InSession<T1, T2>(this T1 sProvider, T2 row)
            where T1 : ISessionable
            where T2 : Row
            => sProvider.Session.Get(row);

        public static IEnumerable<T2> InSession<T1, T2>(this T1 sProvider, IEnumerable<T2> rows)
            where T1 : ISessionable
            where T2 : Row
            => rows.Nnc().Select(row => sProvider.Session.Get(row));

        public static T2[] InSession<T1, T2>(this T1 sProvider, T2[] rows)
            where T1 : ISessionable
            where T2 : Row
        {
            if (rows != null)
                for (var ix = 0; ix < rows.Length; ix++)
                    rows[ix] = sProvider.Session.Get(rows[ix]);

            return rows;
        }

        public static Finder Finder(this ISessionable sProvider)
            => new Finder(sProvider.Session);

        public static T1 GoSave<T1>(this T1 sProvider)
            where T1 : ISessionable
        {
            sProvider.Session.Save();
            sProvider.Session.Dispose();

            return sProvider;
        }

        #endregion

        #region types: Date/FromTo

        public static Date AsDemoDate(this (int month, int day) date)
            => new Date(Defaults.Okres, date.month, date.day);

        #endregion

        #region types: Currency

        public static Currency Zloty(this int kwota)
            => new Currency((decimal) kwota, Currency.SystemSymbol);

        public static Currency Zloty(this double kwota)
            => new Currency(kwota, Currency.SystemSymbol);

        #endregion

        #region Manager księgowań

        public static IEnumerable<ZapisKsiegowy> GetZapisy(this ManagerKsiegowan.Rezultat res)
        {
            if (res.Errors.Count > 0)
                throw new Exception("Menedżer księgowań zwrócił błędy.");

            return res.Dekrety.Cast<DekretBase>().Nnc().SelectMany(d => d.Zapisy);
        }

        #endregion
    }
}
