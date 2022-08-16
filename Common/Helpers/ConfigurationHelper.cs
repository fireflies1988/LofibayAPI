using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public static class ConfigurationHelper
    {
        public static IConfiguration? Configuration;
        public static void Initialize(IConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}
