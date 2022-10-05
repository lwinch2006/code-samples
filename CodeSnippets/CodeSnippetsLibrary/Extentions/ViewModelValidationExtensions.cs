using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace CodeSnippetsLibrary.Extentions;

public static class ViewModelValidationExtensions
{
	public static void ValidateModel(this Controller controller, object viewModel, string propertyPrefix)
	{
		if (viewModel == null)
		{
			controller.ModelState.AddModelError(nameof(viewModel), "Model is null");
			return;
		}

		var validationResults = new List<ValidationResult>();
		var ctx = new ValidationContext(viewModel, null, null);
		Validator.TryValidateObject(viewModel, ctx, validationResults, true);

		foreach (var validationResult in validationResults)
		{
			foreach (var memberName in validationResult.MemberNames)
			{
				controller.ModelState.AddModelError($"{propertyPrefix}{memberName}", validationResult.ErrorMessage);
			}
		}
	}
}