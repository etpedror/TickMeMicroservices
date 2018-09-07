using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TickMeHelpers;

namespace TestMe
{
    public class Program
    {
        static void Main(string[] args)
        {
            var program = new Program();

            var task = program.RunAsync();
            task.Wait();
            Console.WriteLine("Press enter to quit");
            Console.ReadKey();
        }

        async Task RunAsync()
        {
            
        }
    }
}
