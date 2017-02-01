using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace WCF_TempHumidService
{
    [ServiceContract]
    public interface IMeasurementUserCallback
    {
        [OperationContract(IsOneWay = true)]
        void onNewTemperatureMeasurementReceived(MyTemperatureMeasurement measurement);


        [OperationContract(IsOneWay = true)]
        void onNewHumidityMeasurementReceived(MyHumidityMeasurement measurement);

    }
}
