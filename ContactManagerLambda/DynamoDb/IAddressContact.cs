﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DynamoDB.libs.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Threading.Tasks;
using System.Linq;


namespace DynamoDB.libs.DynamoDb
{
    public interface IAddressContact
    {
        Task AddNewEntry(int id, int ContactId, string Street, string City, string State, string Zip);

        Task<DynamoDB.libs.Models.DynamoTableItems> getAddressContacts(int? Id);

        Task<AddressModel> Update(int Id, string Street, string? City, string? State, string? Zip);

        Task Delete(int id);

        Task<List<AddressModel>> getMasterAddressContacts(int? Id, bool isContactId);
    }
}
