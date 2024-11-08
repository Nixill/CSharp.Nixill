using System;
using System.Text.RegularExpressions;
using Nixill.Utils;

namespace Nixill.Objects
{
  /// <summary>
  /// Represents a https://semver.org/ 2.0.0 compatible Semantic Version.
  /// </summary>
  public class SemVer : IComparable<SemVer>
  {
    /// <value>The major version number, as defined by spec rule 8.</value>
    public readonly int Major;
    /// <value>The minor version number, as defined by spec rule 7.</value>
    public readonly int Minor;
    /// <value>The patch number, as defined by spec rule 6.</value>
    public readonly int Patch;
    /// <value>The pre-release tag, as defined by spec rule 9.</value>
    public readonly string PreRelease = null;
    /// <value>The build metadata, as defined by spec rule 10.</value>
    public readonly string BuildMetadata = null;

    private static readonly Regex PreReleaseRegex = new Regex("^(?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\\.(?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*))*$");
    private static readonly Regex BuildMetadataRegex = new Regex("^[0-9a-zA-Z-]+(?:\\.[0-9a-zA-Z-]+)*$");
    private static readonly Regex FullRegex = new Regex("^(?<major>0|[1-9]\\d*)\\.(?<minor>0|[1-9]\\d*)\\.(?<patch>0|[1-9]\\d*)(?:-(?<prerelease>(?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\\.(?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\\+(?<buildmetadata>[0-9a-zA-Z-]+(?:\\.[0-9a-zA-Z-]+)*))?$");

    /// <summary>Constructs a <c>SemVer</c> consisting only of the main numbers.</summary>
    public SemVer(int major, int minor, int patch) : this(major, minor, patch, null, null) { }
    /// <summary>Constructs a <c>SemVer</c> consisting of all parts.
    ///
    /// <c>prerelease</c> and <c>build</c> may be <c>null</c> to be omitted, but may not be empty strings.</summary> 
    public SemVer(int major, int minor, int patch, string prerelease, string build)
    {
      Major = major;
      Minor = minor;
      Patch = patch;

      if (major < 0) throw new ArgumentException("Violation of spec item 8: Major version must be a non-negative integer.");
      if (minor < 0) throw new ArgumentException("Violation of spec item 7: Minor version must be a non-negative integer.");
      if (patch < 0) throw new ArgumentException("Violation of spec item 8: Patch must be a non-negative integer.");

      // Check PreRelease to make sure it's right
      if (prerelease != null)
      {
        if (PreReleaseRegex.IsMatch(prerelease)) PreRelease = prerelease;
        else throw new ArgumentException("SemVer spec item 9 violated: Pre-release string format");
      }

      // Check BuildMetadata to make sure it's right
      if (build != null)
      {
        if (BuildMetadataRegex.IsMatch(build)) BuildMetadata = build;
        else throw new ArgumentException("SemVer spec item 10 violated: Build metadata string format");
      }
    }

    /// Creates a <c>SemVer</c> from a formatted string.
    public SemVer(string version)
    {
      if (version == null) throw new ArgumentNullException("version");

      if (!FullRegex.IsMatch(version)) throw new ArgumentException(version + " is not a valid semantic version number.");

      Match mtc = FullRegex.Match(version);
      GroupCollection gc = mtc.Groups;

      Major = int.Parse(gc["major"].Value);
      Minor = int.Parse(gc["minor"].Value);
      Patch = int.Parse(gc["patch"].Value);
      PreRelease = gc["prerelease"]?.Value;
      BuildMetadata = gc["buildmetadata"]?.Value;
    }

    /// Returns the <c>string</c> representation of a <c>SemVer</c>.
    public override string ToString()
    {
      string ret = Major + "." + Minor + "." + Patch;
      if (PreRelease != null) ret += "-" + PreRelease;
      if (BuildMetadata != null) ret += "+" + BuildMetadata;
      return ret;
    }

    /// <summary>
    /// Compares two semantic versions according to spec rule 11.
    /// </summary>
    public int CompareTo(SemVer target)
    {
      // "Precedence is determined by the first difference when comparing
      // each of these identifiers from left to right as follows: Major,
      // minor, and patch versions are always compared numerically."
      int ret = Sequence.FirstNonZero(
        Major - target.Major,
        Minor - target.Minor,
        Patch - target.Patch);

      if (ret != 0) return ret;

      // "When major, minor, and patch are equal, a pre-release version
      // has lower precedence than a normal version."
      if (PreRelease == null && target.PreRelease != null) return 1;
      if (PreRelease != null && target.PreRelease == null) return -1;

      // "Precedence for two pre-release versions with the same major,
      // minor, and patch version MUST be determined by comparing each dot
      // separated identifier from left to right until a difference is
      // found as follows:"
      string[] leftPR = PreRelease.Split('.');
      string[] rightPR = target.PreRelease.Split('.');
      int length = Math.Min(leftPR.Length, rightPR.Length);

      for (int i = 0; i < length; i++)
      {
        string leftPRE = leftPR[i];
        string rightPRE = rightPR[i];

        bool leftIsNum = int.TryParse(leftPRE, out int leftPRN);
        bool rightIsNum = int.TryParse(rightPRE, out int rightPRN);
        // "identifiers consisting of only digits are compared numerically"
        if (leftIsNum && rightIsNum)
        {
          ret = leftPRN - rightPRN;
          if (ret != 0) return ret;
        }
        // "Numeric identifiers always have lower precedence than non-
        // numeric identifiers."
        else if (leftIsNum) return -1;
        else if (rightIsNum) return 1;
        // "identifiers with letters or hyphens are compared lexically in
        // ASCII sort order"
        else
        {
          ret = leftPRE.CompareTo(rightPRE);
          if (ret != 0) return ret;
        }
      }

      // "A larger set of pre-release fields has a higher precedence than
      // a smaller set, if all of the preceding identifiers are equal."
      if (leftPR.Length > length) return 1;
      if (rightPR.Length > length) return -1;

      return 0;
    }

