using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Soneta.Business;
using Soneta.Kadry;
using Soneta.Place;

namespace SonetaPartner.Tests.Assemblers
{

    static class Tools {

        public static DefinicjaElementu GetDefinicjaElementu(ISessionable session, string nazwa) {
            if (session == null)
                throw new ArgumentNullException(nameof(session));
            if (nazwa == null)
                throw new ArgumentNullException();
            var e = session.Session.GetPlace().DefElementow.WgNazwy[nazwa];
            Assert.NotNull(e, $"Nie znaleziono definicji elemetu wynagrodzenia '{nazwa}'.");
            return e;
        }
    }
}
