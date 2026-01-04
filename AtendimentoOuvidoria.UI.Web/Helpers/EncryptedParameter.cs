using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AtendimentoOuvidoria.UI.Web.Helpers
{
    public class EncryptedParameter(IDataProtectionProvider dpp) :
    IModelBinder,
    IOutboundParameterTransformer
    {
        private readonly IDataProtector protector
            = dpp.CreateProtector("EncryptedParameter");

        public string? TransformOutbound(object? value)
        {
            var result = value?.ToString();
            return string.IsNullOrEmpty(result)
                ? null
                : protector.Protect(result);
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var key = bindingContext.FieldName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(key);

            if (valueProviderResult.FirstValue is { } value)
            {
                var result = protector.Unprotect(value);
                bindingContext.Result = ModelBindingResult.Success(result);
            }

            return Task.CompletedTask;
        }
    }

}
