using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace WCF_TempHumidService
{
    [DataContract]
    public class MyHumidityMeasurement 
    {
        //private const string m_Unit = "mbar";

        //public string MyUnit
        //{
        //    get { return m_Unit; }
        //    //set { m_Unit = "mbar" = value; }
        //}


        private float m_Value;
        private DateTime m_DateTime;
        private string m_LocationName;
        private string m_RMUnitName;

        [DataMember]
        public string MyRMUnitName
        {
            get { return m_RMUnitName; }
            set { m_RMUnitName = value; }
        }
        

        [DataMember]
        public string MyLocationName
        {
            get { return m_LocationName; }
            set { m_LocationName = value; }
        }
        [DataMember]
        public DateTime MyDateTime
        {
            get { return m_DateTime; }
            set { m_DateTime = value; }
        }

        [DataMember]
        public float MyValue
        {
            get { return m_Value; }
            set { m_Value = value; }
        }
        
    }
}
