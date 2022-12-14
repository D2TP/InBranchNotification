using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swagger.Extension
{
    public interface ISwaggerOptionsBuilder
    {
        ISwaggerOptionsBuilder Enable(bool enabled);
        ISwaggerOptionsBuilder ReDocEnable(bool reDocEnabled);
        ISwaggerOptionsBuilder WithName(string name);
        ISwaggerOptionsBuilder WithTitle(string title);
        ISwaggerOptionsBuilder WithVersion(string version);
        ISwaggerOptionsBuilder WithRoutePrefix(string routePrefix);
        ISwaggerOptionsBuilder IncludeSecurity(bool includeSecurity);
        SwaggerOptions Build();
    }
}
