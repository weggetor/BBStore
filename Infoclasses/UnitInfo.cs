using System;
using System.Runtime.Serialization;

namespace Bitboxx.DNNModules.BBStore
{
    [Serializable]
    [DataContract()]
    public class UnitInfo
    {
        public UnitInfo()
        {
            UnitId = 0;
            PortalId = 0;
            Decimals = 0;
            Unit = "";
            Symbol = "";
        }
        [DataMember()]
        public Int32 UnitId { get; set; }
        [DataMember()]
        public Int32 PortalId { get; set; }
        [DataMember()]
        public Int32 Decimals { get; set; }
        public string Unit { get; set; }
        public string Symbol { get; set; }
        [DataMember()]
        public int _status { get; set; }
    }
}