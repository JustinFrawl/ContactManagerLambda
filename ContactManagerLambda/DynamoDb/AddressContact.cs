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
    public class AddressContact : IAddressContact
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private static readonly string tableName = "Contact_Address";

        public AddressContact(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        #region C reate Logic
        public async Task AddNewEntry(int id, int ContactId, string Street, string City, string adrState, string Zip)
        {
            var queryRequest = PutRequestBuilder(id, ContactId, Street, City, adrState, Zip);

            await PutitemAsync(queryRequest);
        }

        private PutItemRequest PutRequestBuilder(int id, int ContactId, string Street, string City, string adrState, string Zip)
        {
            var item = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue{ N = id.ToString()} },
                { "ContactId", new AttributeValue{ N = id.ToString()} },
                { "Street", new AttributeValue {S = Street } },
                { "City", new AttributeValue {S = City } },
                { "adrState", new AttributeValue {S = adrState } },
                { "Zip", new AttributeValue {S = Zip } }
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
        public async Task<DynamoDB.libs.Models.DynamoTableItems> getAddressContacts(int? Id)
        {
            var queryRequest = GetRequestBuilder(Id, false);

            var result = await ScanAsync(queryRequest);

            return new DynamoTableItems
            {
                AddressContacts = result.Items.Select(Map).ToList()
            };
        }

        public async Task<List<AddressModel>> getMasterAddressContacts(int? Id, bool isContactId)
        {
            var queryRequest = GetRequestBuilder(Id, isContactId);

            var result = await ScanAsync(queryRequest);

            return result.Items.Select(Map).ToList();
            
        }

        private AddressModel Map(Dictionary<string, AttributeValue> result)
        {
            //string street2 = "";
            //string city2 = "";
            //string adrState2 = "";
            //string Zip2 = "";
            //This is an example of me trying to figure out how to best handle null values, use as a talking point and try to work it out on the phone to get across my thought process better?


            return new AddressModel
            {
                Id = Convert.ToInt32(result["Id"].N),
                ContactId = Convert.ToInt32(result["ContactId"].N),
                Street = result["Street"].S,
                City = result["City"].S,
                adrState = result["adrState"].S,
                Zip = result["Zip"].S
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
                    ProjectionExpression = "Id, ContactId, Street, City, adrState, Zip"

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
                    ProjectionExpression = "Id, ContactId, Street, City, adrState, Zip"

                };
            }
            
        }
        #endregion
        #region U pdate Logic
        public async Task<AddressModel> Update(int Id, string Street, string? City, string? adrState, string? Zip)
        {
            var response = await getAddressContacts(Id);

            string currentStreet = response.AddressContacts.Select(p => p.Street).FirstOrDefault();
            string currentCity = "";
            if (!String.IsNullOrEmpty(City))
            {
                currentCity = response.AddressContacts.Select(p => p.City).FirstOrDefault();
            }
            string currentadrState = "";
            if (!String.IsNullOrEmpty(adrState))
            {
                currentadrState = response.AddressContacts.Select(p => p.adrState).FirstOrDefault();
            }

            string currentZip = "";
            if (!String.IsNullOrEmpty(Zip))
            {
                currentZip = response.AddressContacts.Select(p => p.Zip).FirstOrDefault();
            }

            var request = UpdateRequestBuilder(Id, Street, currentStreet, currentCity, City, currentadrState, adrState, currentZip, Zip);

            var result = await UpdateItemAsync(request);

            return new AddressModel
            {
                Id = Convert.ToInt32(result.Attributes["Id"].N),
                ContactId = Convert.ToInt32(result.Attributes["ContactId"].N),
                Street = result.Attributes["Street"].S,
                City = result.Attributes["City"].S,
                adrState = result.Attributes["adrState"].S,
                Zip = result.Attributes["Zip"].S
            };
        }


        private UpdateItemRequest UpdateRequestBuilder(int id, string Street, string currentStreet, string currentCity, string? newCity, string currentadrState, string? newadrState, string currentZip, string? newZip)
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
                    {"#S", "Street"}
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {
                        ":newStreet", new AttributeValue
                        {
                            S = Street
                        }
                    },
                    {
                        ":currStreet", new AttributeValue
                        {
                            S = currentStreet
                        }
                    }
                },

                UpdateExpression = "SET #S = :newStreet",
                ConditionExpression = "#S = :currStreet",

                TableName = tableName,
                ReturnValues = "ALL_NEW"
            };
            if (!String.IsNullOrEmpty(newCity))
            {
                request.ExpressionAttributeNames.Add("#C", "City");
                request.ExpressionAttributeValues.Add(":newCity", new AttributeValue { S = newCity });
                request.UpdateExpression = request.UpdateExpression + ", #C = :newCity";

            }
            if (!String.IsNullOrEmpty(newadrState))
            {
                request.ExpressionAttributeNames.Add("#ST", "adrState");
                request.ExpressionAttributeValues.Add(":newadrState", new AttributeValue { S = newadrState });
                request.UpdateExpression = request.UpdateExpression + ", #ST = :newadrState";

            }
            if (!String.IsNullOrEmpty(newZip))
            {
                request.ExpressionAttributeNames.Add("#Z", "Zip");
                request.ExpressionAttributeValues.Add(":newZip", new AttributeValue { S = newZip });
                request.UpdateExpression = request.UpdateExpression + ", #Z = :newZip";

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
