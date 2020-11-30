using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DynamoDB.libs.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Threading.Tasks;

namespace DynamoDB.libs.DynamoDb
{
    public interface IPhoneContact
    {
        Task AddNewEntry(int id, int ContactId, string PhoneType, string PhoneNumber);

        Task<DynamoDB.libs.Models.DynamoTableItems> getPhoneContacts(int? Id);

        Task<PhoneModel> Update(int Id, string Number, string? newPhoneType);

        Task Delete(int id);

        Task<List<PhoneModel>> getMasterPhoneContacts(int? Id, bool isContactId);
    }
}
