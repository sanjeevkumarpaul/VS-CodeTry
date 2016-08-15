using System;
using System.Xml.Serialization;

using Utilities.Extensions;
using Utilities.WebClients;
using Utilities.Helpers;


namespace ConsoleApplication //Main Namespace.
{
    #region ^Start Program for the Application to RUN
    public class Program
    {
        public static void Main(string[] args)
        {
            //WriteSomething();
            //TryWeb();
            Mathamatics();
            SerializeAndDeserializeIt();
            //Console.ReadLine();
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

        public static void Mathamatics()
        {
            int a = new Random().Next(100);
            int b = new Random().Next(1000);

            Console.WriteLine($"(A){a} + (B){b} = ??=> {a+b}");
            Console.WriteLine($"(B){b} / (A){a} = ??=> {b/a}");
            Console.WriteLine($"(A){a} * (B){b} = ??=> {a*b}");
            Console.WriteLine("SAATVIK SANJEEV MAHUYA");
        }

        public static void SerializeAndDeserializeIt()
        {
            var cls = new ToBeSerialized( "First Property 1", 5000 );

            var serial = Serializer.Serialize( cls );

            Console.WriteLine(serial);

            cls = Serializer.DeSerialize<ToBeSerialized>(serial);
            Console.WriteLine(cls.ToString());
        }
    }
    #endregion ~END OF Start Program for the application to run.


    [XmlRoot(Namespace="http://myorg.com")]
    [XmlType(TypeName = "My-Own-Serialization")]
    public class ToBeSerialized //( string firstprop, int secondprof)
    {
        public ToBeSerialized() {}  //To be present in order to get serialized.
        
        public ToBeSerialized(string firstprop, int secondprop)
        {
            this.firstprop = firstprop;
            this.secondprop = secondprop;
        }

        [XmlElement(ElementName="First-Always-Prop")]
        public string firstprop {get; set;}
        [XmlIgnore]
        public int secondprop {get; set;}

        public override string ToString()
        {
            return $"{Environment.NewLine}First Property = {this.firstprop}";
        }
    }
    
}
