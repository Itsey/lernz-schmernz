using Plisky.Diagnostics;
using Plisky.Diagnostics.Listeners;
using TestNLearn;

namespace TestNLearnApp;

internal class Program {
    protected static Bilge b = new Bilge("tnl-main");
    static void Main(string[] args) {
        Console.WriteLine("Hello, World!");

        var hnd = new TCPHandler("127.0.0.1", 9060, true);

        var fmt = new FlimFlamV4Formatter(); //C4M();
        hnd.SetFormatter(fmt);
        Bilge.AddHandler(hnd);
        Bilge.AddHandler(new FileSystemHandler(new FSHandlerOptions(@"C:\temp\log.txt") {
        }));

        b.ActiveTraceLevel = System.Diagnostics.SourceLevels.All;
        Bilge.Alert.Online("Trace Test");
        b.Info.Log("Hello");


        var b1 = new Bilge();
        Console.WriteLine($"Before any configuration resolver.  Default trace is:  {b1.ActiveTraceLevel}");

        Bilge.SetConfigurationResolver((a, b) => {
            return System.Diagnostics.SourceLevels.Verbose;
        });


        var b2 = new Bilge();
        Console.WriteLine($"After a configuration resolver is in effect.  New instance trace is: {b2.ActiveTraceLevel}");

       

        // Now some more complex examples
        Bilge.SetConfigurationResolver((initString, existingLevel) => {
            if (initString.Contains("customer1234")) {
                return System.Diagnostics.SourceLevels.Verbose;

            }
            return System.Diagnostics.SourceLevels.Off;
        });

        // Simple example
        var b3 = new Bilge("customer1234");
        var b4 = new Bilge("customer1235");

        Console.WriteLine($"Customer1234 {b3.ActiveTraceLevel} Customer1234 {b4.ActiveTraceLevel} ");

        b.Verbose.Log("Calling customer subsystem");

        // More complex but explains it better - if we use customer id in the config resolver we only trace for that customer

        for(int i=20; i<50; i++) {
            string customerId = $"customer{1200+i}";
            var f = new CustomerSubsystem(customerId);
            int res = f.AddNumbers(12, 13);
            if (res > 0) {
                b.Warning.Log($"{customerId} Result was greater than zero");
            }
        }
        


        for (int i = 0; i < 10; i++) {
            Console.WriteLine($"End World {i}");
            Thread.Sleep(100);
        }
        if (hnd.LastFault != null) {
            Console.WriteLine(hnd.LastFault.Message);
        }

    }
}
