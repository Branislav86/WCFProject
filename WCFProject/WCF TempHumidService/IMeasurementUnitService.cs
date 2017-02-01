using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace WCF_TempHumidService
{
    [ServiceContract(CallbackContract=typeof(IPublisherCallback), SessionMode=SessionMode.Required)]
    public interface IMeasurementUnitService 
    {

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        List<string> GetAllLocations();

        [OperationContract(IsOneWay=true)]
        void Subscribe(string location);

        [OperationContract(IsOneWay = false)]
        void Unsubscribe(string uuid);

        [OperationContract(IsOneWay=true)]
        void SendNewTemperatureMeasurement(MyTemperatureMeasurement measurement);

        [OperationContract(IsOneWay = true)]
        void SendNewHumidityMeasurement(MyHumidityMeasurement measurement);

        
    }
}
