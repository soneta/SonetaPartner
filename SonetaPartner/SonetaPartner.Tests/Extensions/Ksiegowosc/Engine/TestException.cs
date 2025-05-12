using JetBrains.Annotations;
using System;

namespace SonetaPartner.Tests.Extensions.Ksiegowosc.Engine
{
    public sealed class TestException : Exception
    {
        internal TestException(string message)
            : base($"Test-Exc: {message}")
        { }

        public static TestException MakeEnumOutOfRange(Enum en, string text)
            => new TestException($"Enum out of range: {en} ({text}).");
    }
}
