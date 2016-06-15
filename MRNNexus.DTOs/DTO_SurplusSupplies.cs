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
    public class DTO_SurplusSupplies : DTO_Base
    {
        [DataMember]
        public int SurplusSuppliesID { get; set; }
        [DataMember]
        public int ClaimID { get; set; }
        [DataMember]
        public int UnitOfMeasureID { get; set; }
        [DataMember]
        public int Quantity { get; set; }

        public DateTime PickUpDate { get; set; }
        [DataMember(Name = "PickUpDate")]
        private string PickUpDateForSerialization { get; set; }
        public DateTime DropOffDate { get; set; }
        [DataMember(Name = "DropOddDate")]
        private string DropOffDateForSerialization { get; set; }

        [OnSerializing]
        void onSerilizing(StreamingContext context)
        {
            if(PickUpDate != null)
                this.PickUpDateForSerialization = JsonConvert.SerializeObject(this.PickUpDate).Replace('"', ' ').Trim();
            
            if(DropOffDate != null)
                this.DropOffDateForSerialization = JsonConvert.SerializeObject(this.DropOffDate).Replace('"', ' ').Trim();
        }
        [OnDeserialized]
        void onDeserialized(StreamingContext context)
        {
            if(PickUpDateForSerialization != null)
                this.PickUpDate = DateTime.Parse(this.PickUpDateForSerialization);

            if(DropOffDateForSerialization != null)
                this.DropOffDate = DateTime.Parse(this.DropOffDateForSerialization);
        }

        [DataMember]
        public string Items { get; set; }

    }
}
