using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ContactManagerLambda.DynamoDb;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Threading.Tasks;


namespace ContactManagerLambda.DynamoDb
{
    public interface IMasterContacts
    {
        Task<ContactManagerLambda.Models.DynamoTableItems> getMasterContacts(int? Id);

        Task<ContactManagerLambda.Models.MasterContactModel> Update(int Id, string PrimaryEmail, string LastName, string? newFirstName);

        Task AddNewEntry(int id, string Name, string PrimaryEmail);

        Task Delete(int id, string LastName);
    }
}
