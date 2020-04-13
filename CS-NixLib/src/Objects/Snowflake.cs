using System;
using System.Threading;
using Nixill.Utils;
using NodaTime;

namespace Nixill.Objects {
  /// <summary>
  /// Represents a
  /// <a href="https://github.com/twitter-archive/snowflake/tree/snowflake-2010">Snowflake</a>.
  /// </summary>
  /// <remarks>
  /// The Snowflakes are not program-specific, but do follow the format
  /// originally created by Twitter and documented at that link.
  /// </remarks>
  public class Snowflake : IComparable<Snowflake>, IComparable<long>, IComparable<Instant> {
    // The default epoch is Y2K. However, this can be overridden by the program using the library for convenience.
    // Libraries that use this class should not override this value without clearly documenting it.
    /// <summary>
    /// The epoch that is used for new Snowflakes when an Epoch isn't
    /// specified in the constructor. This is what time snowflake 0
    /// represents.
    /// </summary>
    /// <remarks>
    /// On initialization, this field is set to 2000-01-01 at 00:00:00
    /// UTC.
    ///
    /// Changing this value doesn't affect Snowflakes already
    /// generated.
    ///
    /// This field is designed to be configurable by the final program, so
    /// that a programmer can set it once and forget about it. Libraries
    /// should not set the value without a very good reason, and all such
    /// cases should be well documented.
    /// </remarks> 
    public static Instant DefaultEpoch = Instant.FromUtc(2000, 1, 1, 0, 0);

    // A couple epochs from well-known services that are absolutely NOT affiliated with me.
    /// <summary>
    /// The epoch used by Discord, 2015-01-01 at 00:00:00.000 UTC.
    /// </summary>
    /// <remarks>
    /// This value is documented at
    /// <a href="https://discordapp.com/developers/docs/reference#snowflakes">Discord's
    /// developer documentation portal</a>.
    /// 
    /// I'm not affiliated with Discord except insofaras I have an
    /// account and sometimes make bots with that account.
    /// </remarks>
    public static readonly Instant DiscordEpoch = Instant.FromUtc(2015, 1, 1, 0, 0);
    /// <summary>
    /// The epoch used by Twitter, 2010-11-04 at 01:42:54.657 UTC.
    /// </summary>
    /// <remarks>
    /// This value is documented at
    /// <a href="https://github.com/twitter-archive/snowflake/blob/snowflake-2010/src/main/scala/com/twitter/service/snowflake/IdWorker.scala#L25">Line
    /// 25 of IdWorker</a> from the original Snowflake code.
    ///
    /// I'm not affiliated with Twitter except insofaras I have an
    /// account.
    /// </remarks>
    public static readonly Instant TwitterEpoch = Instant.FromUtc(2010, 11, 4, 1, 42, 54) + Duration.FromMilliseconds(657); //2010-11-04 at 01:42:54.657 UTC

    /// <summary>
    /// The epoch from which this Snowflake is counted.
    /// </summary>
    public readonly Instant Epoch;

    /// <summary>
    /// The actual numeric ID of the snowflake.
    /// </summary>
    public readonly long ID;

    /// <summary>
    /// The absolute time represented by this Snowflake.
    /// </summary>
    public Instant Time => Epoch + Duration.FromMilliseconds(Milliseconds);

    /// <summary>
    /// The delta time represented by this Snowflake (as a Duration).
    /// </summary>
    public Duration TimeFromEpoch => Duration.FromMilliseconds(Milliseconds);

    /// <summary>
    /// The delta time represented by this Snowflake (as a long).
    /// </sumary>
    public long Milliseconds => (ID >> 22) & 0x1FF_FFFF_FFFF;

    /// <summary>
    /// The worker that generated this snowflake.
    /// </summary>
    /// <remarks>
    /// What I call a "Worker", the original specification called a
    /// "configured machine id".
    /// </remarks>
    public int Worker => (int)((ID >> 12) & 0x3FF);

