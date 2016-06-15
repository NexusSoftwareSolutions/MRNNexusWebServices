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
    public class DTO_ClaimDocument : DTO_Base
    {
        [DataMember]
        public int DocumentID { get; set; }
        [DataMember]
        public int ClaimID { get; set; }
        [DataMember]
        public string FilePath { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string FileBytes { get; set; }
        [DataMember]
        public string FileExt { get; set; }
        [DataMember]
        public int DocTypeID { get; set; }

        

        public DateTime DocumentDate { get; set; }
		[DataMember(Name = "DocumentDate")]
		private string DocumentDateForSerialization { get; set; }
		[OnSerializing]
		void onSerializing(StreamingContext context)
		{
            if(DocumentDate != null)
			    this.DocumentDateForSerialization = JsonConvert.SerializeObject(this.DocumentDate).Replace('"', ' ').Trim();
		}
		[OnDeserialized]
		void OnDeserialized(StreamingContext context)
		{
            if(DocumentDateForSerialization != null)
			    this.DocumentDate = DateTime.Parse(this.DocumentDateForSerialization);
		}



		[DataMember]
        public string SignatureImagePath { get; set; }
        [DataMember]
        public Nullable<int> NumSignatures { get; set; }
        [DataMember]
        public string InitialImagePath { get; set; }
        [DataMember]
        public Nullable<int> NumInitials { get; set; }
    }
}
