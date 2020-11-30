using System;
using System.Collections.Generic;
using System.Text;

namespace DynamoDB.libs.Models
{
    public class DynamoTableItems
    {
        public IEnumerable<DynamoDB.libs.Models.MasterContactModel> MasterContacts { get; set; }

        public IEnumerable<DynamoDB.libs.Models.PhoneModel> PhoneContacts { get; set; }

        public IEnumerable<DynamoDB.libs.Models.AddressModel> AddressContacts { get; set; }

        public IEnumerable<DynamoDB.libs.Models.SecondaryEmailModel> SecondaryEmails { get; set; }
    }
}
