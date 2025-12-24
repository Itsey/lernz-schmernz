using Plisky.Diagnostics;
using Plisky.Diagnostics.Listeners;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: Xunit.TestFramework("Plisky.Diagnostics.Test.XunitAutoTraceFixture", "TestNLearn.Test")]

namespace TestNLearn.Test;


public class XunitAutoTraceFixture : XunitTestFramework {

    public XunitAutoTraceFixture(IMessageSink messageSink)
        : base(messageSink) {

        bool trace = true;
        if (trace) {
            var t = new TCPHandler(new TCPHandlerOptions("127.0.0.1", 9060, true));
            t.SetFormatter(new FlimFlamV4Formatter());
            Bilge.AddHandler(t, HandlerAddOptions.SingleType);
            Bilge.SetConfigurationResolver((a, b) => System.Diagnostics.SourceLevels.Verbose);
            //Bilge.SetConfigurationResolver("v-*-d6");
            Bilge.Alert.Online("testing-online");
            Bilge.Default.Info.Log("Diagnostic fixture activating trace");
        }
    }

    public new void Dispose() {
        base.Dispose();
    }
}