using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;

namespace TestNLearn;

/// <summary>
/// A nullability-annotated wrapper around <see cref="Result{T}"/> that allows the compiler
/// to infer that <see cref="Value"/> is non-null when <see cref="IsSuccess"/> is true.
/// </summary>
/// <typeparam name="T">The type of the underlying result value.</typeparam>
public readonly struct ValidatedResult<T>
{
    private readonly Result<T> _inner;

    /// <summary>
    /// Initializes a new instance of <see cref="ValidatedResult{T}"/> wrapping a <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="result">The inner result instance to wrap.</param>
    public ValidatedResult(Result<T> result)
    {
        _inner = result;
    }

    /// <summary>
    /// Creates a <see cref="ValidatedResult{T}"/> from an existing <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="result">The inner result to wrap.</param>
    /// <returns>A new <see cref="ValidatedResult{T}"/> instance.</returns>
    public static ValidatedResult<T> From(Result<T> result) => new(result);

    /// <summary>
    /// Indicates whether the result represents a success.
    /// When true, <see cref="Value"/> is guaranteed to be non-null.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSuccess => _inner.IsSuccess;

    /// <summary>
    /// Indicates whether the result represents a failure.
    /// </summary>
    public bool IsFailure => _inner.IsFailure;

    /// <summary>
    /// Gets the value associated with a successful result, or null.
    /// </summary>
    public T? Value => _inner.Value;

    /// <summary>
    /// Gets the error message associated with a failed result.
    /// </summary>
    public string Error => _inner.Error;

    /// <summary>
    /// Creates a successful <see cref="ValidatedResult{T}"/> containing the specified value.
    /// </summary>
    /// <param name="value">The value for the successful result.</param>
    /// <returns>A successful <see cref="ValidatedResult{T}"/>.</returns>
    public static ValidatedResult<T> Ok(T value) => new(Result.Success(value));

    /// <summary>
    /// Creates a failed <see cref="ValidatedResult{T}"/> containing the specified error message.
    /// </summary>
    /// <param name="error">The error message for the failed result.</param>
    /// <returns>A failed <see cref="ValidatedResult{T}"/>.</returns>
    public static ValidatedResult<T> Fail(string error) => new(Result.Failure<T>(error));

    /// <summary>
    /// Implicitly converts a <see cref="Result{T}"/> into a <see cref="ValidatedResult{T}"/>.
    /// </summary>
    public static implicit operator ValidatedResult<T>(Result<T> result) => From(result);

    /// <summary>
    /// Implicitly converts a <see cref="ValidatedResult{T}"/> back to a <see cref="Result{T}"/>.
    /// </summary>
    public static implicit operator Result<T>(ValidatedResult<T> validatedResult) => validatedResult._inner;
}
