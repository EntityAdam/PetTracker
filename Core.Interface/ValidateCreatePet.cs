namespace Core.Interface;

public class ValidateCreatePet<NewPet> : Validator<NewPet>
{
    public override bool IsValid(NewPet type)
    {
        return !Validate(type).Any();
    }

    public override IEnumerable<ValidatorProblem> Validate(NewPet type)
    {
        return Enumerable.Empty<ValidatorProblem>();
    }
}