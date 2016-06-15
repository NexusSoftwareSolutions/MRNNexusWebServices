using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MRNNexusDTOs
{
    [KnownType(typeof(DTO_Base))]
    [DataContract]
    public class DTO_ClaimStatus : DTO_Base
    {
        [DataMember]
        public int ClaimStatusID { get; set; }

        [DataMember]
        public int ClaimID { get; set; }

        [DataMember]
        public int ClaimStatusTypeID { get; set; }

        public DateTime ClaimStatusDate { get; set; }

        [DataMember(Name = "ClaimStatusDate")]
        public string ClaimStatusDateForSerialization { get; set; }

        [OnSerializing]
        public void onSerializing(StreamingContext context)
        {
            if (ClaimStatusDate != null)
                this.ClaimStatusDateForSerialization = JsonConvert.SerializeObject(this.ClaimStatusDate).Replace('"', ' ').Trim();
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            if (ClaimStatusDateForSerialization != null)
                this.ClaimStatusDate = DateTime.Parse(this.ClaimStatusDateForSerialization);
        }
    }
}
