using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace dka.net5.codesamples.web.Utils
{
    public static class ViewModelValidationExtensions
    {
        public static void ValidateModel(this Controller controller, object viewModel, string propertyPerfix)
        {
            if (viewModel == null)
            {
                return;
            }

            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(viewModel, null, null);
            Validator.TryValidateObject(viewModel, ctx, validationResults, true);

            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    controller.ModelState.AddModelError($"{propertyPerfix}{memberName}", validationResult.ErrorMessage);
                }
            }            
        }
    }
}