    /// <summary>
    /// Where this Snowflake fell in the sequence of up to 4096 by a
    /// single worker in a single millisecond.
    /// </summary>
    public int Sequence => (int)(ID & 0xFFF);

    /// <summary>
    /// Constructs a new Snowflake from its constituent parts.
    /// </summary>
    /// <param name="time">The instant in time the Snowflake should
    /// represent. If unspecified, defaults to actual time of
    /// generation.</param>
    /// <param name="worker">The configured machine id of the Snowflake.
    /// If unspecified, defaults to 0.</param>
    /// <param name="sequence">The sequence number of the Snowflake. If
    /// unspecified, defaults to 0.</param>
    /// <param name="epoch">The epoch the Snowflake is counting from. If
    /// unspecified, defaults to
    /// <a cref="Snowflake.DefaultEpoch">Snowflake.DefaultEpoch</a>.</param>
    public Snowflake(Instant? time = null, int worker = 0, int sequence = 0, Instant? epoch = null) {
      // Set default values
      Instant vTime = time ?? SystemClock.Instance.GetCurrentInstant();
      int vWorker = worker & 0x3FF;
      int vSequence = sequence & 0xFFF;

      Epoch = epoch ?? DefaultEpoch;

      // Figure out the time value of this epoch
      Duration epochOffset = vTime - Epoch;
      long ticks = epochOffset.ToInt64Nanoseconds();
      ticks /= 1_000_000; // nanos to millis
      if (ticks > 2_199_023_255_551 || ticks < 0)
        throw new ArgumentOutOfRangeException("time", "Time must be after; but within 25,451 days (about 69 years 7 months), 15 hours, 47 minutes, 35.551 seconds of; the epoch.");

      // CS0675 shouldn't apply because I've already &ed above
#pragma warning disable CS0675
      ID = (ticks << 22) | (vWorker << 12) | (vSequence);
#pragma warning restore CS0675
    }

    /// <summary>
    /// Constructs a new Snowflake from a calculated ID.
    /// </summary>
    /// <param name="id">The numeric ID of the Snowflake.</param>
    /// <param name="epoch">The epoch the Snowflake is counting from. If
    /// unspecified, defaults to
    /// <a cref="Snowflake.DefaultEpoch">Snowflake.DefaultEpoch</a>.</param>
    public Snowflake(long id = 0, Instant? epoch = null) {
      if (id < 0) throw new ArgumentOutOfRangeException("id", "ID cannot be negative.");

      Epoch = epoch ?? DefaultEpoch;
      ID = id;
    }

    // Comparisons! :D
    /// <summary>
    /// Checks if this Snowflake is equal to another.
    /// </summary>
    /// <remarks>
    /// If <c>other</c> isn't a <c>Snowflake</c>, this method
    /// automatically returns <c>false</c>.
    /// </remarks>
    /// <param name="other">The other object to compare to.</param>
    public sealed override bool Equals(object other) {
      if (!(other is Snowflake that)) return false;

      return ID == that.ID && Epoch == that.Epoch;
    }

    /// <summary>
    /// Gets the hash code of this Snowflake.
    /// </summary>
    public sealed override int GetHashCode() {
      int hash = (int)(ID & 0xFFFFFFFF);
      hash ^= (int)((ID >> 32) & 0xFFFFFFFF);
      hash ^= Epoch.GetHashCode();

      return hash;
    }

    /// <summary>
    /// Checks how this Snowflake compares to another.
    /// </summary>
    /// <remarks>
    /// Snowflakes are compared by absolute time first. Two Snowflakes
    /// with equal absolute times are compared by the numeric ID.
    /// </remarks>
    public int CompareTo(Snowflake other) {
      return CompareUtils.FirstNonZero(
        CompareTo(other.Time),
        ID.CompareTo(other.ID)
      );
    }

    /// <summary>
    /// Checks how this Snowflake's ID compares to another long.
    /// </summary>
    /// <remarks>
    /// Only the numeric ID is compared; the epoch is ignored.
    /// </remarks>
    public int CompareTo(long other) {
      return ID.CompareTo(other);
    }

