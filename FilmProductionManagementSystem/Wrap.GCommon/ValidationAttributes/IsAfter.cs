namespace Wrap.GCommon.ValidationAttributes;

using System.ComponentModel.DataAnnotations;

using static GCommon.OutputMessages;

public class IsAfter : ValidationAttribute
{
    private readonly string comparisonProperty;

    public IsAfter(string comparisonProperty)
    {
        this.comparisonProperty = comparisonProperty;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    { 
        object? comparisonValue = validationContext.ObjectType.GetProperty(comparisonProperty)?.GetValue(validationContext.ObjectInstance);

        if (value is DateTime dateTimeValue && comparisonValue is DateTime comparisonDateTime)
        {
            if (dateTimeValue <= comparisonDateTime)
            {
                return new ValidationResult(ErrorMessage ?? string.Format(IsAfterExceptionMessage, comparisonValue));
            }
        }
        
        return ValidationResult.Success;
    }
}