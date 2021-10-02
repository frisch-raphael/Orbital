using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;
using Shared.Config;

namespace Orbital.Attributes
{
    public class RestrictFileExtensions : ValidationAttribute
    {
        //private List<string> AllowedExtensions { get; set; }

        public RestrictFileExtensions()
        {
            //AllowedExtensions = extensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!FileUploadConfig.kSupportedExtensions.Contains(extension))
                {
                    return new ValidationResult(GetErrorMessage(extension));
                }

            }

            return ValidationResult.Success;
        }
        public string GetErrorMessage(string extension)
        {
            return $"{extension} is not supported";
        }
    }
}
