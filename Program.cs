using System;
using Utilities.Extensions;
using Utilities.WebClients;


namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WriteSomething();
            TryWeb();

            Console.ReadLine();
        }

        public static void WriteSomething()
        {
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Start...");
            Console.WriteLine($@"This is the first time I am trying at this hour.
                                 ".AllUpper());
            Console.WriteLine(". END .");
        }

        public static async void TryWeb()
        {
            Console.WriteLine("Entering web call...");
            var response = await new WebCall().Call();
            Console.WriteLine("Web Response: ");
            Console.WriteLine(response);
        }
    }
}
