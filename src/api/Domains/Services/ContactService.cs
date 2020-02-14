using API.Domains.Models;
using API.Domains.Models.Faults;
using API.Domains.Queries;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Domains.Services
{
    public interface IContactService
    {
        Task<IEnumerable<Contact>> ListAsync(int id);
        Task<Contact> GetAsync(int id);
        Task<Contact> CreateAsync(Contact contact);
        Task<Contact> UpdateAsync(int id, Contact contact);
        Task DeleteAsync(int id);
    }
    public class ContactService : IContactService
    {
        private readonly ILogger<ContactService> _logger;
        private readonly SqlService _sqlService;
        private readonly IValidator<Contact> _contactValidator; //model validation
        private readonly IValidationService _validationService; //if something goes unexpected on the business validations

        public ContactService(
            ILogger<ContactService> logger,
            SqlService sqlService,
            IValidator<Contact> contactValidator,
            IValidationService validationService
            )
        {
            _logger = logger;
            _sqlService = sqlService;
            _contactValidator = contactValidator;
            _validationService = validationService;
        }

        public async Task<IEnumerable<Contact>> ListAsync(int idUsuario)
        {
            this._logger.LogDebug("Starting GetAsync");

            this._logger.LogDebug("Retriving the list of Contacts");

            var contacts = await _sqlService.ListAsync<Contact>(ContactQuery.LIST_BY_USERID, new
            {
                UserId = idUsuario
            });

            this._logger.LogDebug("Checking if has any contact");

            this._logger.LogDebug("Ending GetAsync");

            return contacts;
        }

        public async Task<Contact> GetAsync(int id)
        {
            this._logger.LogDebug("Start GetAsync");

            this._logger.LogDebug("Retrieving Contact");

            var contact = await this._sqlService.GetAsync<Contact>(ContactQuery.GET,new
            {
                Id = id
            });

            this._logger.LogDebug("Checking if email exists");

            if(contact == null)
            {
                this._logger.LogDebug("Contact does not exists, triggering 404");
                throw new ArgumentNullException(nameof(id));
            }

            this._logger.LogDebug("Ending GetAsync");

            return contact;
        }

        public async Task<Contact> CreateAsync(Contact contact)
        {
            this._logger.LogDebug("Start CreateAsync");

            this._logger.LogDebug("Validating Email");

            await this._contactValidator.ValidateAndThrowAsync(contact);

            this._logger.LogDebug("Checking if the email already exists");

            var existsEmail = await this._sqlService.ExistsAsync(ContactQuery.EXISTS_EMAIL, new
            {
                Email = contact.Email
            });

            if (existsEmail)
            {
                this._logger.LogDebug("Email already exists, triggering 400");
                this._validationService.Throw("Email", "There is already another user with that email", contact.Email, Validation.UserRepeatedEmail);
            }
            
            this._logger.LogDebug("Inserting new email");

            contact.ContactId = await this._sqlService.CreateAsync(ContactQuery.INSERT, new
            {
                Email = contact.Email,
                UserId = contact.UserId
            });

            return contact;
        }

        public async Task<Contact> UpdateAsync(int id, Contact contact)
        {
            this._logger.LogDebug("Starting UpdateAsync");

            this._logger.LogDebug("Validating Email");

            await this._contactValidator.ValidateAndThrowAsync(contact);

            this._logger.LogDebug("Retrieving the contact to update it");

            var oldContact = await this.GetAsync(id);

            this._logger.LogDebug("Updating contact");

            await this._sqlService.ExecuteAsync(ContactQuery.UPDATE, new {
                Id = oldContact.ContactId,
                Email = contact.ContactId
            });

            contact.ContactId = oldContact.ContactId;

            this._logger.LogDebug("Ending UpdateAsync");

            return contact;
        }

        public async Task DeleteAsync(int id)
        {
            this._logger.LogDebug("Starting DeleteAsync");

            this._logger.LogDebug("Retriving the contact to delete it");

            var contact = await GetAsync(id);

            this._logger.LogDebug("Deleting contact");

            await _sqlService.ExecuteAsync(ContactQuery.DELETE, new
            {
                Id = contact.ContactId
            });

            this._logger.LogDebug("Ending DeleteAsync");
        }
    }
}