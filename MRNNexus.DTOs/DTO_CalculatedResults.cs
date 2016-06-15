using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace MRNNexusDTOs
{
    [KnownType(typeof(DTO_Base))]
    [DataContract]
    public class DTO_CalculatedResults : DTO_Base
    {
        [DataMember]
        public double SumOfInvoices { get; set; }

        [DataMember]
        public double SumOfPayments { get; set; }

        public DateTime MostRecentDate { get; set; }

        [DataMember(Name = "MostRecentDate")]
        public string MostRecentDateForSerialization { get; set; }

        [OnSerializing]
        void onSerializing(StreamingContext context)
        {
            if (MostRecentDate != null)
                this.MostRecentDateForSerialization = JsonConvert.SerializeObject(this.MostRecentDate).Replace('"', ' ').Trim();
        }
        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            if (MostRecentDateForSerialization != null)
                this.MostRecentDate = DateTime.Parse(this.MostRecentDateForSerialization);
        }
    }
}
