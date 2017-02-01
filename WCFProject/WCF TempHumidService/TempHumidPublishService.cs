using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data.Entity;
using System.Data.EntityModel;
using System.Transactions;
using System.Threading;

namespace WCF_TempHumidService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    public class TempHumidPublishService : IMeasurementUnitService, IDisposable, IMeasurementClientService
    {
        private static Dictionary<string, IPublisherCallback> m_MeasurementPublishers;
        private static Dictionary<string, Dictionary<string, IMeasurementUserCallback>> m_TemperatureCallbacks;
        private static Dictionary<string, Dictionary<string, IMeasurementUserCallback>> m_HumidityCallbacks;
        private static db1Entities1 m_MyDbContext;
        //private static List<MyHumidityMeasurement> hm;
       // private static List<MyTemperatureMeasurement> tm;
        private List<string> m_Locations;

        public List<string> MyLocations
        {
            get { return m_Locations; }
            set { m_Locations = value; }
        }

        public db1Entities1 MyDbContext
        {
            get { return m_MyDbContext; }
            set { m_MyDbContext = value; }
        }
        

        public TempHumidPublishService()
        {
            m_MeasurementPublishers = new Dictionary<string, IPublisherCallback>();
            m_HumidityCallbacks = new Dictionary<string, Dictionary<string, IMeasurementUserCallback>>();
            m_TemperatureCallbacks = new Dictionary<string, Dictionary<string, IMeasurementUserCallback>>();
            //hm = new List<MyHumidityMeasurement>();
            //tm = new List<MyTemperatureMeasurement>();
            MyDbContext = new db1Entities1();

            //m_MyDbContext.

            List<TemperatureMeasurement> list = m_MyDbContext.TemperatureMeasurements.ToList();
            List<HumidityMeasurement> list1 = m_MyDbContext.HumidityMeasurements.ToList();
            List<Location> list3 = m_MyDbContext.Locations.ToList();
            List<RMUnit> list2 = m_MyDbContext.RMUnits.ToList();
            

            MyLocations = GetLocations();
            Action<string> insert = (locationName) =>
            {
                m_MeasurementPublishers.Add(locationName, null);
            };

            MyLocations.ForEach(insert);

        }

        public void SendNewTemperatureMeasurement(MyTemperatureMeasurement measurement)
        {
            //var my = (MyMeasurement)measurement;
            IPublisherCallback callback = m_MeasurementPublishers[measurement.MyLocationName];

            if (callback != null)
            {
                AddNewTemperatureMeasuremenToDb(measurement);
                callback.OnTemperatureMeasurementSent(measurement);
            }

            foreach (var item in m_TemperatureCallbacks[measurement.MyLocationName].Values)
            {
                WorkerClass work = new WorkerClass();
                work.MyTemperature = measurement;
                work.MyCallback = item;
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolSendTemperatureCallback), work);
            }
        }

        public void SendNewHumidityMeasurement(MyHumidityMeasurement measurement)
        {
            //string str = "Received";

            IPublisherCallback callback = m_MeasurementPublishers[measurement.MyLocationName];

            if (callback != null)
            {
                callback.OnHumidityMeasurementSent(measurement);
                AddNewHumidityMeasuremenToDb(measurement);
            }

            foreach (var item in m_HumidityCallbacks[measurement.MyLocationName].Values)
            {
                WorkerClass work = new WorkerClass();
                work.MyHumidity = measurement;
                work.MyCallback = item;
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolSendHumidityCallback), work);
            }
        }

        public void ThreadPoolSendHumidityCallback(object x)
        {
            var worker = (WorkerClass)x;
            worker.MyCallback.onNewHumidityMeasurementReceived(worker.MyHumidity);
        }

        public void ThreadPoolSendTemperatureCallback(object x)
        {
            var worker = (WorkerClass)x;
            worker.MyCallback.onNewTemperatureMeasurementReceived(worker.MyTemperature);
        }

        public void Subscribe(string location)
        {

            string message = "";
            string uuid = "";
            RMUnit unit = null;

            IPublisherCallback publisher = OperationContext.Current.GetCallbackChannel<IPublisherCallback>();
            if (m_MeasurementPublishers.Keys.Contains(location))
            {
                IPublisherCallback publishers = m_MeasurementPublishers[location];
                 
                if (publishers == null)
                {
                    m_MeasurementPublishers[location] = publisher;
                    message = "Measurement unit initialised";
                    unit = GetRMUnitForLocation(location);
                    m_TemperatureCallbacks[location] = new Dictionary<string, IMeasurementUserCallback>();
                    m_HumidityCallbacks[location] = new Dictionary<string, IMeasurementUserCallback>();
                    if (unit == null)
                    {
                        unit = GenerateNewRMUnit();
                        AddNewRMUnitToDBEntities(location, unit);
                    }

                    uuid = unit.Name;
                }
                else
                {
                    message = "Publisher allready running";
                }
            }
            else
            {
                message = "Invalid location";
            }

            MyRMUnit myUnit = FromDbEntityToMyClass(unit);
            publisher.OnSubcribe(myUnit, message);
        }

        public List<string> GetLocations()
        {
            List<string> locationNames = new List<string>();

            List<Location> locations = MyDbContext.Locations.ToList();
                           
            Action<Location> insert = (locationName) =>
            {
                locationNames.Add(locationName.Name);
            };

            locations.ForEach(insert);

            return locationNames;
        }

        public List<string> GetAllLocations()
        {
           // List<string> locations = GetLocations();
            return MyLocations;
        }

        public RMUnit GetRMUnitForLocation(string location)
        {
            Location dbLocation = MyDbContext.Locations.ToList().FirstOrDefault(r => r.Name.Equals(location));
            return dbLocation.RMUnit;
        }

        public RMUnit GenerateNewRMUnit()
        {
            RMUnit rmu = new RMUnit();
            rmu.Name = UuIdGenerator();

            return rmu;
        }

        public string UuIdGenerator()
        {
            char[] array = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'L', 'K', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'Y', 'Q', 'X', 'V', 'W', 'U' };
            string str = "";
            Random rnd = new Random();

            for (int i = 0; i < 2; i++)
            {
                str += array[rnd.Next(array.Length)];
            }
            str += "-";
            str += rnd.Next(1000).ToString();

            return str;

        }

        public void AddNewRMUnitToDBEntities(string location, RMUnit unit)
        {

            Location dbLocation = m_MyDbContext.Locations.ToList().FirstOrDefault(r => r.Name.Equals(location));

            if (dbLocation == null)
                return;

            unit.LocationName = dbLocation.Name;
            MyDbContext.RMUnits.Add(unit);

            try
            {
                m_MyDbContext.SaveChanges();
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException dbEx)
            {

                Exception raisel = dbEx.InnerException;

                while (raisel.InnerException != null)
                {
                    raisel = raisel.InnerException;
                    continue;
                }
            }
               
            
        }


        public void Unsubscribe(string uuid)
        {
            IPublisherCallback callback = OperationContext.Current.GetCallbackChannel<IPublisherCallback>();
            string message = "";
            RMUnit rmu;

            rmu = MyDbContext.RMUnits.ToList().FirstOrDefault(r => r.Name.Equals(uuid));

            if (rmu == null)
                message = "Your unsubscriber UUId now valid!!";
            else
            {
                m_MeasurementPublishers.Remove(rmu.LocationName);
                m_MeasurementPublishers[rmu.LocationName] = null;
                //m_MyDbContext.SaveChanges();
                message = "Unsubscribed!";
            }

            callback.OnUnsubcribe(message);

        }

        public MyRMUnit FromDbEntityToMyClass(RMUnit dbUnit)
        {
            MyRMUnit myUnit;

            if (dbUnit == null)
                myUnit = null;
            else
            {
                myUnit = new MyRMUnit();
                myUnit.MyName = dbUnit.Name;
                myUnit.MyLocationName = dbUnit.LocationName;
            }

            return myUnit;
        }

        public void AddNewHumidityMeasuremenToDb(MyHumidityMeasurement measurement)
        {
           // hm.Add(measurement);
            //RMUnit rmu = m_MyDbContext.RMUnits.ToList().FirstOrDefault(r => r.LocationName.Equals(measurement.MyLocationName));
            HumidityMeasurement m = new HumidityMeasurement();
            m.Value = measurement.MyValue;
            m.LocationName = measurement.MyLocationName;
            m.DateTime = measurement.MyDateTime;
            MyDbContext.HumidityMeasurements.Add(m);

            if (MyDbContext.Entry(m).State == System.Data.EntityState.Added)
            {
                try
                {
                    MyDbContext.SaveChanges();
                }
                catch (InvalidOperationException ex)
                {
                    MyDbContext.HumidityMeasurements.Remove(m);
                    MyDbContext.HumidityMeasurements.Add(m);
                    //MyDbContext.SaveChanges();
                }
                
            }
            
        }

        public void AddNewTemperatureMeasuremenToDb(MyTemperatureMeasurement measurement)
        {
            //tm.Add(measurement);
            TemperatureMeasurement m = new TemperatureMeasurement();
            m.Value = measurement.MyValue;
            m.LocationName = measurement.MyLocationName;
            m.DateTime = measurement.MyDateTime;
            MyDbContext.TemperatureMeasurements.Add(m);

            if (MyDbContext.Entry(m).State == System.Data.EntityState.Added)
            {
                MyDbContext.SaveChanges();
            }                
        }

        public void Dispose()
        {
            MyDbContext.SaveChanges();
            MyDbContext.Dispose();
            m_MeasurementPublishers.Clear();
            m_TemperatureCallbacks.Clear();
            m_HumidityCallbacks.Clear();
        }

        public string ConnnectToServer()
        {
            SaveToDB();
            return UuIdGenerator();  
        }

        public MyMeasurements GetAllMeasurementsFromLocationDateToDate(DateTime from, DateTime todate, params string[] pars)
        {
            MyMeasurements m = new MyMeasurements();
            List<TemperatureMeasurement> tlist = new List<TemperatureMeasurement>();
            List<HumidityMeasurement> hlist = new List<HumidityMeasurement>();
            string location = pars[0];
            string measurement = pars[1];

            if (pars[0].Equals("all"))
            {

                hlist = MyDbContext.HumidityMeasurements.ToList();
                tlist = MyDbContext.TemperatureMeasurements.ToList();
                
                
            }
            else
            {

                hlist = MyDbContext.HumidityMeasurements.ToList().Where(r => r.LocationName.Equals(location)).ToList();
                tlist = MyDbContext.TemperatureMeasurements.ToList().Where(r => r.LocationName.Equals(location)).ToList();
                
            }

            if (pars[1].Equals("humid") || pars[1].Equals("both"))
            {
                hlist = hlist.Where(r => r.DateTime > from).ToList();
                hlist = hlist.Where(r => r.DateTime < todate).ToList();

                Action<HumidityMeasurement> toSendList = (item) =>
                                            {
                                                var item2 = new MyHumidityMeasurement();
                                                item2.MyDateTime = item.DateTime;
                                                item2.MyRMUnitName = item.RMUnit.Name;
                                                item2.MyLocationName = item.LocationName;
                                                item2.MyValue = (float)item.Value;
                                                m.MyHumMeasurements.Add(item2);
                                            };

                hlist.ForEach(toSendList);

            }

            if (pars[1].Equals("temp") || pars[1].Equals("both"))
            {
                tlist = tlist.Where(r => r.DateTime > from).ToList();
                tlist = tlist.Where(r => r.DateTime < todate).ToList();

                Action<TemperatureMeasurement> toSendList = (item) =>
                                            {
                                                var item2 = new MyTemperatureMeasurement();
                                                item2.MyDateTime = item.DateTime;
                                                item2.MyRMUnitName = item.RMUnit.Name;
                                                item2.MyLocationName = item.LocationName;
                                                item2.MyValue = (float)item.Value;
                                                m.MyTempMeasurements.Add(item2);
                                            };

                tlist.ForEach(toSendList);
            }

            return m;

        }

        public MyMeasurements GetAllMeasuremensWithLimit(float limit, params string[] pars)
        {
            MyMeasurements m = new MyMeasurements();
            List<TemperatureMeasurement> tlist = new List<TemperatureMeasurement>();
            List<HumidityMeasurement> hlist = new List<HumidityMeasurement>();
           
            if(pars[1].Equals("humid") || pars[1].Equals("both"))
            {
                if (pars[0].Equals("above"))
                {
                    hlist = MyDbContext.HumidityMeasurements.ToList().Where(r => r.Value > limit).ToList();
                       
                }
                else if (pars[0].Equals("under"))
                {
                    hlist = MyDbContext.HumidityMeasurements.ToList().Where(r => r.Value < limit).ToList();
                }
               

                Action<HumidityMeasurement> toSendList = (item) =>
                                        {
                                            var item2 = new MyHumidityMeasurement();
                                            item2.MyDateTime = item.DateTime;
                                            item2.MyRMUnitName = item.RMUnit.Name;
                                            item2.MyValue = (float)item.Value;
                                            item2.MyLocationName = item.LocationName;
                                            m.MyHumMeasurements.Add(item2);
                                        };

                hlist.ForEach(toSendList);


            }
            if (pars[1].Equals("temp") || pars[1].Equals("both"))
            {
                if (pars[0].Equals("above"))
                {
                    tlist = MyDbContext.TemperatureMeasurements.ToList().Where(r => r.Value > limit).ToList();

                }
                else if (pars[0].Equals("under"))
                {
                    tlist = MyDbContext.TemperatureMeasurements.ToList().Where(r => r.Value < limit).ToList();
                }

                Action<TemperatureMeasurement> toSendList = (item) =>
                                            {
                                                var item2 = new MyTemperatureMeasurement();
                                                item2.MyDateTime = item.DateTime;
                                                item2.MyRMUnitName = item.RMUnit.Name;
                                                item2.MyLocationName = item.LocationName;
                                                item2.MyValue = (float)item.Value;
                                                m.MyTempMeasurements.Add(item2);
                                            };

                tlist.ForEach(toSendList);
            }
                
            
            return m;
        }

        public MyMeasurements GetAverageMeasurementsFromLocation(string location, params string[] pars)
        {
            MyMeasurements m = new MyMeasurements();

            if (m_MeasurementPublishers.ContainsKey(location))
            {
                if (pars[0].Equals("temp") || pars[0].Equals("both"))
                {
                    List<TemperatureMeasurement> ms = MyDbContext.TemperatureMeasurements.ToList();
                    double sum = 0.0f;

                    Action<TemperatureMeasurement> addToSum = (measurement) =>
                                                 {
                                                     sum += measurement.Value;
                                                 };
                    ms.ForEach(addToSum);
                    m.MyAverageTemperature = (float)sum / (float)ms.Count;
                }

                if (pars[0].Equals("humid") || pars[0].Equals("both"))
                {
                    List<HumidityMeasurement> ms = MyDbContext.HumidityMeasurements.ToList();
                    double sum = 0.0f;

                    Action<HumidityMeasurement> addToSum = (measurement) =>
                    {
                        sum += measurement.Value;
                    };
                    ms.ForEach(addToSum);
                    m.MyAverageHumidity = (float)sum / (float)ms.Count;
                }
            }
            return m;
        }


        public string DisconnectFromServer(string uuid)
        {
            foreach (var item in m_HumidityCallbacks.Values.ToList())
            {
                if (item.Keys.Contains(uuid))
                {
                    item.Remove(uuid);
                }
            }

            foreach (var item in m_TemperatureCallbacks.Values.ToList())
            {
                if (item.Keys.Contains(uuid))
                {
                    item.Remove(uuid);
                }
            }

            return "Client ID: " + uuid + " Disconnected from servere";
        }


        public string SubscribeToMeasurement(string location, string uuid, params string[] pars)
        {
            string reply = "Error";
            var callback = OperationContext.Current.GetCallbackChannel<IMeasurementUserCallback>();

            if (m_MeasurementPublishers[location] == null)
            {
                reply =  "Location unavailable!!";
            }
            else
            {
                if (pars[0].Equals("temp") || pars[0].Equals("both"))
                {
                    if (!m_TemperatureCallbacks[location].Keys.Contains(uuid))
                    {
                        m_TemperatureCallbacks[location].Add(uuid, callback);
                        reply = "Subscribed!";
                    }
                    else
                    {
                        reply = "Allready subscribed";
                    }
                }
                if (pars[0].Equals("humid") || pars[0].Equals("both"))
                {
                    if (!m_HumidityCallbacks[location].Keys.Contains(uuid))
                    {
                        m_HumidityCallbacks[location].Add(uuid, callback);
                        reply = "Subscribed!";
                    }
                    else
                    {
                        reply = "Allready subscribed";
                    }
                }
            }

            return reply;
        }

        public string UnsubscribeToMeasurement(string location, string uuid, params string[] pars)
        {
            string reply = "Error";

            if (m_MeasurementPublishers.Keys.Contains(location))
            {
                if (pars[0].Equals("temp") || pars[0].Equals("both"))
                {
                    if (m_TemperatureCallbacks[location].Keys.Contains(uuid))
                    {
                        m_TemperatureCallbacks[location].Remove(uuid);
                        reply = "Unsubscribed!";
                    }
                    else
                    {
                        reply = "Unknown unsubscriber!";
                    }
                }
                if (pars[0].Equals("humid") || pars[0].Equals("both"))
                {
                    if (m_HumidityCallbacks[location].Keys.Contains(uuid))
                    {
                        m_HumidityCallbacks[location].Remove(uuid);
                        reply = "Unsubscribed!";
                    }
                    else
                    {
                        reply = "Unknown unsubscriber!";
                    }
                }
            }

            return reply;
        }

        public void SaveToDB()
        {
            try
            {
                MyDbContext.SaveChanges();
            }
            catch (InvalidOperationException ex)
            {
                Exception e = ex.InnerException;
            }
           
           // foreach (var item in hm)
           // {
           //     HumidityMeasurement m = new HumidityMeasurement();
           //     //RMUnit rmu = m_MyDbContext.RMUnits.ToList().FirstOrDefault(r => r.LocationName.Equals(measurement.MyLocationName));
           //     m.Value = item.MyValue;
           //     m.LocationName = item.MyLocationName;
           //     m.DateTime = item.MyDateTime;
           //     m_MyDbContext.HumidityMeasurements.Add(m);  
           // }
           //// m_MyDbContext.SaveChanges();
           // hm.Clear();

           // foreach (var item in tm)
           // {
           //     TemperatureMeasurement m = new TemperatureMeasurement();
           //     //RMUnit rmu = m_MyDbContext.RMUnits.ToList().FirstOrDefault(r => r.LocationName.Equals(measurement.MyLocationName));
           //     m.Value = item.MyValue;
           //     m.LocationName = item.MyLocationName;
           //     m.DateTime = item.MyDateTime;
           //     m_MyDbContext.TemperatureMeasurements.Add(m);
                
           // }

           // m_MyDbContext.SaveChanges();
           // tm.Clear();
        }
    }


}
