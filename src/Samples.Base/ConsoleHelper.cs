using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Base
{
    public class ConsoleHelper
    {
        public virtual void Separator()
        {
            Console.WriteLine(new string('-', 100));
        }

        public virtual void WriteError(Exception ex, string additionalText = "")
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            if (!string.IsNullOrWhiteSpace(additionalText))
                Console.Write($"{additionalText}: ");
            Console.WriteLine(ex.Message);
            Console.ForegroundColor = color;
        }
    }
}
