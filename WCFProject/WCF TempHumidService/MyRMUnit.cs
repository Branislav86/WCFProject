using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace WCF_TempHumidService
{
    [DataContract]
    public  class MyRMUnit
    {
        private string m_Name;
        private string m_LocationName;

        [DataMember]
        public string MyLocationName
        {
            get { return m_LocationName; }
            set { m_LocationName = value; }
        }
        
        [DataMember]
        public string MyName
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        
    }
}
