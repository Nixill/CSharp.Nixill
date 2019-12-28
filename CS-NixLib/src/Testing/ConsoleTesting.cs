using System;
using System.Collections.Generic;

namespace Nixill.Testing {
  /// <summary>
  /// A helper class to create console-app tests for various applications.
  /// </summary>
  public class ConsoleTestRunner {
    Dictionary<string, Action<ConsoleTestRunner>> Tests = new Dictionary<string, Action<ConsoleTestRunner>>();
    bool Cancelled = false;

    /// <summary>
    /// Adds a new test method to the runner's list.
    ///
    /// If the new test shares a name with an existing test, the existing
    /// test is overwritten.
    /// </summary>
    /// <param name="keyword">The name / keyword to use to begin the
    /// test.</param>
    /// <param name="test">The method which comprises the test.</param>
    public void AddTest(string keyword, Action<ConsoleTestRunner> test) {
      Tests[keyword] = test;
    }

    /// <summary>
    /// Runs the tests. This should be the end of the <c>Main</c> method
    /// in the testing class, unless cleanup is needed afterwards.
    /// </summary>
    public void RunTests() {
      Console.WriteLine("NixLib ConsoleTestRunner v0.1.0");
      Console.WriteLine();

      // List available tests
      Console.WriteLine("Available tests:");
      foreach (string name in Tests.Keys) {
        Console.WriteLine("- " + name);
      }
      Console.WriteLine();

      // Handle the Ctrl-C event
      Console.CancelKeyPress += new ConsoleCancelEventHandler(SetCancelled);

      try {
        while (true) {
          // Query which test to run.
          Console.Write("Enter desired test name, or Ctrl-C to quit: ");
          string name = Console.ReadLine();
          if (Cancelled) ThrowCancelException();

          // Make sure a test actually exists
          if (Tests.ContainsKey(name)) {
            Console.WriteLine();
            Console.WriteLine("Beginning test " + name + ".");
            Console.WriteLine();
            // Run the test
            try {
              Tests[name].Invoke(this);
            }
            catch (ConsoleCancelException) {
              // do nothing
            }
            Console.WriteLine();
            Console.WriteLine("End of test " + name + ".");
          }
          else {
            Console.WriteLine("Test " + name + " does not exist.");
          }
          Console.WriteLine();
        }
      }
      catch (ConsoleCancelException) {
        // do nothing
      }

      Console.WriteLine();
      Console.WriteLine("Test runner shutting down.");

      Console.CancelKeyPress -= new ConsoleCancelEventHandler(SetCancelled);
    }

    private void SetCancelled(object sender, ConsoleCancelEventArgs args) {
      Cancelled = true;
      args.Cancel = true;
      Console.Write("\nPress enter to continue.");
    }

    private void ThrowCancelException() {
      Cancelled = false;
      throw new ConsoleCancelException();
    }

    /// <summary>
    /// Use this method within tests to get string values.
    ///
    /// This method should be used over <c>Console.ReadLine()</c> because
    /// it checks for cancels (ctrl-c followed by enter), and can end the
    /// test when the cancel key combination is pressed.
    /// </summary>
    /// <param name="name">The name of the value to get.</param>
    public string GetStringValue(string name) {
      Console.Write("Enter value for string " + name + ": ");
      string ret = Console.ReadLine();
      if (Cancelled) ThrowCancelException();
      return ret;
    }

    /// <summary>
    /// Use this method within tests to get long values.
    ///
    /// This method should be used over <c>Console.ReadLine()</c> because
    /// it checks for cancels (ctrl-c followed by enter), and can end the
    /// test when the cancel key combination is pressed. It also makes
    /// sure a parseable number is entered, looping until a valid value is
    /// entered or the cancel combination is pressed.
    /// </summary>
    /// <param name="name">The name of the value to get.</param>
    public long GetLongValue(string name) {
      Console.Write("Enter value for long " + name + ": ");
      string val;
      long ret;
      bool first = true;
      do {
        if (first) first = false;
        else Console.Write("Not a valid value, try again: ");
        val = Console.ReadLine();
        if (Cancelled) ThrowCancelException();
      } while (!long.TryParse(val, out ret));
      return ret;
    }

    /// <summary>
    /// Use this method within tests to get double values.
    ///
    /// This method should be used over <c>Console.ReadLine()</c> because
    /// it checks for cancels (ctrl-c followed by enter), and can end the
    /// test when the cancel key combination is pressed. It also makes
    /// sure a parseable number is entered, looping until a valid value is
    /// entered or the cancel combination is pressed.
    /// </summary>
    /// <param name="name">The name of the value to get.</param>
    public double GetDoubleValue(string name) {
      Console.Write("Enter value for double " + name + ": ");
      string val;
      double ret;
      bool first = true;
      do {
        if (first) first = false;
        else Console.Write("Not a valid value, try again: ");
        val = Console.ReadLine();
        if (Cancelled) ThrowCancelException();
      } while (!double.TryParse(val, out ret));
      return ret;
    }
  }

  internal class ConsoleCancelException : Exception {
  }
}