using CSharpFunctionalExtensions;
using Shouldly;
using Xunit;

namespace TestNLearn.Test;

public class ValidatedResultTests
{
    [Fact]
    public void Ok_CreatesSuccessfulValidatedResult()
    {
        var result = ValidatedResult<string>.Ok("Hello World");

        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe("Hello World");
    }

    [Fact]
    public void Fail_CreatesFailedValidatedResult()
    {
        var result = ValidatedResult<string>.Fail("Something went wrong");

        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Something went wrong");
    }

    [Fact]
    public void From_WrapsExistingResultCorrectly()
    {
        Result<int> innerOk = Result.Success(42);
        ValidatedResult<int> validatedOk = ValidatedResult<int>.From(innerOk);

        validatedOk.IsSuccess.ShouldBeTrue();
        validatedOk.Value.ShouldBe(42);

        Result<int> innerFail = Result.Failure<int>("Error code 500");
        ValidatedResult<int> validatedFail = ValidatedResult<int>.From(innerFail);

        validatedFail.IsSuccess.ShouldBeFalse();
        validatedFail.Error.ShouldBe("Error code 500");
    }

    [Fact]
    public void ImplicitConversions_WorkBidirectionally()
    {
        Result<string> inner = Result.Success("Implicit Test");
        ValidatedResult<string> validated = inner; // Implicit Result -> ValidatedResult

        validated.IsSuccess.ShouldBeTrue();
        validated.Value.ShouldBe("Implicit Test");

        Result<string> convertedBack = validated; // Implicit ValidatedResult -> Result
        convertedBack.IsSuccess.ShouldBeTrue();
        convertedBack.Value.ShouldBe("Implicit Test");
    }

    [Fact]
    public void CompilerNullabilityInference_Demonstration()
    {
        ValidatedResult<string> r = ValidatedResult<string>.Ok("Test Value");

        if (r.IsSuccess)
        {
            // Compiler infers r.Value is non-null here due to [MemberNotNullWhen(true, nameof(Value))]
            int length = r.Value.Length;
            length.ShouldBe(10);
        }
    }
}
