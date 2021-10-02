using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Shared.Config;

namespace Orbital.Attributes
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        public MaxFileSizeAttribute()
        {
        }

        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                if (file.Length > FileUploadConfig.kMaxFileSize)
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"Maximum allowed file size is {FileUploadConfig.kMaxFileSize / (1024 * 1024)} MB.";
        }
    }
}
