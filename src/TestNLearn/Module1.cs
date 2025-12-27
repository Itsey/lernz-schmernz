using Plisky.Diagnostics;

namespace TestNLearn;

public class Module1 {
    protected Bilge b = new("tnl-module1");


    public int AddNumbers(int one, int two) {
        b.Info.Flow();
        return one + two;
    }
}
