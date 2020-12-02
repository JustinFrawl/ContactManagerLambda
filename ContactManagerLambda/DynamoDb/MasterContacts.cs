using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ContactManagerLambda.DynamoDb;
using ContactManagerLambda.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Threading.Tasks;
using System.Linq;


namespace ContactManagerLambda.DynamoDb
{
    public class MasterContacts : IMasterContacts
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private static readonly string tableName = "Contact_Master";
        private readonly IPhoneContact _PhoneContacts;
        private readonly IAddressContact _AddressContacts;
        private readonly ISecondaryEmail _SecondaryEmail;

        public MasterContacts(IAmazonDynamoDB dynamoDbClient, IPhoneContact phoneContacts, IAddressContact addressContacts, ISecondaryEmail secondaryEmail)
        {
            _dynamoDbClient = dynamoDbClient;
            _PhoneContacts = phoneContacts;
            _AddressContacts = addressContacts;
            _SecondaryEmail = secondaryEmail;
        }

        #region C reate Logic
        public async Task AddNewEntry(int id, string Name, string PrimaryEmail)
        {
            var queryRequest = PutRequestBuilder(id, Name, PrimaryEmail);

            await PutitemAsync(queryRequest);
        }

        private PutItemRequest PutRequestBuilder(int id, string Name, string PrimaryEmail)
        {
            var item = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue{ N = id.ToString()} },
                { "LastName", new AttributeValue {S = Name.Split(" ")[1] } },
                { "PrimaryEmail", new AttributeValue {S = PrimaryEmail } },
                { "FirstName", new AttributeValue {S = Name.Split(" ")[0] } }
            };

            return new PutItemRequest
            {
                TableName = "Contact_Master",
                Item = item
            };
        }

        private async Task PutitemAsync(PutItemRequest request)
        {
            await _dynamoDbClient.PutItemAsync(request);
        }
        #endregion
        #region R ead Logic
        public async Task<ContactManagerLambda.Models.DynamoTableItems> getMasterContacts(int? Id)
        {
            var queryRequest = GetRequestBuilder(Id);

            var result = await ScanAsync(queryRequest);

            return new DynamoTableItems
            {
                MasterContacts = result.Items.Select(Map).ToList(),
                PhoneContacts = _PhoneContacts.getMasterPhoneContacts(Id, true).Result,
                AddressContacts = _AddressContacts.getMasterAddressContacts(Id, true).Result,
                SecondaryEmails = _SecondaryEmail.getMasterSecondaryEmails(Id, true).Result
            };
        }

        private MasterContactModel Map(Dictionary<string, AttributeValue> result)
        {
            return new MasterContactModel
            {
                Id = Convert.ToInt32(result["Id"].N),
                LastName = result["LastName"].S,
                Firstname = result["FirstName"].S,
                PrimaryEmail = result["PrimaryEmail"].S
            };
        }

        private async Task<ScanResponse> ScanAsync(ScanRequest queryRequest)
        {
            var response = await _dynamoDbClient.ScanAsync(queryRequest);

            return response;
        }

        private ScanRequest GetRequestBuilder(int? id)
        {
            if (!id.HasValue)
            {
                return new ScanRequest
                {
                    TableName = "Contact_Master"
                };
            }
            return new ScanRequest
            {
                TableName = "Contact_Master",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":v_Id", new AttributeValue{N = id.ToString()} }
                },
                FilterExpression = "Id = :v_Id",
                ProjectionExpression = "Id, LastName, FirstName, PrimaryEmail"

            };
        }
        #endregion
        #region U pdate Logic
        public async Task<MasterContactModel> Update(int Id, string PrimaryEmail, string LastName, string? newFirstName)
        {
            var response = await getMasterContacts(Id);

            var currentEmail = response.MasterContacts.Select(p => p.PrimaryEmail).FirstOrDefault();
            var currentFirstName = response.MasterContacts.Select(p => p.Firstname).FirstOrDefault();

            var lastName = response.MasterContacts.Select(p => p.LastName).FirstOrDefault();

            var request = UpdateRequestBuilder(Id, PrimaryEmail, currentEmail, lastName, currentFirstName, newFirstName);

            var result = await UpdateItemAsync(request);

            return new MasterContactModel
            {
                Id = Convert.ToInt32(result.Attributes["Id"].N),
                LastName = result.Attributes["LastName"].S,
                Firstname = result.Attributes["FirstName"].S,
                PrimaryEmail = result.Attributes["PrimaryEmail"].S
            };
        }

        private UpdateItemRequest UpdateRequestBuilder(int id, string primaryEmail, string currentEmail, string lastName,string currentFirstName, string? newFirstName)
        {
            var request = new UpdateItemRequest
            {
                Key = new Dictionary<string, AttributeValue>
                {
                    {
                        "Id", new AttributeValue
                        {
                            N = id.ToString()
                        }
                    },
                    {
                        "LastName", new AttributeValue
                        {
                            S = lastName
                        }
                    }
                },
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    {"#P", "PrimaryEmail"}
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {
                        ":newEmail", new AttributeValue
                        {
                            S = primaryEmail
                        }
                    },
                    {
                        ":currEmail", new AttributeValue
                        {
                            S = currentEmail
                        }
                    }
                },

                UpdateExpression = "SET #P = :newEmail",
                ConditionExpression = "#P = :currEmail",

                TableName = tableName,
                ReturnValues = "ALL_NEW"
            };
            if (!String.IsNullOrEmpty(newFirstName))
            {
                request.ExpressionAttributeNames.Add("#FN", "FirstName");
                request.ExpressionAttributeValues.Add(":newFirstName", new AttributeValue { S = newFirstName });
                request.UpdateExpression = request.UpdateExpression + ", #FN = :newFirstName";
            }
            return request;

        }

        private async Task<UpdateItemResponse> UpdateItemAsync(UpdateItemRequest request)
        {
            var response = await _dynamoDbClient.UpdateItemAsync(request);

            return response;
        }
        #endregion
        #region D elete Logic
        public async Task Delete(int id, string LastName)
        {
            var queryRequest = DeleteRequestBuilder(id, LastName);

            await DeleteItemAsync(queryRequest);
        }
        private DeleteItemRequest DeleteRequestBuilder(int id, string LastName)
        {
            Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>
            {
                {
                    "Id", new AttributeValue
                    {

                        N = id.ToString()
                    }
                },
                {
                    "LastName", new AttributeValue
                    {
                        S = LastName
                    }
                }
            };
            var request = new DeleteItemRequest
            {

                TableName = tableName,
                Key = key
            };
            return request;
        }

        private async Task<DeleteItemResponse> DeleteItemAsync(DeleteItemRequest request)
        {
            var response = await _dynamoDbClient.DeleteItemAsync(request);

            return response;
        }

        #endregion
    }
}
