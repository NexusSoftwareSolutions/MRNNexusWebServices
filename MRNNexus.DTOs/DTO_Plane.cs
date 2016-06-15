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
    public class DTO_Plane : DTO_Base
    {
        [DataMember]
        public int PlaneID { get; set; }
        [DataMember]
        public int PlaneTypeID { get; set; }
        [DataMember]
        public int InspectionID { get; set; }
        [DataMember]
        public int GroupNumber { get; set; }
        [DataMember]
        public int? NumOfLayers { get; set; }
        [DataMember]
        public int? ThreeAndOne { get; set; }
        [DataMember]
        public int? FourAndUp { get; set; }
        [DataMember]
        public int? Pitch { get; set; }
        [DataMember]
        public int? HipValley { get; set; }
        [DataMember]
        public int? RidgeLength { get; set; }
        [DataMember]
        public int? RakeLength { get; set; }
        [DataMember]
        public int? EaveHeight { get; set; }
        [DataMember]
        public int? EaveLength { get; set; }
        [DataMember]
        public int? NumberDecking { get; set; }
        [DataMember]
        public int? StepFlashing { get; set; }
        [DataMember]
        public double? SquareFootage { get; set; }
        [DataMember]
        public string ItemSpec { get; set; }
        
    }
}
