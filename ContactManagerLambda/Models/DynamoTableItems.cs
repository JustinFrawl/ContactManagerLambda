using System;
using System.Collections.Generic;
using System.Text;

namespace ContactManagerLambda.Models
{
    public class DynamoTableItems
    {
        public IEnumerable<ContactManagerLambda.Models.MasterContactModel> MasterContacts { get; set; }

        public IEnumerable<ContactManagerLambda.Models.PhoneModel> PhoneContacts { get; set; }

        public IEnumerable<ContactManagerLambda.Models.AddressModel> AddressContacts { get; set; }

        public IEnumerable<ContactManagerLambda.Models.SecondaryEmailModel> SecondaryEmails { get; set; }
    }
}