    /// <summary>
    /// Checks how this Snowflake's absolute time compares to another
    /// Instant.
    /// </summary>
    /// <remarks>
    /// Only absolute times are compared; the worker and sequence numbers
    /// are ignored.
    /// </summary>
    public int CompareTo(Instant other) {
      return ((IComparable<Instant>)Time).CompareTo(other);
    }

    #region // property adjustments
    /// <summary>
    /// Returns this Snowflake but with the Worker changed.
    /// </summary>
    public Snowflake WithWorker(int worker) => new Snowflake(Time, worker, Sequence, Epoch);
    /// <summary>
    /// Returns this Snowflake but with the Sequence changed.
    /// </summary>
    public Snowflake WithSequence(int sequence) => new Snowflake(Time, Worker, sequence, Epoch);
    /// <summary>
    /// Returns this Snowflake but with the absolute time changed.
    /// </summary>
    public Snowflake WithTime(Instant time) => new Snowflake(time, Worker, Sequence, Epoch);
    /// <summary>
    /// Returns this Snowflake but with the epoch changed.
    /// </summary>
    public Snowflake WithEpoch(Instant epoch) => new Snowflake(Time, Worker, Sequence, epoch);
    /// <summary>
    /// Returns this Snowflake but with the epoch-offset changed.
    /// </summary>
    public Snowflake WithDuration(Duration dur) => new Snowflake(Epoch + dur, Worker, Sequence, Epoch);
    /// <summary>
    /// Returns this Snowflake but with the epoch-offset changed.
    /// </summary>
    public Snowflake WithMilliseconds(long millis) => new Snowflake(Epoch + Duration.FromMilliseconds(millis), Worker, Sequence, Epoch);
    /// <summary>
    /// Returns this Snowflake but with the ID (everything but the epoch)
    /// changed.
    /// </summary>
    public Snowflake SameEpoch(long id) => new Snowflake(id, Epoch);
    #endregion

    #region // conversions
    public static explicit operator Snowflake(long input) => new Snowflake(input);
    public static explicit operator Snowflake(Instant input) => new Snowflake(input);
    public static explicit operator Snowflake(Duration input) => new Snowflake(DefaultEpoch + input);
    public static explicit operator long(Snowflake input) => input.ID;
    public static explicit operator Instant(Snowflake input) => input.Time;
    public static explicit operator Duration(Snowflake input) => input.TimeFromEpoch;
    #endregion

    #region // arithmetic operators
    public static Snowflake operator -(Snowflake left, Duration right) => new Snowflake(left.Time - right, left.Worker, left.Sequence, left.Epoch);
    public static Snowflake operator +(Snowflake left, Duration right) => new Snowflake(left.Time + right, left.Worker, left.Sequence, left.Epoch);
    public static Snowflake operator +(Duration left, Snowflake right) => new Snowflake(right.Time + left, right.Worker, right.Sequence, right.Epoch);
    public static Duration operator -(Snowflake left, Snowflake right) => left.Time - right.Time;
    #endregion

    #region // comparison operators
    // and now all the comparison operator overrides :D
    public static bool operator <(Snowflake left, Snowflake right) => left.CompareTo(right) < 0;
    public static bool operator >(Snowflake left, Snowflake right) => left.CompareTo(right) > 0;
    public static bool operator <=(Snowflake left, Snowflake right) => left.CompareTo(right) <= 0;
    public static bool operator >=(Snowflake left, Snowflake right) => left.CompareTo(right) >= 0;
    public static bool operator ==(Snowflake left, Snowflake right) => left.CompareTo(right) == 0;
    public static bool operator !=(Snowflake left, Snowflake right) => left.CompareTo(right) != 0;

