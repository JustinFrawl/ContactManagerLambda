using System;
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
    public class SecondaryEmail : ISecondaryEmail
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private static readonly string tableName = "Secondary_Email";

        public SecondaryEmail(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        #region C reate Logic
        public async Task AddNewEntry(int id, int ContactId, string Email)
        {
            var queryRequest = PutRequestBuilder(id, ContactId, Email);

            await PutitemAsync(queryRequest);
        }

        private PutItemRequest PutRequestBuilder(int id, int ContactId, string Email)
        {
            var item = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue{ N = id.ToString()} },
                { "ContactId", new AttributeValue{ N = ContactId.ToString()} },
                { "Email", new AttributeValue {S = Email } }
            };

            return new PutItemRequest
            {
                TableName = tableName,
                Item = item
            };
        }

        private async Task PutitemAsync(PutItemRequest request)
        {
            await _dynamoDbClient.PutItemAsync(request);
        }
        #endregion
        #region R ead Logic
        public async Task<DynamoDB.libs.Models.DynamoTableItems> getSecondaryEmails(int? Id)
        {
            var queryRequest = GetRequestBuilder(Id, false);

            var result = await ScanAsync(queryRequest);

            return new DynamoTableItems
            {
                SecondaryEmails = result.Items.Select(Map).ToList()
            };
        }

        public async Task<List<SecondaryEmailModel>> getMasterSecondaryEmails(int? Id, bool isContactId)
        {
            var queryRequest = GetRequestBuilder(Id, isContactId);

            var result = await ScanAsync(queryRequest);

            return result.Items.Select(Map).ToList();

        }

        private SecondaryEmailModel Map(Dictionary<string, AttributeValue> result)
        {
            return new SecondaryEmailModel
            {
                Id = Convert.ToInt32(result["Id"].N),
                ContactId = Convert.ToInt32(result["ContactId"].N),
                Email = result["Email"].S
            };
        }

        private async Task<ScanResponse> ScanAsync(ScanRequest queryRequest)
        {
            var response = await _dynamoDbClient.ScanAsync(queryRequest);

            return response;
        }

        private ScanRequest GetRequestBuilder(int? id, bool isContactId)
        {
            if (!id.HasValue)
            {
                return new ScanRequest
                {
                    TableName = tableName
                };
            }
            else if (isContactId)
            {
                return new ScanRequest
                {
                    TableName = tableName,
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":v_Id", new AttributeValue{N = id.ToString()} }
                },
                    FilterExpression = "ContactId = :v_Id",
                    ProjectionExpression = "Id, ContactId, Email"

                };
            }
            else
            {
                return new ScanRequest
                {
                    TableName = tableName,
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":v_Id", new AttributeValue{N = id.ToString()} }
                },
                    FilterExpression = "Id = :v_Id",
                    ProjectionExpression = "Id, ContactId, Email"

                };
            }
            
        }
        #endregion
        #region U pdate Logic
        public async Task<SecondaryEmailModel> Update(int Id, string Email)
        {
            var response = await getSecondaryEmails(Id);

            var currentEmail = response.SecondaryEmails.Select(p => p.Email).FirstOrDefault();

            var request = UpdateRequestBuilder(Id, Email, currentEmail);

            var result = await UpdateItemAsync(request);

            return new SecondaryEmailModel
            {
                Id = Convert.ToInt32(result.Attributes["Id"].N),
                ContactId = Convert.ToInt32(result.Attributes["ContactId"].N),
                Email = result.Attributes["Email"].S
            };
        }

        private UpdateItemRequest UpdateRequestBuilder(int id, string Email, string currentEmail)
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
                    }
                },
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    {"#E", "Email"}
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {
                        ":newEmail", new AttributeValue
                        {
                            S = Email
                        }
                    },
                    {
                        ":currEmail", new AttributeValue
                        {
                            S = currentEmail
                        }
                    }
                },

                UpdateExpression = "SET #E = :newEmail",
                ConditionExpression = "#E = :currEmail",

                TableName = tableName,
                ReturnValues = "ALL_NEW"
            };
            
            return request;

        }

        private async Task<UpdateItemResponse> UpdateItemAsync(UpdateItemRequest request)
        {
            var response = await _dynamoDbClient.UpdateItemAsync(request);

            return response;
        }
        #endregion
        #region D elete Logic
        public async Task Delete(int id)
        {
            var queryRequest = DeleteRequestBuilder(id);

            await DeleteItemAsync(queryRequest);
        }
        private DeleteItemRequest DeleteRequestBuilder(int id)
        {
            Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>
            {
                {
                    "Id", new AttributeValue
                    {

                        N = id.ToString()
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
