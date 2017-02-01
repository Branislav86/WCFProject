using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace WCF_TempHumidService
{
    [ServiceContract(SessionMode=SessionMode.Required, CallbackContract=typeof(IMeasurementUserCallback))]
    public interface IMeasurementClientService
    {
        [OperationContract(IsInitiating = true, IsOneWay = false)]
        string ConnnectToServer();

        [OperationContract(IsOneWay = false)]
        string SubscribeToMeasurement(string location, string uuid, params string[] pars);

        [OperationContract(IsOneWay = false)]
        string UnsubscribeToMeasurement(string location, string uuid, params string[] pars);

        [OperationContract(IsOneWay = false)]
        MyMeasurements GetAllMeasurementsFromLocationDateToDate(
                              DateTime from, DateTime todate, params string[] pars);

        [OperationContract(IsOneWay = false)]
        MyMeasurements GetAllMeasuremensWithLimit(float limit, params string[] pars);

        [OperationContract(IsOneWay = false)]
        MyMeasurements GetAverageMeasurementsFromLocation(string locatin, params string[] pars);

        [OperationContract(IsOneWay = false, IsTerminating = true)]
        string DisconnectFromServer(string  uuid);


    }
}