    /// <summary>
    /// Checks whether two semantic versions are equal.
    /// 
    /// This check follows rule 11. Notably, this means that build
    /// metadata is ignored for the purposes of the check. Use
    /// <a cref="EqualsStrict(SemVer)">EqualsStrict</a> to check with metadata.
    /// </summary>
    ///
    /// <exception cref="InvalidCastException">
    /// If <c>obj</c> is not a <c>SemVer</c>.
    /// </exception>
    public override bool Equals(object obj)
    {
      SemVer target = (SemVer)obj;
      return
        Major == target.Major &&
        Minor == target.Minor &&
        Patch == target.Patch &&
        PreRelease == target.PreRelease;
    }

    /// <summary>
    /// Checks whether two semantic versions are equal.
    /// 
    /// This check does not follow rule 11. Instead, build metadata is
    /// part of the check. Use <a cref="Equals(object)">Equals</a> to
    /// check without metadata, according to rule 11.
    /// </summary>
    public bool EqualsStrict(SemVer target)
    {
      return
        Major == target.Major &&
        Minor == target.Minor &&
        Patch == target.Patch &&
        PreRelease == target.PreRelease &&
        BuildMetadata == target.BuildMetadata;
    }

    public override int GetHashCode()
    {
      return
        (Major & 0x4 << 28) |
        (Minor & 0x8 << 20) |
        (Patch & 0x4 << 16) |
        (((PreRelease?.GetHashCode()) ?? 0) & 0xF);
    }

    public static bool operator ==(SemVer left, SemVer right) => left.Equals(right);
    public static bool operator !=(SemVer left, SemVer right) => !(left.Equals(right));
    public static bool operator >(SemVer left, SemVer right) => left.CompareTo(right) > 0;
    public static bool operator <(SemVer left, SemVer right) => left.CompareTo(right) < 0;
    public static bool operator >=(SemVer left, SemVer right) => left.CompareTo(right) >= 0;
    public static bool operator <=(SemVer left, SemVer right) => left.CompareTo(right) <= 0;

    /// <summary>
    /// Creates a new copy of this SemVer with the specified major version number.
    /// </summary>
    public SemVer WithMajor(int maj) => new SemVer(maj, Minor, Patch, PreRelease, BuildMetadata);
    /// <summary>
    /// Creates a new copy of this SemVer with the specified minor version number.
    /// </summary>
    public SemVer WithMinor(int min) => new SemVer(Major, min, Patch, PreRelease, BuildMetadata);
    /// <summary>
    /// Creates a new copy of this SemVer with the specified patch number.
    /// </summary>
    public SemVer WithPatch(int pat) => new SemVer(Major, Minor, pat, PreRelease, BuildMetadata);
    /// <summary>
    /// Creates a new copy of this SemVer with the specified pre-release tag.
    ///
    /// Use <c>null</c> to omit the pre-release tag from the copy.
    /// </summary>
    public SemVer WithPreRelease(string pr) => new SemVer(Major, Minor, Patch, pr, BuildMetadata);
    /// <summary>
    /// Creates a new copy of this SemVer with the specified build metadata.
    ///
    /// Use <c>null</c> to omit the build metadata from the copy.
    /// </summary>
    public SemVer WithBuildMetadata(string build) => new SemVer(Major, Minor, Patch, PreRelease, build);

    /// <summary>Creates a new copy of this SemVer with no pre-release tag or build metadata.</summary>
    public SemVer Release() => new SemVer(Major, Minor, Patch);
    /// <summary>Creates a new copy of this SemVer representing the next patch after this one.</summary>
    public SemVer BumpPatch() => new SemVer(Major, Minor, Patch + 1);
    /// <summary>Creates a new copy of this SemVer representing the next minor version after this one.</summary>
    public SemVer BumpMinor() => new SemVer(Major, Minor + 1, 0);
    /// <summary>Creates a new copy of this SemVer representing the next major version after this one.</summary>
    public SemVer BumpMajor() => new SemVer(Major + 1, 0, 0);
  }
}