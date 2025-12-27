using Plisky.Diagnostics;

namespace TestNLearnApp;

public class C4M : FlimFlamV4Formatter {
    bool hasSet = false;
    public C4M() :base() {

    }

    protected override string DefaultConvertWithReference(MessageMetadata msg, string uniquenessReference) {
        if (!hasSet) {
            if (Thread.CurrentThread.Name == "Bilge>>RouterQueueDispatcher") {
                hasSet = true;
                Thread.CurrentThread.IsBackground = false;
            }
        }
        return base.DefaultConvertWithReference(msg, uniquenessReference);
    }
    protected override string ActualConvert(MessageMetadata msg) {
       
        return base.ActualConvert(msg);
    }
}
