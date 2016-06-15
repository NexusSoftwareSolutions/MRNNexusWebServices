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
    public class DTO_Order : DTO_Base
    {
        [DataMember]
        public int OrderID { get; set; }
        [DataMember]
        public int VendorID { get; set; }
        
        public DateTime DateOrdered { get; set; }
		[DataMember(Name = "DateOrdered")]
		private string DateOrderedForSerialization { get; set; }
		[OnSerializing]
		void onSerializing(StreamingContext context)
		{
            if(DateOrdered != null)
			    this.DateOrderedForSerialization = JsonConvert.SerializeObject(this.DateOrdered).Replace('"', ' ').Trim();

            if(DateDropped != null)
                this.DateDroppedForSerialization = JsonConvert.SerializeObject(this.DateDropped).Replace('"', ' ').Trim();

            if(ScheduledInstallation != null)
                this.ScheduledInstallationForSerialization = JsonConvert.SerializeObject(this.ScheduledInstallation).Replace('"', ' ').Trim();
        }
		[OnDeserialized]
		void OnDeserialized(StreamingContext context)
		{
            if(DateOrderedForSerialization != null)
			    this.DateOrdered = DateTime.Parse(this.DateOrderedForSerialization);

            if(DateDroppedForSerialization != null)
                this.DateDropped = DateTime.Parse(this.DateDroppedForSerialization);

            if(ScheduledInstallationForSerialization != null)
                this.ScheduledInstallation = DateTime.Parse(this.ScheduledInstallationForSerialization);
        }

        public DateTime DateDropped { get; set; }
		[DataMember(Name = "DateDropped")]
		private string DateDroppedForSerialization { get; set; }
		
		
        public DateTime ScheduledInstallation { get; set; }
		[DataMember(Name = "ScheduledInstallation")]
		private string ScheduledInstallationForSerialization { get; set; }

		[DataMember]
        public int ClaimID { get; set; }
    }
}
