using Plisky.Diagnostics;
using Plisky.Diagnostics.Listeners;

namespace TestNLearnApp;
internal class Program {
    protected static Bilge b = new Bilge("tnl-main");
    static void Main(string[] args) {
        Console.WriteLine("Hello, World!");

        Bilge.SetConfigurationResolver((a, b) => {
            return System.Diagnostics.SourceLevels.Verbose;
        });

        var hnd = new TCPHandler("127.0.0.1", 9060, true);

        var fmt = new FlimFlamV4Formatter(); //C4M();
        hnd.SetFormatter(fmt);
  
        b.AddHandler(hnd);
        b.ActiveTraceLevel = System.Diagnostics.SourceLevels.Verbose;

        Bilge.Alert.Online("Trace Test");
        b.Info.Log("Hello");


        for (int i = 0; i < 10; i++) {
            Console.WriteLine($"End World {i}");
         
        }
        if (hnd.LastFault != null) {
            Console.WriteLine(hnd.LastFault.Message);
        }

    }
}
