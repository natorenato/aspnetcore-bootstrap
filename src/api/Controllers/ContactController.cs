using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Domains.Models;
using API.Domains.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [Route("contacts")]
    [ApiController]
    [SwaggerTag("Create, edit, delete and retrieve contacts")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;
      
        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }


        [HttpGet("{userID}")]
        [SwaggerOperation
            (
            Summary = "Retrieve a list of contacts by the userID",
            Description = "Retrieves the list only if it has something"
        )]
        [SwaggerResponse(200, "A list filtered by userID", typeof(List<Contact>))]
        public async Task<ActionResult> List(
            [SwaggerParameter("Users ID")]int userID)
        {
            var contacts = await _contactService.ListAsync(userID);

            return Ok(contacts);
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Creates a new contact",
            Description = "Creates a new contact if all validations are succeded"
        )]
        [SwaggerResponse(201, "The contact was successfully created", typeof(Contact))]
        public async Task<ActionResult> Post(
            [FromBody] Contact contact)
        {
            var created = await _contactService.CreateAsync(contact);

            return CreatedAtAction(nameof(Post), new { id = contact.UserId }, created);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Edits an existing contact by their ID",
            Description = "Edits an existing contact if all validations are succeded and were created by the authenticated user"
        )]
        [SwaggerResponse(200, "The contact was successfully edited", typeof(Contact))]
        public async Task<ActionResult> Put(
            [SwaggerParameter("Contact's ID")] int id,
            [FromBody] Contact contact)
        {
            var edited = await _contactService.UpdateAsync(id, contact);

            return Ok(edited);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(
           Summary = "Deletes a contact by their ID",
           Description = "Deletes a contact if that contact is deletable and were created by the authenticated user"
       )]
        [SwaggerResponse(204, "The contact was successfully deleted")]
        public async Task<ActionResult> Delete(
           [SwaggerParameter("contact's ID")]int id)
        {
            await _contactService.DeleteAsync(id);

            return Ok();
        }
    }
}