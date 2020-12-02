using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ContactManagerLambda.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Threading.Tasks;

namespace ContactManagerLambda.DynamoDb
{
    public interface IPhoneContact
    {
        Task AddNewEntry(int id, int ContactId, string PhoneType, string PhoneNumber);

        Task<ContactManagerLambda.Models.DynamoTableItems> getPhoneContacts(int? Id);

        Task<PhoneModel> Update(int Id, string Number, string? newPhoneType);

        Task Delete(int id);

        Task<List<PhoneModel>> getMasterPhoneContacts(int? Id, bool isContactId);
    }
}
