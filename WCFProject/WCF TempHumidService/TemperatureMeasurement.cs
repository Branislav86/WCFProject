//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WCF_TempHumidService
{
    using System;
    using System.Collections.Generic;
    
    public partial class TemperatureMeasurement
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public System.DateTime DateTime { get; set; }
        public string LocationName { get; set; }
    
        public virtual RMUnit RMUnit { get; set; }
    }
}
