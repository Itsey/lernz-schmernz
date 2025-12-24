using Plisky.Diagnostics;

namespace TestNLearn.Test;


public class Module1Tests {
    protected Bilge b = new("tnl-module1-test");


    [Fact]
    public void Test1() {
        var sut = new Module1();
    }
}