    public static bool operator <(Snowflake left, long right) => left.CompareTo(right) < 0;
    public static bool operator >(Snowflake left, long right) => left.CompareTo(right) > 0;
    public static bool operator <=(Snowflake left, long right) => left.CompareTo(right) <= 0;
    public static bool operator >=(Snowflake left, long right) => left.CompareTo(right) >= 0;
    public static bool operator ==(Snowflake left, long right) => left.CompareTo(right) == 0;
    public static bool operator !=(Snowflake left, long right) => left.CompareTo(right) != 0;

    public static bool operator <(long left, Snowflake right) => right.CompareTo(left) > 0;
    public static bool operator >(long left, Snowflake right) => right.CompareTo(left) < 0;
    public static bool operator <=(long left, Snowflake right) => right.CompareTo(left) >= 0;
    public static bool operator >=(long left, Snowflake right) => right.CompareTo(left) <= 0;
    public static bool operator ==(long left, Snowflake right) => right.CompareTo(left) == 0;
    public static bool operator !=(long left, Snowflake right) => right.CompareTo(left) != 0;

    public static bool operator <(Snowflake left, Instant right) => left.CompareTo(right) < 0;
    public static bool operator >(Snowflake left, Instant right) => left.CompareTo(right) > 0;
    public static bool operator <=(Snowflake left, Instant right) => left.CompareTo(right) <= 0;
    public static bool operator >=(Snowflake left, Instant right) => left.CompareTo(right) >= 0;
    public static bool operator ==(Snowflake left, Instant right) => left.CompareTo(right) == 0;
    public static bool operator !=(Snowflake left, Instant right) => left.CompareTo(right) != 0;

    public static bool operator <(Instant left, Snowflake right) => right.CompareTo(left) > 0;
    public static bool operator >(Instant left, Snowflake right) => right.CompareTo(left) < 0;
    public static bool operator <=(Instant left, Snowflake right) => right.CompareTo(left) >= 0;
    public static bool operator >=(Instant left, Snowflake right) => right.CompareTo(left) <= 0;
    public static bool operator ==(Instant left, Snowflake right) => right.CompareTo(left) == 0;
    public static bool operator !=(Instant left, Snowflake right) => right.CompareTo(left) != 0;
    #endregion
  }

  /// <summary>
  /// A class that makes Snowflakes following the same rules as the
  /// original specification.
  /// </summary>
  /// <remarks>
  /// This class isn't thread safe. Multiple threads wishing to
  /// independently generate Snowflakes should each use their own
  /// SnowflakeFactory.
  /// </remarks>
  public class SnowflakeFactory {
    /// <summary>
    /// The worker, or configured machine id, of the factory.
    /// </summary>
    public int Worker { get; private set; }
    /// <summary>
    /// The sequence number of the next Snowflake to be generated.
    /// </summary>
    public int Sequence { get; private set; }
    /// <summary>
    /// The last Instant a snowflake was generated.
    /// </summary>
    public Instant LastGen { get; private set; }
    /// <summary>
    /// The epoch to use for Snowflakes generated by this factory.
    /// </summary>
    public Instant Epoch { get; private set; }

    public SnowflakeFactory() : this(0, Snowflake.DefaultEpoch) { }
    public SnowflakeFactory(int worker) : this(worker, Snowflake.DefaultEpoch) { }
    public SnowflakeFactory(Instant epoch) : this(0, epoch) { }

    /// <summary>
    /// Creates a new SnowflakeFactory with a specified Worker and Epoch.
    /// </summary>
    /// <param name="worker">The Worker to use. If omitted, defaults to
    /// 0.</param>
    /// <param name="epoch">The Epoch to use. If omitted, defaults to
    /// <a cref="Snowflake.DefaultEpoch">Snowflake.DefaultEpoch</a>.
    /// Changing Snowflake.DefaultEpoch later will not affect this factory
    /// even in such a circumstance.</param>
    public SnowflakeFactory(int worker, Instant epoch) {
      Worker = worker;
      Sequence = 0;
      LastGen = SystemClock.Instance.GetCurrentInstant();
    }

