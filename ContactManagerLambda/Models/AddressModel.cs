using System;
using System.Collections.Generic;
using System.Text;

namespace DynamoDB.libs.Models
{
    public class AddressModel
    {
        public int Id { get; set; }
        public int ContactId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string adrState { get; set; }
        public string Zip { get; set; }
    }
}