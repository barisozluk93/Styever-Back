using System.Text;

namespace UserManagement.Utils
{
    public static class ShopierFileLogger
    {
        private static readonly object Sync = new();
        public static string? LastSuccessfulPath { get; private set; }
        public static string? LastError { get; private set; }

        public static void Info(string message) => Write("INFO", message);
        public static void Warning(string message) => Write("WARN", message);

        public static void Error(string message, Exception? exception = null)
        {
            if (exception != null)
                message += Environment.NewLine + exception;

            Write("ERROR", message);
        }

        public static void WriteForm(IReadOnlyDictionary<string, string> form)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Shopier OSB alanlari:");

            foreach (var item in form.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
            {
                var value = IsSensitive(item.Key) ? "***MASKED***" : item.Value;
                builder.AppendLine($"  {item.Key} = {value}");
            }

            Info(builder.ToString().TrimEnd());
        }

        public static object Probe()
        {
            Info("SHOPIER LOGGER PROBE");
            return new
            {
                success = !string.IsNullOrWhiteSpace(LastSuccessfulPath),
                path = LastSuccessfulPath,
                error = LastError,
                baseDirectory = AppContext.BaseDirectory,
                tempPath = Path.GetTempPath(),
                processIdentity = Environment.UserName
            };
        }

        private static void Write(string level, string message)
        {
            var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {message}{Environment.NewLine}";

            lock (Sync)
            {
                LastError = null;

                foreach (var directory in GetCandidateDirectories())
                {
                    if (TryWrite(directory, line, out var path, out var error))
                    {
                        LastSuccessfulPath = path;
                        return;
                    }

                    LastError = error;
                }
            }
        }

        private static IEnumerable<string> GetCandidateDirectories()
        {
            var candidates = new[]
            {
                Path.Combine(Path.GetTempPath(), "StyeverLogs"),
                Path.Combine(Environment.GetEnvironmentVariable("TEMP") ?? string.Empty, "StyeverLogs"),
                Path.Combine(Environment.GetEnvironmentVariable("TMP") ?? string.Empty, "StyeverLogs"),
                @"C:\Windows\Temp\StyeverLogs",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Styever", "Logs"),
                Path.Combine(AppContext.BaseDirectory, "Logs"),
                @"C:\Temp\StyeverLogs"
            };

            return candidates
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase);
        }

        private static bool TryWrite(string directory, string line, out string? path, out string? error)
        {
            path = null;
            error = null;

            try
            {
                Directory.CreateDirectory(directory);
                path = Path.Combine(directory, $"shopier-{DateTime.Now:yyyy-MM-dd}.log");
                File.AppendAllText(path, line, Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                error = $"{directory}: {ex.GetType().Name}: {ex.Message}";
                path = null;
                return false;
            }
        }

        private static bool IsSensitive(string key)
        {
            var normalized = key.Replace("_", string.Empty).ToLowerInvariant();
            return normalized == "hash" ||
                   normalized == "res" ||
                   normalized.Contains("password") ||
                   normalized.Contains("pass") ||
                   normalized.Contains("username") ||
                   normalized.Contains("osbuser") ||
                   normalized.Contains("signature") ||
                   normalized.Contains("token") ||
                   normalized.Contains("authorization");
        }
    }
}
