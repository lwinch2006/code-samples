using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApp.Mapping;

public class AntiXssBinder : IModelBinder
{
	private const string HtmlTagsRegExPattern = "<.*?>";

	public Task BindModelAsync(ModelBindingContext bindingContext)
	{
		if (bindingContext == null)
		{
			throw new ArgumentNullException(nameof(bindingContext));
		}

		var modelName = bindingContext.ModelName;

		var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
		if (valueProviderResult == ValueProviderResult.None)
		{
			return Task.CompletedTask;
		}

		bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

		var value = valueProviderResult.FirstValue;

		if (string.IsNullOrWhiteSpace(value))
		{
			bindingContext.Result = ModelBindingResult.Success(value);
			return Task.CompletedTask;
		}

		value = Regex.Replace(value, HtmlTagsRegExPattern, string.Empty);
		bindingContext.Result = ModelBindingResult.Success(value);
		return Task.CompletedTask;
	}
}