using System;
using System.Text.RegularExpressions;

namespace Nixill.Utils
{
  public class SemVer
  {
    public readonly int Major;
    public readonly int Minor;
    public readonly int Patch;
    public readonly string PreRelease = null;
    public readonly string BuildMetadata = null;

    private static readonly Regex PreReleaseRegex = new Regex("^(?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\\.(?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*))*$");
    private static readonly Regex BuildMetadataRegex = new Regex("^[0-9a-zA-Z-]+(?:\\.[0-9a-zA-Z-]+)*$");
    private static readonly Regex FullRegex = new Regex("^(?P<major>0|[1-9]\\d*)\\.(?P<minor>0|[1-9]\\d*)\\.(?P<patch>0|[1-9]\\d*)(?:-(?P<prerelease>(?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\\.(?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\\+(?P<buildmetadata>[0-9a-zA-Z-]+(?:\\.[0-9a-zA-Z-]+)*))?$");

    public SemVer(int major, int minor, int patch) : this(major, minor, patch, null, null) { }
    public SemVer(int major, int minor, int patch, string prerelease, string build)
    {
      Major = major;
      Minor = minor;
      Patch = patch;

      // Check PreRelease to make sure it's right
      if (prerelease != null)
      {
        if (PreReleaseRegex.IsMatch(prerelease))
        {
          PreRelease = prerelease;
        }
        else
        {
          throw new ArgumentException("SemVer spec item 9 violated: Pre-release string format");
        }
      }

      // Check BuildMetadata to make sure it's right
      if (build != null)
      {
        if (BuildMetadataRegex.IsMatch(build))
        {
          BuildMetadata = build;
        }
        else
        {
          throw new ArgumentException("SemVer spec item 10 violated: Build metadata string format");
        }
      }
    }

    public SemVer(string version)
    {
      if (version == null)
      {
        throw new ArgumentNullException("version");
      }

      if (!FullRegex.IsMatch(version))
      {
        throw new ArgumentException(version + " is not a valid semantic version number.");
      }

      Match mtc = FullRegex.Match(version);
      GroupCollection gc = mtc.Groups;

      Major = (int)gc["major"];
      Minor = (int)gc["minor"];
      Patch = (int)gc["patch"];
      PreRelease = gc["prerelease"];
      BuildMetadata = gc["buildmetadata"];
    }
  }
}