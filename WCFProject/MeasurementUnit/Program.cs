using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace MeasurementUnit
{
    class Program
    {
      //static WCF_TempHumidService.MeasurementUnitServiceClient client;

        static void Main(string[] args)
        {
            var callback = new MeasurementUnitCallback(1000, 6000);
            List<string> locations = callback.GetAllLocationsFromService();

            Console.WriteLine("Locations from service!!!");
            foreach (var item in locations)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("Insert location..");
            string str =  Console.ReadLine();

            callback.Subscribe(str);

            Console.WriteLine("Press any key to unsubscribe..");
            Console.ReadKey();
            callback.Unsubscribe();
            Console.WriteLine("Press any key to exit..");
            Console.ReadKey();
            callback.Dispose();
        }
    }
}
