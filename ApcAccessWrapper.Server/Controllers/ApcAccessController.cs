using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace ApcAccessWrapper.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApcAccessController : ControllerBase
    {
        private readonly ILogger<ApcAccessController> _logger;
        private readonly IApcAccessBinaryWrapper _binaryWrapper;

        private const char RawDataDelimeter = ':';

        public ApcAccessController(ILogger<ApcAccessController> logger, IApcAccessBinaryWrapper binaryWrapper)
        {
            _logger = logger;
            _binaryWrapper = binaryWrapper;
        }

        [HttpGet]
        public object Get() => _binaryWrapper.ReadRaw()
            .Split("\n", StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Split(RawDataDelimeter))
            .ToDictionary(key => key.First().Trim().Replace(' ', '_'), value => ParseValue(string.Join(RawDataDelimeter, value.Skip(1)).Trim()));

        //TODO: Do ALL the types properly in json output
        private object? ParseValue(string input) 
        {
            static bool IsSuffix(string x, params string[] candidates) 
                => candidates.Any(suffix => x.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
            static string RemoveSuffix(string x)
                => Regex.Replace(x, @"\D+$", string.Empty);

            input = input.Trim();

            if (string.IsNullOrEmpty(input))
                return null;
            else if (IsSuffix(input, "Percent", "Volts", "Watts") && float.TryParse(RemoveSuffix(input), out float fOutput))
                return fOutput;
            //else if (IsSuffix(input, "Seconds") && int.TryParse(RemoveSuffix(input), out int iOutput))
            //    return iOutput;

            return input;
        }
    }
}