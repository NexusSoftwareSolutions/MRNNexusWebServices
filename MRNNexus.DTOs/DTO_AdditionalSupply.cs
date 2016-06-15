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
    public class DTO_AdditionalSupply : DTO_Base
    {
        [DataMember]
        public int AdditionalSuppliesID { get; set; }
        [DataMember]
        public int ClaimID { get; set; }

        
        public DateTime PickUpDate { get; set; }
        [DataMember(Name = "PickUpDate")]
		private string PickUpDateForSerialization { get; set; }

		[OnSerializing]
		void OnSerializing(StreamingContext context)
		{
            if(PickUpDate != null)
			    this.PickUpDateForSerialization = JsonConvert.SerializeObject(this.PickUpDate).Replace('"', ' ').Trim();

            if(DropOffDate != null)
                this.DropOffDateForSerialization = JsonConvert.SerializeObject(this.DropOffDate).Replace('"', ' ').Trim();
        }
		[OnDeserialized]
		void OnDeserialized(StreamingContext context)
		{
            if(PickUpDateForSerialization != null)
			    this.PickUpDate = DateTime.Parse(this.PickUpDateForSerialization);

            if(DropOffDateForSerialization != null)
                this.DropOffDate = DateTime.Parse(this.DropOffDateForSerialization);
        }

        public DateTime DropOffDate { get; set; }
		[DataMember(Name = "DropOffDate")]
		private string DropOffDateForSerialization { get; set; }
		

		[DataMember]
        public string Items { get; set; }
        [DataMember]
        public double Cost { get; set; }
        [DataMember]
        public string ReceiptImagePath { get; set; }
    }
}
