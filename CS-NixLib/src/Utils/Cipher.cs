using System;
using System.Collections.Generic;

namespace Nixill.Utils {
  /// <summary>
  /// Represents a simple one-way substitution cipher.
  /// 
  /// Instances of the Cipher class can be used for multi-character substitution including swaps and cycles.
  /// </summary>
  public class Cipher {
    Dictionary<char, string> CharMap;
    
    /// <summary>
    /// The string representing the characters being substituted.
    /// </summary>
    public string Source { get; }

    /// <summary>
    /// The string representing the substitute characters.
    /// </summary>
    public string Target { get; }

    /// <summary>
    /// Whether or not this is a reversible cipher.
    /// </summary>
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