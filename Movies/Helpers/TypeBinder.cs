using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Movies.Helpers;

public class TypeBinder<T> : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var propertyName = bindingContext.ModelName;
        var providerValues = bindingContext.ValueProvider.GetValue(propertyName);
        if (providerValues ==  ValueProviderResult.None) return Task.CompletedTask;

        try
        {
            var value = JsonConvert.DeserializeObject<T>(providerValues.FirstValue);
            bindingContext.Result = ModelBindingResult.Success(value);
        }
        catch
        {
            bindingContext.ModelState.TryAddModelError(propertyName, "Invalid value for List<T>");
        }

        return Task.CompletedTask;
    }
}