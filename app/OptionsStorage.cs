using Newtonsoft.Json;

namespace Sibvic.ConsoleMoney
{
    class OptionsStorage
    {
        public static string GetHomeDir(string optionsPath)
        {
            var optionsFilePath = Path.Combine(optionsPath, "options.json");
            if (!File.Exists(optionsFilePath))
            {
                return ".";
            }
            var optionsText = File.ReadAllText(optionsFilePath);
            var options = JsonConvert.DeserializeObject<Options>(optionsText);
            if (options == null)
            {
                return ".";
            }
            return options.RootPath;
        }
    }
}
