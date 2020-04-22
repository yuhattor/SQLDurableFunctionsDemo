using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

[assembly: FunctionsStartup(typeof(DurableSQLFunction.Startup))]

namespace DurableSQLFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<SqlConnection>((s) =>
            {
                var log = s.GetService<ILoggerFactory>().CreateLogger("Startup");
                log.LogInformation("スタート");
                string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
                var conn = new SqlConnection(connectionString);
                conn.Open();
                return conn;
            });
        }
    }
}
