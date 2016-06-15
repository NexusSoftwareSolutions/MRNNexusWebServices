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
    public class DTO_NewRoof : DTO_Base
    {
        [DataMember]
        public int NewRoofID { get; set; }
        [DataMember]
        public int ClaimID { get; set; }
        [DataMember]
        public int ProductID { get; set; }
        [DataMember]
        public double UpgradeCost { get; set; }
        [DataMember]
        public string Comments { get; set; }
    }
}
