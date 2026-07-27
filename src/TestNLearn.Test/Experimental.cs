using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Plisky.Diagnostics;
using Plisky.Plumbing;


namespace TestNLearn.Test;

public class Experimental {
    public Bilge b = new Bilge("experimental-tests");



    [Fact]
    public void Guard_clause_implementation_test1() {
        b.Info.Flow();

        object o = new object();
        object o1 = new object();
        object o2 = new object();
        object o3 = new object();
        object on = null;

        //Guard(on);
        Guard(0, on);
        Guard(o, o1, o2);  // This works
        Guard(o, 01, "fred");  // This does not

    }


    public void Guard([NotNull] object subj1) {

        if (subj1 is null) {
            throw new ArgumentNullException(nameof(subj1));
        }


    }


#if false
    public void Guard([NotNull] object subj1, [NotNull] object subj2) {

        if (subj1 is null) {
            throw new ArgumentNullException(nameof(subj1));
        }

        if (subj2 is null) {
            throw new ArgumentNullException(nameof(subj1));
        }

              
    }


#else
    public void Guard([NotNull] object subj1, [NotNull] object subj2, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0) {

        if (subj1 is null) {
            throw new ArgumentNullException(nameof(subj1), $"{callerName} @ {callerLineNumber}");
        }

        if (subj2 is null) {
            throw new ArgumentNullException(nameof(subj2), $"{callerName} @ {callerLineNumber}");
        }


    }

#endif

    public void Guard([NotNull] object subj1, [NotNull] object subj2, [NotNull] object subj3) {
        //  public void Guard([NotNull]object subj1,[NotNull]object subj2,[NotNull]object subj3,[NotNull]object subj4 
        if (subj1 is null) {
            throw new ArgumentNullException(nameof(subj1));
        }

        if (subj2 is null) {
            throw new ArgumentNullException(nameof(subj2));
        }

        if (subj3 is null) {
            throw new ArgumentNullException(nameof(subj3));
        }
        var caller = InternalUtil.GetCallingStackFrame("<calling method name>");
        throw new ArgumentNullException(nameof(subj1), $"{caller.Item1}::{caller.Item2}");
    }

    public void Guard([NotNull] object subj1, [NotNull] object subj2, [NotNull] object subj3, [NotNull] object subj4) {
        if (subj1 is null) {
            throw new ArgumentNullException(nameof(subj1));
        }

        var caller = InternalUtil.GetCallingStackFrame("<calling method name>");
        throw new ArgumentNullException(nameof(subj1), $"{caller.Item1}::{caller.Item2}");
    }


}
