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

    public class PhoneContact : IPhoneContact
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private static readonly string tableName = "Contact_Phone";

        public PhoneContact(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }
        #region C reate Logic
        public async Task AddNewEntry(int id, int ContactId, string PhoneType, string PhoneNumber)
        {
            var queryRequest = PutRequestBuilder(id, ContactId, PhoneType, PhoneNumber);

            await PutitemAsync(queryRequest);
        }

        private PutItemRequest PutRequestBuilder(int id, int ContactId, string PhoneType, string PhoneNumber)
        {
            var item = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue{ N = id.ToString()} },
                { "ContactId", new AttributeValue{ N = id.ToString()} },
                { "PhoneType", new AttributeValue {S = PhoneType } },
                { "PhoneNumber", new AttributeValue {S = PhoneNumber } }
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
        public async Task<DynamoDB.libs.Models.DynamoTableItems> getPhoneContacts(int? Id)
        {
            var queryRequest = GetRequestBuilder(Id, false);

            var result = await ScanAsync(queryRequest);

            return new DynamoTableItems
            {
                PhoneContacts = result.Items.Select(Map).ToList()
            };
        }

        public async Task<List<PhoneModel>> getMasterPhoneContacts(int? Id, bool isContactId)
        {
            var queryRequest = GetRequestBuilder(Id, isContactId);

            var result = await ScanAsync(queryRequest);

            return result.Items.Select(Map).ToList();

        }

        private PhoneModel Map(Dictionary<string, AttributeValue> result)
        {
            return new PhoneModel
            {
                Id = Convert.ToInt32(result["Id"].N),
                ContactId = Convert.ToInt32(result["ContactId"].N),
                PhoneType = result["PhoneType"].S,
                PhoneNumber = result["PhoneNumber"].S
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
                    ProjectionExpression = "Id, ContactId, PhoneType, PhoneNumber"
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
                    ProjectionExpression = "Id, PhoneType, PhoneNumber"

                };
            }
            
        }
        #endregion
        #region U pdate Logic
        public async Task<PhoneModel> Update(int Id, string PhoneNumber, string? newPhoneType)
        {
            var response = await getPhoneContacts(Id);

            string currentPhoneNumber = response.PhoneContacts.Select(p => p.PhoneNumber).FirstOrDefault();
            string currentPhoneType = "";
            if (!String.IsNullOrEmpty(newPhoneType))
            {
                currentPhoneType = response.PhoneContacts.Select(p => p.PhoneType).FirstOrDefault();
            }

            var request = UpdateRequestBuilder(Id, PhoneNumber, currentPhoneNumber, currentPhoneType, newPhoneType);

            var result = await UpdateItemAsync(request);

            return new PhoneModel
            {
                Id = Convert.ToInt32(result.Attributes["Id"].N),
                ContactId = Convert.ToInt32(result.Attributes["ContactId"].N),
                PhoneType = result.Attributes["PhoneType"].S,
                PhoneNumber = result.Attributes["PhoneNumber"].S
            };
        }
        

        private UpdateItemRequest UpdateRequestBuilder(int id, string PhoneNumber, string currentPhoneNumber, string currentPhoneType, string? newPhoneType)
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
                    {"#N", "PhoneNumber"}
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {
                        ":newNum", new AttributeValue
                        {
                            S = PhoneNumber
                        }
                    },
                    {
                        ":currNum", new AttributeValue
                        {
                            S = currentPhoneNumber
                        }
                    }
                },

                UpdateExpression = "SET #N = :newNum",
                ConditionExpression = "#N = :currNum",

                TableName = tableName,
                ReturnValues = "ALL_NEW"
            };
            if (!String.IsNullOrEmpty(newPhoneType))
            {
                request.ExpressionAttributeNames.Add("#PT", "PhoneType");
                request.ExpressionAttributeValues.Add(":newPhoneType", new AttributeValue { S = newPhoneType });
                request.UpdateExpression = "SET #N = :newNum, #PT = :newPhoneType";
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
