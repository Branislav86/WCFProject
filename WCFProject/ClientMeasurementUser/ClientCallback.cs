using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace ClientMeasurementUser
{
    public class ClientCallback : IDisposable, WCF_TempHumidService.IMeasurementClientServiceCallback
    {
        private WCF_TempHumidService.MeasurementClientServiceClient m_Client;
        private string m_UuId;

        public string MyUuid
        {
            get { return m_UuId; }
        }
        
        public ClientCallback()
        {
            InstanceContext context = new InstanceContext(this);
            m_Client = new WCF_TempHumidService.MeasurementClientServiceClient(context);
        }

        public string ConnectToServer()
        {          
            m_UuId =  m_Client.ConnnectToServer();
            return MyUuid;
        }

        public WCF_TempHumidService.MyMeasurements GetAllMeasurementsDateToDate(DateTime from, DateTime todate, params string[] pars)
        {
            WCF_TempHumidService.MyMeasurements m = m_Client.GetAllMeasurementsFromLocationDateToDate(from, todate, pars);
            PrintData(m);
            return m;
        }

        public WCF_TempHumidService.MyMeasurements GetAllMeasuremensWithLimit(float limit, params string[] pars)
        {
            WCF_TempHumidService.MyMeasurements m = m_Client.GetAllMeasuremensWithLimit(limit, pars);
            PrintData(m);
            return m;
        }

        public WCF_TempHumidService.MyMeasurements GetAverageMeasurementsFromLocation(string location, params string[] pars)
        {
            WCF_TempHumidService.MyMeasurements m = m_Client.GetAverageMeasurementsFromLocation(location, pars);
            PrintData(m);
            return m;
        }

        public void PrintData(WCF_TempHumidService.MyMeasurements m)
        {
            foreach (var item in m.MyHumMeasurements)
            {
                Console.WriteLine("Humidity:{0} mbar,  location:{1},  date:{2},  unitID:{3}",
                                  item.MyValue, item.MyLocationName, item.MyDateTime, item.MyRMUnitName);
            }

            foreach (var item in m.MyTempMeasurements)
            {
                Console.WriteLine("Temperature:{0} C,  location:{1},  date:{2},  unitID:{3}",
                                  item.MyValue, item.MyLocationName, item.MyDateTime, item.MyRMUnitName);
            }

            if (m.MyAverageHumidity != 0)
            {
                Console.WriteLine("Average humidity:{0}", m.MyAverageHumidity);
            }

            if (m.MyAverageTemperature != 0)
            {
                Console.WriteLine("Average temperature:{0}", m.MyAverageTemperature);
            }

        }

        public string SubscribeToMeasurement(string location, string uuid, params string[] pars)
        {
            return m_Client.SubscribeToMeasurement(location, uuid, pars);
        }

        public string UnsubscribeToMeasurement(string location, string uuid, params string[] pars)
        {
            return m_Client.UnsubscribeToMeasurement(location, uuid, pars);
        }

        public string DisconnectFromServer()
        {
            return m_Client.DisconnectFromServer(MyUuid);
        }

        public void Dispose()
        {
            m_Client.Close();
        }

        public void onNewTemperatureMeasurementReceived(WCF_TempHumidService.MyTemperatureMeasurement measurement)
        {
            Console.WriteLine("Temperature:{0},  datetime:{1},  location:{2},   Uuid:{3}", measurement.MyValue, measurement.MyDateTime, measurement.MyLocationName, measurement.MyRMUnitName);
        }

        public void onNewHumidityMeasurementReceived(WCF_TempHumidService.MyHumidityMeasurement measurement)
        {
            Console.WriteLine("Humidity:{0},  datetime:{1},  location:{2}", measurement.MyValue, measurement.MyDateTime, measurement.MyLocationName);
        }
    }
}
