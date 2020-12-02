# ContactManagerLambda

Notes for this Web API.

This is a serverless web api using DynamoDB with methods published to amazon Lambda.

GATEWAY:  https://rt70k5d59a.execute-api.us-east-1.amazonaws.com/Prod

The Table Structure is as follows:

Contact_Master
Id(Number): This is the primary ID and is used on the other three tables to dictate ownership
LastName(String): Fairly straightforward I think.
FirstName(String): See above
Email(String): The Primary Email for this contact.

Secondary_Email
Id(Number): Table specific Id for use in getting specific records
ContactId(Number): This is the foreignKey used to show which contact this secondary email belongs to.
Email(String): The secondary or additional email that a contact might have.

Contact_Address
Id(Number): Table specific Id for use in getting specific records
ContactId(Number): This is the foreignKey used to show which contact this address belongs to.
Street(String): The street of this address
City(String): The City of this Address
adrState(String): The State in this address, had to break tablename conventions when in testing calls I discovered State is a reserved word, hindsight probably should have predicted that.
Zip(String): Zip code of an address

Contact_Phone
Id(Number): Table specific Id for use in getting specific records
ContactId(Number): This is the foreignKey used to show which contact this Phone Number belongs to.
PhoneType(String): Field which holds what type of phone number this is (EX Cell or home or work), also originally encountered reserved use for this initially.
PhoneNumber(String): The phone number for this row entry


*****Structure*****
Each of these four tables has a method class that inherits from an interface and a model as well.  The naming convention is:
Method Classes: tablename.cs
Interface Class: Itablename.cs
Model: tablenameModel.cs

I use the interfaces to better handle instantiation into the DynamoDB controller, more easily use constructor instantiation, as well as adding them as a service in the startup.cs


Methods:

Each table has a method that corresponds to CRUD.  However the three child tables all have one additional method that allows the Master_Contact table to gather from them. Below I will list all of the methods, provide an explanation if neccesary and provide the route to hit these methods with a db call.

Contact Master
[Route("addMasterContact")](int id, string Name, string PrimaryEmail)
**I always like to look for ease of use scenarios where possible. I'm sure clients would rather type a whole name or copy and paste into one text box as opposed to break them up.  This method allows the code to break them up into First and Last.

[Route("getMasterContact")](int? Id)
**The nullable referance here is so that if Id is not provided, the method will gather ALL Master Contact records.  This is simply to avoid having to write a second method.  This method actually gathers from all tables on the db, so a call with an ID will effectively gather all information about that contact, including address, secondary email, and phones.
*This required the creation of a fifth method that simply collected the items after mapping them from each of the three child rows.  All four models are instantiated in a parent model called DynamoTableItems so that they can be represented together in this call.

[HttpPut][Route("updateMasterContact")](int id, string PrimaryEmail, string LastName, string? newFirstName)
**Allows you to update the email and FirstName if you add a string for that.  Originally I intended to do the same for LastName but I inadvertantly set that as the Range column and was unable to alter it without deleting the table.  For the purposes of this project I decided to leave it as a talking point about the learning process.

[Route("deleteMasterContact")](int id, string LastName)
**Deletes a master contact, pretty straightforward.  I liked the use case of having to add the last name as it prevents some confusion when sending a delete request.

Contact Phone

[Route("addPhoneContact")](int id, int ContactId, string PhoneType, string PhoneNumber)
**Very Standard create here

[Route("getPhoneContact")](int? Id)

[HttpPut][Route("updatePhoneContact")](int id, string PhoneNumber, string? newPhoneType)
**You can change the PhoneType if you want, but any call must change the number.  It's the first thing I would change revisiting this if it were production, but honestly some part of me realizes that a front end can easily auto include the same number and the user would never realize the obligatory aspect of this.  That being said it's not ideal and unneccesary server work.

[Route("deletePhoneContact")](int id)
**No range column here, just use a single id.

Contact Address

[Route("addAddressContact")](int id, int ContactId, string Street, string City, string adrState, string Zip)
**Straightforward I think

[Route("getAddressContact")](int? Id)
**Provide an ID to get a specific record, or don't to get all records

[HttpPut][Route("updateAddressContact")](int Id, string Street, string? City, string? adrState, string? Zip)
**ID of the row and the Street must be included but otherwise any value but ID can be changed in this row in this method.

[Route("deleteAddressContact")](int id)
**nothing new here

Secondary Email

[Route("addSecondaryEmail")](int id, int ContactId, string Email)
**Nothing new here

[Route("getSecondaryEmail")](int? Id)
**Nothing new here

[HttpPut][Route("updateSecondaryEmail")](int Id, string Email)
**There are only 3 columns in this table, I don't want a user to be able to update the Id or ContactId, so it's just a call to change the emaill adress

[Route("deleteSecondaryEmail")](int id)
**Nothing new here


Things I could have done better:

Most of the things I am not happy about were left in here as potential conversation pieces as well as the fact that I first created a simpler dynamodb database from a follow along youtube tutorial and much of the resulting structure you see here is mimicking that to an extent.
I find there are three chief ways I learn to use new features in programming.  First, like everyone else I read documentation and watch tutorials online. Then I will use a build along if I can even if I may not learn as much going through it.  This is because where I find my knowledge really grows
is when I hit refactoring.  With a tutorial having given me some sense in how to handle things, I generally feel more comfortable tinkering, breaking(thanks stack overflow), and ultimately improving the code.

So things I see need for improvement and we can try and talk through some of them if you want.
Null handling: Several times I ran into errors because a field was missing or my type-case wasn't strong enough and I accidentally created new columns in the tables, these would usually break my code. I've looked at how I could handle nulls better, likely in the map method for each table class.  I think spinning off some kind of method to check every value in the row would likely be best, returning a collection of Tuples for each that can be referanced in Map() and used to correct the values to a more returnable state.

Secondary_Email & Contact_Phone: I realize that the less tables, the better in Dynamodb.  Seeing that I can certainly see how adding logic and combining the columns of these two tables would greatly improve that aspect.  Their various classes could be combined and extra methods added to just call for the secondary email or the phone using a type column, like PhoneType in Contact_phone. It's a really cool aspect coming from standard SQLServer background.

FINAL THOUGHTS:
MY only previous experience with building web APIs was with Rails, this was altogether a more straight forward way of doing it i think.  Not having to worry about building it in a stricter relational sense saves a lot of time and coding effort, those methods in rails were obviously annoying.  In the future I assume a lot of this work would be done through the CLI similar to rails but working on it programmatically certainly gave me a deeper understanding of it.
Also the AWS toolkit is surprisingly useful, I love visual studio but can admit how wonky and over the top it can be, this was not the case and the AWS console in browser is incredibly useful.  Simply publishing the lambdas through an option click was a lovely experience.
