using System;
using System.Collections.Generic;

namespace Nixill.Utils {
  public class Cipher {
    Dictionary<char, string> CharMap;
    public string Source { get; }
    public string Target { get; }
    public bool Reversible { get; }

    public Cipher(string src, string trg) {
      if (src == null) throw new ArgumentNullException("src");
      if (trg == null) throw new ArgumentNullException("trg");

      if (src.Length != trg.Length)
        throw new ArgumentException("Source and target strings must be of equal length.");
      if (src.Length < 1)
        throw new ArgumentException("Can't construct a Cipher with empty strings.");

      Reversible = true;
      Source = src;
      Target = trg;
    }
  }
}