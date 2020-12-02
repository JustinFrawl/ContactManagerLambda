using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ContactManagerLambda.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Threading.Tasks;
using System.Linq;


namespace ContactManagerLambda.DynamoDb
{
    public interface ISecondaryEmail
    {
        Task AddNewEntry(int id, int ContactId, string Email);

        Task<ContactManagerLambda.Models.DynamoTableItems> getSecondaryEmails(int? Id);

        Task<SecondaryEmailModel> Update(int Id, string Email);

        Task Delete(int id);

        Task<List<SecondaryEmailModel>> getMasterSecondaryEmails(int? Id, bool isContactId);
    }
}
