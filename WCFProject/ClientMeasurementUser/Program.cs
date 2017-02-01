using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientMeasurementUser
{
    class Program
    {
        static void Main(string[] args)
        {
            var callback = new ClientCallback();
            //while (true)
            //{
            Console.WriteLine("Client started!");
            Console.WriteLine("Press any key to connect to server..");
            Console.ReadKey();
            string reply = callback.ConnectToServer();
            string[] str = new string[3];

            Console.WriteLine("Connected to server, client ID:{0}", reply);

            while (true)
            {
                Console.WriteLine("Subscribe to new measurement event insert: Subs");
                Console.WriteLine("Unsubscribe from measurement event insert: Unsubs");
                Console.WriteLine("Measurement reporting insert: report");
                Console.WriteLine("Disconnect from server insert: disc");
                reply = Console.ReadLine();

                if(reply.Equals("report"))
                {
                    Console.WriteLine("Get all measurements date to date insert: getM");
                    Console.WriteLine("Get all measuremets with limit(above, under, time) insert: limit");
                    Console.WriteLine("Get average measuremets from location insert: aver");
                    reply = Console.ReadLine();

                    if (reply.Equals("getM"))
                    {
                        Console.WriteLine("Insert location name or (all):");
                        str[0] = Console.ReadLine();
                        Console.WriteLine("Insert wanted measurements (temp, humid, both):");
                        str[1] = Console.ReadLine();
                        Console.WriteLine("Input from date dd-MM-yyyy:");
                        string fr = Console.ReadLine();
                        DateTime from;
                        from = DateTime.Parse(fr);
                        Console.WriteLine("Input to date dd-MM-yyyy:");
                        string to = Console.ReadLine();
                        DateTime todate = DateTime.Parse(to);
                        //call function
                        callback.GetAllMeasurementsDateToDate(from, todate, str);
                        
                    }
                    else if (reply.Equals("limit"))
                    {
                        Console.WriteLine("Insert limit:");
                        float limit = float.Parse(Console.ReadLine());
                        Console.WriteLine("Insert above Or under limit:");
                        str[0] = Console.ReadLine();
                        Console.WriteLine("Insert wanted measurements (temp, humid, both):");
                        str[1] = Console.ReadLine();
                        //call function
                        callback.GetAllMeasuremensWithLimit(limit, str);
                    }
                    else if (reply.Equals("aver"))
                    {
                        Console.WriteLine("Insert loocation name:");
                        string location = Console.ReadLine();
                        Console.WriteLine("Insert wanted measurements (temp, humid, both):");
                        str[0] = Console.ReadLine();
                        //call function
                        callback.GetAverageMeasurementsFromLocation(location, str);

                    }
                }
                else if (reply.Equals("disc"))
                {
                    Console.WriteLine(callback.DisconnectFromServer());
                    Console.WriteLine("Press any key to exit..");
                    Console.ReadKey();
                    callback.Dispose();
                    break;
                }
                else if (reply.Equals("Subs"))
                {
                    Console.WriteLine("Insert location name:");
                    string location = Console.ReadLine();
                    Console.WriteLine("Insert event (both, humid, temp):");
                    str[0] = Console.ReadLine();
                    reply = callback.SubscribeToMeasurement(location, callback.MyUuid, str);
                    Console.WriteLine(reply);
                }
                else if (reply.Equals("Unsubs"))
                {
                    Console.WriteLine("Insert location name:");
                    string location = Console.ReadLine();
                    Console.WriteLine("Insert event (both, humid, temp):");
                    str[0] = Console.ReadLine();
                    reply = callback.UnsubscribeToMeasurement(location, callback.MyUuid, str);
                    Console.WriteLine(reply);
                }
            }
        }
    }
}
