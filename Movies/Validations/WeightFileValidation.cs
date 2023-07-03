using System.ComponentModel.DataAnnotations;

namespace Movies.Validations;

public class WeightFileValidation : ValidationAttribute
{
    private readonly int _maxFileWeightOnMegabyte;

    public WeightFileValidation(int maxFileWeightOnMegabyte)
    {
        _maxFileWeightOnMegabyte = maxFileWeightOnMegabyte;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }
        IFormFile formFile = value as IFormFile;
        if(formFile == null) return ValidationResult.Success;

        if (formFile.Length > _maxFileWeightOnMegabyte * 1024 * 1024) return new ValidationResult($"The file can't exceed {_maxFileWeightOnMegabyte}MB");
        
        return ValidationResult.Success;
    }
}