    private static Instant TruncateMilliseconds(Instant when) {
      Duration offset = when - Snowflake.DefaultEpoch;
      long nanos = offset.ToInt64Nanoseconds();
      long nanosOfMillis = nanos % 1_000_000;
      return when - Duration.FromNanoseconds(nanosOfMillis);
    }

    /// <summary>
    /// Generates a new Snowflake with this factory.
    /// </summary>
    /// <param name="ifFull">What to do if 4,096 Snowflakes have already
    /// been generated during the current millisecond.</param>
    public Snowflake Generate(SnowflakeGenOption ifFull = SnowflakeGenOption.Hang) {
      Instant now = TruncateMilliseconds(SystemClock.Instance.GetCurrentInstant());

      if (now > LastGen) {
        LastGen = now;
        Sequence = 0;
      }

      if (Sequence >= 4096) {
        switch (ifFull) {
          case SnowflakeGenOption.Null:
            return null;
          case SnowflakeGenOption.Error:
            throw new SnowflakeGenerationException("SnowflakeFactory " + Worker + " overloaded with Error option specified.");
          case SnowflakeGenOption.Wrap:
            Sequence = 0;
            return new Snowflake(now, Worker, Sequence++, Epoch);
          case SnowflakeGenOption.NextWorker:
            return new Snowflake(now, Worker + (Sequence / 4096), Sequence++ % 4096, Epoch);
          case SnowflakeGenOption.Hang:
            Instant msPlusPlus = now + Duration.FromMilliseconds(1);
            LastGen = msPlusPlus;
            Sequence = 0;
            Thread.Sleep((msPlusPlus - SystemClock.Instance.GetCurrentInstant()).ToTimeSpan());
            return new Snowflake(msPlusPlus, Worker, Sequence++, Epoch);
        }
      }

      return new Snowflake(now, Worker, Sequence++, Epoch);
    }

    /// <summary>
    /// Generates an array of Snowflakes.
    /// </summary>
    /// <remarks>
    /// Unlike
    /// <a cref="SnowflakeFactory.Generate(SnowflakeGenOption)">Generate()</a>,
    /// this method can only error when Snowflakes overflow within a
    /// millisecond.
    /// </remarks>
    public Snowflake[] BulkGenerate(int count) {
      if (count <= 0) throw new ArgumentOutOfRangeException("count", "Must be at least 1.");
      if (count + Sequence > 4096) throw new ArgumentOutOfRangeException("count", "Cannot generate more than 4096 Snowflakes in one millisecond.");

      Snowflake[] ret = new Snowflake[count];

      Instant now = TruncateMilliseconds(SystemClock.Instance.GetCurrentInstant());

      if (now > LastGen) {
        LastGen = now;
        Sequence = 0;
      }

      for (int i = 0; i < count; i++) {
        ret[i] = new Snowflake(now, Worker, Sequence++, Epoch);
      }

      return ret;
    }
  }

  /// <summary>
  /// How to handle Snowflake generation when attempting to generate more
  /// than 4,096 Snowflakes within a single millisecond.
  /// </summary>
  public enum SnowflakeGenOption {
    /// <summary>Return null.</summary>
    Null,
    /// <summary>Throw a SnowflakeGenerationException.</summary>
    Error,
    /// <summary>Reset sequence to 0.</summary>
    Wrap,
    /// <summary>Continue with the next worker.</summary>
    NextWorker,
    /// <summary>Wait until the next millisecond.</summary>
    Hang
  }

  /// <summary>
  /// Thrown when a program attempts to generate more than 4,096
  /// Snowflakes within a single millisecond.
  /// </summary>
  [System.Serializable]
  public class SnowflakeGenerationException : System.Exception {
    public SnowflakeGenerationException() { }
    public SnowflakeGenerationException(string message) : base(message) { }
    public SnowflakeGenerationException(string message, System.Exception inner) : base(message, inner) { }
    protected SnowflakeGenerationException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
  }
}