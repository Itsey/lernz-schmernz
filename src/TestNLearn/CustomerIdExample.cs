using Plisky.Diagnostics;

namespace TestNLearn;

public class CustomerSubsystem {
    protected Bilge b;
    protected string cid;
    public bool mockSuspiciousActivity = true;

    public CustomerSubsystem(string customerId) {
        cid = customerId;
        b = new Bilge(customerId);
    }

    public int AddNumbers(int one, int two) {
        b.Info.Flow();

        b.Info.Log($"Customer {cid} checking for sus activity.");
        if (mockSuspiciousActivity) {
            b.Warning.Log("Suspicious activity flag for customer, need to call out to central system");

            MakeAdditionalCallBecauseOfFlag();
        }
        b.Info.Log($"returning for customer.");
        return one + two;
    }

    private void MakeAdditionalCallBecauseOfFlag() {
        b.Info.Flow();
    }
}
