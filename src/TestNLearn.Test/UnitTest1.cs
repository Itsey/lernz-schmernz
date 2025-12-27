using Plisky.Diagnostics;

namespace TestNLearn.Test;


public class Module1Tests {
    protected Bilge b = new("tnl-module1-test");


    [Fact]
    public void Test1() {
        b.Info.Flow();
        var sut = new Module1();
    }
}
