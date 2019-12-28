using Nixill.Testing;
using System;

namespace Nixill.Test {
  public class NixLibConsoleTest {
    static void Main(string[] args) {
      ConsoleTestRunner runner = new ConsoleTestRunner();
      runner.AddTest("echo", EchoTest);
      runner.RunTests();
    }

    static void EchoTest(ConsoleTestRunner runner) {
      while (true) {
        string test = runner.GetStringValue("test");
        Console.WriteLine("You wrote: " + test);
        Console.WriteLine();
      }
    }
  }
}