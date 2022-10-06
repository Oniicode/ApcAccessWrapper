using System.Diagnostics;

namespace ApcAccessWrapper
{
    public interface IApcAccessBinaryWrapper
    {
        public string ReadRaw();
    }

    public class ApcAccessBinaryWrapper : IApcAccessBinaryWrapper
    {
        private readonly IConfiguration _configuration;
        private string Binary => _configuration["Binary"] ?? "/sbin/apcaccess";
        private string Host => _configuration["Host"] ?? "127.0.0.1";

        public ApcAccessBinaryWrapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ReadRaw()
        {
            var process = Process.Start(new ProcessStartInfo { 
                UseShellExecute = false,
                FileName = Binary,
                ArgumentList = { "-h", Host },
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });
            if (process == null)
                throw new Exception("Process null.");

            // Ensure 0 exit code
            process.WaitForExit();
            if (process.ExitCode != 0)
                throw new Exception($"{Binary} exited with code {process.ExitCode}: {process.StandardError.ReadToEnd}");

            return process.StandardOutput.ReadToEnd();
        }
    }
}
