using System.Text.Json;
using Cysharp.Text;
using ZLogger;

namespace WebAPIServer.Util;

public static class LogManager
{
    public static void SetLogging(WebApplicationBuilder builder)
    {
        IConfiguration configuration = builder.Configuration;

        builder.Logging.ClearProviders();

        var fileDir = configuration["LogDir"];
        if (Directory.Exists(fileDir) == false)
        {
            Directory.CreateDirectory(fileDir);
        }


        builder.Logging.AddZLoggerRollingFile(
            (dt, x) => $"{fileDir}{dt.ToLocalTime():yyyy-MM-dd}_{x:000}.log",
            x => x.ToLocalTime().Date, 1024,
            options =>
            {
                options.EnableStructuredLogging = true;
            });

        builder.Logging.AddZLoggerConsole(options =>
        {
            options.EnableStructuredLogging = true;

            /*options.StructuredLoggingFormatter = (writer, info) =>
            {
                writer.WriteString("abc", "123");
                info.Timestamp.ToLocalTime();
            };
            */
            //var prefixFormat = ZString.PrepareUtf8<LogLevel, DateTime>("[{0}][{1}]");
            //options.PrefixFormatter = (writer, info) => prefixFormat.FormatTo(ref writer, info.LogLevel, info.Timestamp.DateTime.ToLocalTime());
            //options.PrefixFormatter = (writer, info) => ZString.Utf8Format(writer, "[{0}]", info.Timestamp.ToLocalTime().AddHours(9).DateTime);
            //options.PrefixFormatter = (writer, info) => ZString.Utf8Format(writer, "[{0}][{1}]", info.LogLevel, info.Timestamp.AddHours(9));
        });
    }

    public static EventId MakeEventId(ErrorCode errorCode)
    {
        return new EventId((int)errorCode, errorCode.ToString());
    }
}
