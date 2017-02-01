using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace WCF_TempHumidService
{
    [DataContract]
    public class MyMeasurements
    {
        private List<MyHumidityMeasurement> m_HumMeasurements;
        private List<MyTemperatureMeasurement> m_TempMeasurements;
        private float m_AverageHumidity;
        private float m_AverageTemperature;

        [DataMember]
        public float MyAverageTemperature
        {
            get { return m_AverageTemperature; }
            set { m_AverageTemperature = value; }
        }
        
        [DataMember]
        public float MyAverageHumidity
        {
            get { return  m_AverageHumidity; }
            set {  m_AverageHumidity = value; }
        }
        

        public MyMeasurements()
        {
            m_HumMeasurements = new List<MyHumidityMeasurement>();
            m_TempMeasurements = new List<MyTemperatureMeasurement>();
            MyAverageTemperature = 0.0f;
            MyAverageHumidity = 0.0f;
        }

        [DataMember]
        public List<MyTemperatureMeasurement> MyTempMeasurements
        {
            get { return m_TempMeasurements; }
            set { m_TempMeasurements = value; }
        }


        [DataMember]
        public List<MyHumidityMeasurement> MyHumMeasurements
        {
            get { return m_HumMeasurements; }
            set { m_HumMeasurements = value; }
        }
        
    }
}
