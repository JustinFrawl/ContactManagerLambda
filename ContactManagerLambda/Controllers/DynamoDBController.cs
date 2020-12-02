using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactManagerLambda.DynamoDb;

namespace ContactManagerLambda.Controllers
{
    [Produces("application/json")]
    [Route("api/DynamoDb")]
    public class DynamoDbController : Controller
    {
        private readonly IMasterContacts _MasterContacts;
        private readonly IPhoneContact _PhoneContacts;
        private readonly IAddressContact _AddressContacts;
        private readonly ISecondaryEmail _SecondaryEmail;

        public DynamoDbController(IMasterContacts getMasterContacts, IPhoneContact phoneContacts, IAddressContact addressContacts, ISecondaryEmail secondaryEmail)
        {
            _MasterContacts = getMasterContacts;
            _PhoneContacts = phoneContacts;
            _AddressContacts = addressContacts;
            _SecondaryEmail = secondaryEmail;
        }

        #region MasterContact
        [Route("addMasterContact")]
        public IActionResult PutMasterContact([FromQuery] int id, string Name, string PrimaryEmail)
        {
            _MasterContacts.AddNewEntry(id, Name, PrimaryEmail);
            return Ok();
        }
        [HttpGet]
        [Route("getMasterContact")]
        public async Task<IActionResult> getMasterContact([FromQuery] int? Id)
        {
            var response = await _MasterContacts.getMasterContacts(Id);
            return Ok(response);
        }

        [HttpPut]
        [Route("updateMasterContact")]
        public async Task<IActionResult> updateMasterContact([FromQuery] int id, string PrimaryEmail, string LastName, string? newFirstName)
        {
            var response = await _MasterContacts.Update(id, PrimaryEmail, LastName, newFirstName);

            return Ok(response);
        }
        [Route("deleteMasterContact")]
        public IActionResult deleteMasterContact([FromQuery] int id, string LastName)
        {
            _MasterContacts.Delete(id, LastName);

            return Ok();
        }
        #endregion

        #region PhoneContact
        [Route("addPhoneContact")]
        public IActionResult PutPhoneContact([FromQuery] int id, int ContactId, string PhoneType, string PhoneNumber)
        {
            _PhoneContacts.AddNewEntry(id, ContactId, PhoneType, PhoneNumber);
            return Ok();
        }
        [HttpGet]
        [Route("getPhoneContact")]
        public async Task<IActionResult> getPhoneContact([FromQuery] int? Id)
        {
            var response = await _PhoneContacts.getPhoneContacts(Id);
            return Ok(response);
        }

        [HttpPut]
        [Route("updatePhoneContact")]
        public async Task<IActionResult> updatePhoneContact([FromQuery] int id, string PhoneNumber, string? newPhoneType)
        {
            var response = await _PhoneContacts.Update(id, PhoneNumber, newPhoneType);

            return Ok(response);
        }
        [Route("deletePhoneContact")]
        public IActionResult deletePhoneContact([FromQuery] int id)
        {
            _PhoneContacts.Delete(id);

            return Ok();
        }
        #endregion

        #region AddressContact
        [Route("addAddressContact")]
        public IActionResult PutAddressContact([FromQuery] int id, int ContactId, string Street, string City, string adrState, string Zip)
        {
            _AddressContacts.AddNewEntry(id, ContactId, Street, City, adrState, Zip);
            return Ok();
        }
        [HttpGet]
        [Route("getAddressContact")]
        public async Task<IActionResult> getAddressContact([FromQuery] int? Id)
        {
            var response = await _AddressContacts.getAddressContacts(Id);
            return Ok(response);
        }

        [HttpPut]
        [Route("updateAddressContact")]
        public async Task<IActionResult> updateAddressContact([FromQuery] int Id, string Street, string? City, string? adrState, string? Zip)
        {
            var response = await _AddressContacts.Update(Id, Street, City, adrState, Zip);

            return Ok(response);
        }
        [Route("deleteAddressContact")]
        public IActionResult deleteAddressContact([FromQuery] int id)
        {
            _AddressContacts.Delete(id);

            return Ok();
        }
        #endregion

        #region SecondaryEmail
        [Route("addSecondaryEmail")]
        public IActionResult PutSecondaryEmail([FromQuery] int id, int ContactId, string Email)
        {
            _SecondaryEmail.AddNewEntry(id, ContactId, Email);
            return Ok();
        }
        [HttpGet]
        [Route("getSecondaryEmail")]
        public async Task<IActionResult> getSecondaryEmail([FromQuery] int? Id)
        {
            var response = await _SecondaryEmail.getSecondaryEmails(Id);
            return Ok(response);
        }

        [HttpPut]
        [Route("updateSecondaryEmail")]
        public async Task<IActionResult> updateSecondaryEmail([FromQuery] int Id, string Email)
        {
            var response = await _SecondaryEmail.Update(Id, Email);

            return Ok(response);
        }
        [Route("deleteSecondaryEmail")]
        public IActionResult deleteSecondaryEmail([FromQuery] int id)
        {
            _SecondaryEmail.Delete(id);

            return Ok();
        }
        #endregion
    }
}
