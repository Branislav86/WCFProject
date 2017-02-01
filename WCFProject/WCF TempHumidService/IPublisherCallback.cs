using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace WCF_TempHumidService
{
    [ServiceContract]
    public interface IPublisherCallback
    {

        [OperationContract(IsOneWay=true)]
        void OnHumidityMeasurementSent(MyHumidityMeasurement measurement);

        [OperationContract(IsOneWay = true)]
        void OnTemperatureMeasurementSent(MyTemperatureMeasurement measurement);

        [OperationContract(IsOneWay = true)]
        void OnSubcribe(MyRMUnit unit, string message);

        [OperationContract(IsOneWay = true)]
        void OnUnsubcribe(string message);
    }
}
