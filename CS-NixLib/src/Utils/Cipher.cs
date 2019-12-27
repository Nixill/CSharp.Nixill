using System;

namespace Nixill.Utils {
  /// <summary>
  /// Represents a simple one-way substitution cipher.
  /// 
  /// Instances of the Cipher class can be used for multi-character
  /// substitution including swaps and cycles.
  ///
  /// A Cipher is said to be Reversible iff:
  ///
  /// <list type="bullet">
  ///   <item>Every Source character maps to exactly one Target
  ///   character.</item>
  ///   <item>Every Target character has exactly one Source character
  ///   mapped to it.</item>
  ///   <item>Every Target character is a Source character.</item>
  ///   <item>No Source character maps to null.</item>
  /// </list>
  /// </summary>
  public class Cipher {
    GeneratorDictionary<char, string> CharMap = new GeneratorDictionary<char, string>(new SingleValueGenerator<char, string>(""));

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

    /// <summary>
    /// Creates a new Cipher.
    /// </summary>
    public Cipher(string src, string trg) {
      if (src == null) throw new ArgumentNullException("src");
      if (trg == null) throw new ArgumentNullException("trg");

      if (src.Length != trg.Length)
        throw new ArgumentException("Source and target strings must be of equal length.");
      if (src.Length < 1)
        throw new ArgumentException("Can't construct a Cipher with empty strings.");

      Source = src;
      Target = trg;

      int reverseScore = 0;

      GeneratorDictionary<char, int> metaMap = new GeneratorDictionary<char, int>(new SingleValueGenerator<char, int>(0));

      for (int i = 0; i < src.Length; i++) {
        char srcChar = src[i];
        char trgChar = trg[i];

        CharMap[srcChar] += trgChar;

        if (reverseScore >= 0) {
          // Make sure the target isn't null
          if (trgChar == '\0') reverseScore = 1;
          else {
            // Each char should only be a source once
            int score = metaMap[srcChar];
            if (score == 1 || score == 3) {
              // Char is already a source, stop tracking
              reverseScore = -1;
            }
            else {
              // Char isn't yet a source
              if (score == 0) {
                // Char isn't a target
                reverseScore += 1;
                score = 1;
              }
              else /* srcScore == 2 */ {
                // Char is a target
                reverseScore -= 1;
                score = 3;
              }
              metaMap[srcChar] = score;

              // Each char should only be a target once
              score = metaMap[trgChar];
              if (score == 2 || score == 3) {
                // Char is already a target, stop tracking
                reverseScore = -1;
              }
              else {
                // Char isn't yet a target
                if (score == 0) {
                  // Char isn't a source
                  reverseScore += 1;
                  score = 2;
                }
                else /* score == 1 */ {
                  // Char is a source
                  reverseScore -= 1;
                  score = 3;
                }
                metaMap[trgChar] = score;
              }
            }
          }
        }
      }

      Reversible = reverseScore == 0;
    }

    /// <summary>
    /// Applies the cipher to a string.
    ///
    /// Within the input string, any character matching a character in the
    /// source string is replaced with a character in the same position in
    /// the target string.
    /// </summary>
    public string Apply(string input) {
      string ret = "";

      for (int i = 0; i < input.Length; i++) {
        char inChar = input[i];
        if (CharMap.ContainsKey(inChar)) {
          string outChars = CharMap[inChar];
          char outChar = outChars[i % outChars.Length];
          if (outChar != '\0') ret += outChar;
        }
        else ret += inChar;
      }

      return ret;
    }
  }
}