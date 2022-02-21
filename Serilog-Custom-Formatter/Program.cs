using Serilog;
using Serilog.Templates;
using System;

namespace Serilog_Custom_Formatter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            JsonFormattingExample();
            //OutPutTemplateExample();
        }
        static void JsonFormattingExample()
        {
            Console.WriteLine("-----Logging Messages with out Custom formatting----");
            using var logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            logger.Information("Running {Example}", nameof(JsonFormattingExample));

            logger.ForContext<Program>()
                .Information("Cart contains {@Items}", new[] { "Tea", "Coffee" });

            logger.ForContext<Program>()
                .Warning("Cart is empty");

            Console.WriteLine("-----End of Logging Messages with out Custom formatting----");


            Console.WriteLine("----- Logging Messages with custom formatting-----");
            using var log = new LoggerConfiguration()
                .Enrich.WithProperty("Application", "SampleApplication")
                .WriteTo.Console(new ExpressionTemplate(
                     "{ {@t, @mt, @l: if @l='Information' then undefined() else @l, @x, ApplicationName: {Application}, ..rest()} }\n"))
                .CreateLogger();

            log.Information("Running {Example}", nameof(JsonFormattingExample));

            log.ForContext<Program>()
                .Information("Cart contains {@Items}", new[] { "Tea", "Coffee" });

            log.ForContext<Program>()
                .Warning("Cart is empty");
        }

        static void OutPutTemplateExample()
        {
            Console.WriteLine("----- Logging Messages with Pipeline Component formatting-----");

            using var log = new LoggerConfiguration()
                .Enrich.WithProperty("Application", "Example")
                .Enrich.WithComputed("FirstItem", "coalesce(Items[0], '<empty>')")
                .Enrich.WithComputed("SourceContext", "coalesce(Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1), '<no source>')")
                .Filter.ByIncludingOnly("Items is null or Items[?] like 'C%'")
                .WriteTo.Console(outputTemplate:
                    "{Timestamp:HH:mm:ss} {Level:u3} ({SourceContext}) {Message:lj} (first item is {FirstItem}){NewLine}{Exception}")
                .CreateLogger();

            log.Information("Running {Example}", nameof(OutPutTemplateExample));

            log.ForContext<Program>()
                .Information("Cart contains {@Items}", new[] { "Tea", "Coffee" });

            log.ForContext<Program>()
                .Information("Cart contains {@Items}", new[] { "Apricots","Apples" });
        }
    }
}
