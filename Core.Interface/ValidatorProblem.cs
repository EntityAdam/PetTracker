using System.Diagnostics.CodeAnalysis;

namespace Core.Interface;

[ExcludeFromCodeCoverage]
public record ValidatorProblem(bool IsValid, string Reason);
