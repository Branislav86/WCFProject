using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.ServiceModel;

namespace MeasurementUnit
{
    class MeasurementUnitCallback : WCF_TempHumidService.IMeasurementUnitServiceCallback, IDisposable
    {
        private Timer m_TemperatureT;
        private Timer m_HumidityT;
        private WCF_TempHumidService.MyRMUnit m_RMUnit;

        public WCF_TempHumidService.MyRMUnit MyRMUnit
        {
            get { return m_RMUnit; }
           // set { WCF_TempHumidService. myVar = value; }
        }
        
        

        static WCF_TempHumidService.MeasurementUnitServiceClient m_Client;
        private InstanceContext m_Context;
        

        public MeasurementUnitCallback(int temperatureT, int humidityT)
        {
            m_TemperatureT = new Timer(temperatureT);
            m_HumidityT = new Timer(humidityT);
            m_Context = new InstanceContext(this);
            m_Client = new WCF_TempHumidService.MeasurementUnitServiceClient(m_Context);
            m_TemperatureT.Elapsed += m_TemperatureT_Elapsed;
            m_HumidityT.Elapsed += m_HumidityT_Elapsed;
          //  m_UuId = "";
        }

        void m_HumidityT_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (m_Client.State == CommunicationState.Opened)
            {
                WCF_TempHumidService.MyHumidityMeasurement m = GetNewHumidityMeasurement();
                m_Client.SendNewHumidityMeasurement(m);

            }
        }

        void m_TemperatureT_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (m_Client.State == CommunicationState.Opened)
            {
                WCF_TempHumidService.MyTemperatureMeasurement m = GetNewTemperatureMeasurement();
                m_Client.SendNewTemperatureMeasurement(m);
                
            }
        }

        public void StartSendingMeasuremetns()
        {
            Console.WriteLine("Started sending..");
            m_HumidityT.Enabled = true;
            m_TemperatureT.Enabled = true;
        }

        public void Subscribe(string location)
        {

            try
            {
                m_Client.Subscribe(location);
            }
            catch (EndpointNotFoundException)
            {
                int rstop = 2;
            }


        }

        public Timer HumidityT
        {
            get { return m_HumidityT; }
            set { m_HumidityT = value; }
        }
        

        public Timer TemperatureT
        {
            get { return m_TemperatureT; }
            set { m_TemperatureT = value; }
        }

        public void Dispose()
        {
            m_Client.Close();
            m_HumidityT.Dispose();
            m_TemperatureT.Dispose();
        }

        public void OnTemperatureMeasurementSent(string message)
        {
            throw new NotImplementedException();
        }

        public void OnHumidityMeasurementSent(string message)
        {
            Console.WriteLine(message);
        }

        public void OnHumidityMeasurementSent(WCF_TempHumidService.MyHumidityMeasurement measurement)
        {
            Console.WriteLine("Humidity: {0}, Time: {1}, Location: {2},  RMUnitId: {3}",
                              measurement.MyValue, measurement.MyDateTime,
                              measurement.MyLocationName, MyRMUnit.MyName);
        }

        public List<string> GetAllLocationsFromService()
        {
            string[] locations;
            List<string> returnLocations = new List<string>();
            try
            {
                locations = m_Client.GetAllLocations();
            }
            catch (Exception)
            {
                
                throw;
            }

            for (int i = 0; i < locations.Length; i++)
            {
                returnLocations.Add(locations[i]);
            }

            return returnLocations;
        }


        public void OnUnsubcribe(string message)
        {
            Console.WriteLine(message);
        }

        public WCF_TempHumidService.MyHumidityMeasurement GetNewHumidityMeasurement()
        {
            WCF_TempHumidService.MyHumidityMeasurement m = new WCF_TempHumidService.MyHumidityMeasurement();
            m.MyDateTime = DateTime.Now;
            m.MyLocationName = MyRMUnit.MyLocationName;
            Random rnd = new Random();
            m.MyValue = 980.0f + rnd.Next(40);
            return m;
        }

        WCF_TempHumidService.MyTemperatureMeasurement GetNewTemperatureMeasurement()
        {
            WCF_TempHumidService.MyTemperatureMeasurement m = new WCF_TempHumidService.MyTemperatureMeasurement();
            m.MyDateTime = DateTime.Now;
            m.MyLocationName = MyRMUnit.MyLocationName;
            Random rnd = new Random();
            m.MyValue = 20.0f + rnd.Next(10);
            return m;
        }

        public void Unsubscribe()
        {
            m_HumidityT.Enabled = false;
            m_TemperatureT.Enabled = false;
            m_Client.Unsubscribe(m_RMUnit.MyName);
        }


        public void OnSubcribe(WCF_TempHumidService.MyRMUnit unit, string message)
        {
            if (unit != null)
            {
                Console.WriteLine("Subscribe as publisher at location: {0}, with Id: {1}", unit.MyLocationName, unit.MyName);
                m_RMUnit = unit;
                this.StartSendingMeasuremetns();
            }
            else
            {
                Console.WriteLine(message);
            }
        }


        public void OnTemperatureMeasurementSent(WCF_TempHumidService.MyTemperatureMeasurement measurement)
        {
            Console.WriteLine("Temperature: {0}, Time: {1}, Location: {2},  RMUnitId: {3}",
                              measurement.MyValue, measurement.MyDateTime,
                              measurement.MyLocationName, MyRMUnit.MyName);
        }
    }
}
