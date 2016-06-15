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
    public class DTO_KnockerResponse : DTO_Base
    {
        [DataMember]
        public int KnockerResponseID { get; set; }
        [DataMember]
        public int KnockerID { get; set; }
        [DataMember]
        public int KnockResponseTypeID { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public string Zip { get; set; }
        [DataMember]
        public double? Latitude { get; set; }
        [DataMember]
        public double? Longitude { get; set; }


    }
}
