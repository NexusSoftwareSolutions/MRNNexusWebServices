using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MRNNexusDTOs;
using MRNNexus_DAL;

namespace MRNNexus_Service
{
    public class ObjectConverter
    {


        public static DTO_Address toDTO(proc_GetAddressByID_Result obj)
        {
            return new DTO_Address
            {
                AddressID = obj.AddressID,
                CustomerID = obj.CustomerID,
                Address = obj.Address,
                Zip = obj.Zip,
            };
        }
    }
}