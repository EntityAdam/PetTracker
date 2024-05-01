using System.Diagnostics.CodeAnalysis;

namespace Core.Interface;

[ExcludeFromCodeCoverage]
public abstract class Validator<T>
{
    public abstract bool IsValid(T type);
    public abstract IEnumerable<ValidatorProblem> Validate(T type);
}
