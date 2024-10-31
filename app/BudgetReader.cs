﻿using Newtonsoft.Json;

namespace Sibvic.ConsoleMoney
{
    public class BudgetReader : IBudgetReader
    {
        public Budget[]? ReadFromFile(string filename)
        {
            if (!File.Exists(filename))
            {
                return [];
            }
            var data = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<Budget[]?>(data) ?? [];
        }
    }
}
