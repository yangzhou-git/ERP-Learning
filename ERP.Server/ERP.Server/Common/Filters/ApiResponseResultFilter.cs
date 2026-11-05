using ERP.Server.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ERP.Server.Common.Filters
{
    public class ApiResponseResultFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if(context.Result is ObjectResult objectResult)
            {
                if(objectResult.Value is IApiResponse apiResponse)
                {
                    objectResult.StatusCode = apiResponse.Code;
                }
            }
            await next();
        }
    }
}
