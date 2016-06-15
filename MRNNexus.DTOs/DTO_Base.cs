using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MRNNexusDTOs
{
    //[KnownType(typeof(DTO_User))]
    [DataContract]
    public class DTO_Base
    {
        [DataMember]
        public string Exception { get; set; }
        [DataMember]
        public string InnerException { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public bool SuccessFlag { get; set; }
    }
}
