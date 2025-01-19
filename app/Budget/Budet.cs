using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sibvic.ConsoleMoney.Budget
{
    public record Budget(string Name, string Id, double? DefaultPercent);
}
