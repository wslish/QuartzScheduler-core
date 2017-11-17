using System;
using System.ComponentModel.DataAnnotations;
using QuartzScheduler.Infrastructure.Utils;

namespace QuartzScheduler.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ValidExpressionAttribute:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            bool isValid = JobUtil.IsValidCronExpression(value.ToString());
            return isValid
                ? ValidationResult.Success
                : new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
    }
}