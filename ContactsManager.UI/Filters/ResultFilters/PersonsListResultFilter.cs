﻿using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ResultFilters
{
    public class PersonsListResultFilter : IAsyncResultFilter
    {
        private readonly ILogger<PersonsListResultFilter> _logger;

        public PersonsListResultFilter(ILogger<PersonsListResultFilter> logger) { _logger = logger; }
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName} - before", nameof(PersonsListResultFilter),nameof(PersonsListResultFilter));
            context.HttpContext.Response.Headers["Last-Modified"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            await next();
            _logger.LogInformation("{FilterName}.{MethodName} - after", nameof(PersonsListResultFilter), nameof(PersonsListResultFilter));
            

        }
    }
}
