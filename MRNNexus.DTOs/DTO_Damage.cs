using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MRNNexusDTOs
{
    [KnownType(typeof(DTO_Base))]
    [DataContract]
    public class DTO_Damage : DTO_Base
    {
        [DataMember]
        public int DamageID { get; set; }
        [DataMember]
        public int PlaneID { get; set; }
        [DataMember]
        public int DamageTypeID { get; set; }
        [DataMember]
        public int DamageMeasurement { get; set; }
        [DataMember]
        public int XCoordinate { get; set; }
        [DataMember]
        public int YCoordinate { get; set; }
        [DataMember]
        public int DocumentID { get; set; }
    }
}
