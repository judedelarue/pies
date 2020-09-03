using System;
using System.Diagnostics.CodeAnalysis;

namespace Services.Dto
{
    public class Pie : IEquatable<Pie>
    {
        public string Flavour { get; set; }
        public String LastMadeOn { get; set; }

        public bool Equals([AllowNull] Pie other)
        {
            return other != null &&
                 other.LastMadeOn == LastMadeOn
                 && string.Equals(other.Flavour, Flavour, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}