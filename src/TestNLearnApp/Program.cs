using System.Diagnostics;
using Plisky.Diagnostics;
using Plisky.Diagnostics.Listeners;

namespace TestNLearnApp;

internal class Program {
    static void Main(string[] args) {

        Bilge.SetConfigurationResolver((a, b) => {
            return SourceLevels.Verbose;
        });
        var b = new Bilge("testnlearn-main", tl: SourceLevels.Verbose);

        var thnd = new TCPHandler("127.0.0.1", 9060);
        thnd.SetFormatter(new FlimFlamV4Formatter());


        var thnd2 = new InMemoryHandler();
        thnd2.SetFormatter(new FlimFlamV4Formatter());

        Bilge.AddHandler(thnd);
        Bilge.AddHandler(thnd2);

        for (int i = 0; i < 10; i++) {
            Thread.Sleep(100);
            b.Info.Log("Starting Test - Timings");
        }

        if (thnd2.GetMessageCount() < 9) {
            throw new Exception("Did not receive expected messages in in-memory handler");
        }



    }
}
