using System.ComponentModel.DataAnnotations;

namespace CampusConnect.Validation;

public sealed class FutureDateAttribute : ValidationAttribute
{
    public FutureDateAttribute()
    {
        ErrorMessage = "Event date must be in the future.";
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true;
        }

        if (value is DateTime dateTime)
        {
            return dateTime > DateTime.Now;
        }

        return false;
    }
}
