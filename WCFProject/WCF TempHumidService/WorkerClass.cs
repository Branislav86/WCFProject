using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace WCF_TempHumidService
{
    public class WorkerClass
    {
        private IMeasurementUserCallback m_Callback;
        private MyHumidityMeasurement m_Humidity;
        private MyTemperatureMeasurement m_Temperature;

        public MyTemperatureMeasurement MyTemperature
        {
            get { return m_Temperature; }
            set { m_Temperature = value; }
        }
        

        public MyHumidityMeasurement MyHumidity
        {
            get { return m_Humidity; }
            set { m_Humidity = value; }
        }
        

        public IMeasurementUserCallback MyCallback
        {
            get { return m_Callback; }
            set { m_Callback = value; }
        }
        
    }
}
