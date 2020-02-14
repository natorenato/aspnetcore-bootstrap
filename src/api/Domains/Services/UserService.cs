using System;
using System.Linq;
using System.Threading.Tasks;
using API.Domains.Models;
using API.Domains.Models.Faults;
using API.Domains.Queries;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace API.Domains.Services
{
    public interface IUserService
    {
        Task<Pagination<User>> ListAsync(int offset, int limit);
        Task<User> GetAsync(int id);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(int id, User user);
        Task DeleteAsync(int id);
        Task ActivateDeactivateAsync(int id, bool active);
    }

    public class UserService : IUserService
    {
        private readonly SqlService _sqlService;
        private readonly IValidator<User> _userValidator; //model validation
        private readonly IValidationService _validationService; //if something goes unexpected on the business validations
        private readonly IAuthenticatedService _authenticatedService; //autentication validator (login) (CreatedBy column)
        private readonly ILogger<UserService> _logger;
        private readonly IContactService _contactService;
        
        public UserService(
             IValidator<User> userValidator,
             SqlService sqlService,
             IValidationService validationService,
             IAuthenticatedService authenticatedService,
             ILogger<UserService> logger,
             IContactService contactService
             )
        {
            _userValidator = userValidator;
            _sqlService = sqlService;
            _validationService = validationService;
            _authenticatedService = authenticatedService;
            _logger = logger;
            _contactService = contactService;
        }

        public async Task<Pagination<User>> ListAsync(int offset, int limit)
        {
            this._logger.LogDebug("Starting ListAsync");

            this._logger.LogDebug("Validating pagination parameters");

            if (limit - offset > 100) 
            {
                this._validationService.Throw("Limit", "Too much items for pagination", limit, Validation.PaginationExceedsLimits);
            }

            this._logger.LogDebug("Retriving paginated list of users");

            var users = await _sqlService.ListAsync<User>(UserQuery.PAGINATE, new 
            {
                Limit = limit,
                CreatedBy = "Renato",
                Offset = offset
            });

            foreach (var user in users)
            {
                user.Contacts = await this._contactService.ListAsync(user.UserId);
            }

            this._logger.LogDebug("Retriving the number of users registered by the authenticated user");

            var total = await _sqlService.CountAsync(UserQuery.TOTAL, new 
            {
                CreatedBy = "Renato",
            });

            this._logger.LogDebug("Retriving the number of users registered by the authenticated user");

            var pagination = new Pagination<User>() 
            {
                Offset = offset,
                Limit = limit,
                Items = users,
                Total = total
            };

            this._logger.LogDebug("Ending ListAsync");

            return pagination;
        }
        
        public async Task<User> GetAsync(int id)
        {
            this._logger.LogDebug("Starting GetAsync");

            this._logger.LogDebug("Retriving a user");

            var user = await _sqlService.GetAsync<User>(UserQuery.GET, new 
            {
                Id = id,
                CreatedBy = "Renato"
            });

            this._logger.LogDebug("Checking if user exists");

            if (user == null)
            {
                this._logger.LogDebug("User does not exists, triggering 404");

                throw new ArgumentNullException(nameof(id));
            }

            this._logger.LogDebug("Retriving the user contacts");

            var contacts = await _contactService.ListAsync(id);
            user.Contacts = contacts.ToList();

            this._logger.LogDebug("Ending GetAsync");

            return user;
        }

        public async Task<User> CreateAsync(User user)
        {
            this._logger.LogDebug("Starting CreateAsync");

            this._logger.LogDebug("Validating payload");

            await _userValidator.ValidateAndThrowAsync(user);

            this._logger.LogDebug("Checking if that document already exists");

            var existsDocument = await _sqlService.ExistsAsync(UserQuery.EXISTS_DOCUMENT, new 
            {
                Document = user.Document
            });

            if (existsDocument) 
            {
                this._logger.LogDebug("Document already exists, triggering 400");

                this._validationService.Throw("Document", "There is already another user with that document", user.Document, Validation.UserRepeatedDocument);
            }

            this._logger.LogDebug("Inserting new user");

            user.UserId = await _sqlService.CreateAsync(UserQuery.INSERT, new 
            {
                IdProfile = user.Profile,
                IdCountry = user.Country,
                CreatedBy = "Renato",
                Name = user.Name,
                Document = user.Document,
                Birthdate = user.Birthdate,
                Active = user.Active
            });

            this._logger.LogDebug("Checking if there is any email to insert");


            if (user.Contacts.Count() > 0) {

                this._logger.LogDebug("Passing the email to be inserted");

                foreach (var contact in user.Contacts)
                {
                    contact.UserId = user.UserId;
                    await this._contactService.CreateAsync(contact);
                }
            }

            this._logger.LogDebug("Ending CreateAsync");

            return user;
        }

        public async Task<User> UpdateAsync(int id, User user)
        {
            this._logger.LogDebug("Starting UpdateAsync");

            this._logger.LogDebug("Validating payload");

            await _userValidator.ValidateAndThrowAsync(user);

            this._logger.LogDebug("Retriving the user the user wants to update");

            var oldUser = await GetAsync(id);

            this._logger.LogDebug("Updating user");

            await _sqlService.ExecuteAsync(UserQuery.UPDATE, new 
            {
                Id = oldUser.UserId,
                IdProfile = user.Profile,
                IdCountry = user.Country,
                Name = user.Name,
                Birthdate = user.Birthdate,
                Active = user.Active
            });

            user.UserId = oldUser.UserId;

            this._logger.LogDebug("Ending UpdateAsync");

            return user;
        }

        public async Task DeleteAsync(int id)
        {
            this._logger.LogDebug("Starting DeleteAsync");

            this._logger.LogDebug("Retriving the user the user wants to delete");

            var user = await GetAsync(id);

            this._logger.LogDebug("Deleting user");

            await _sqlService.ExecuteAsync(UserQuery.DELETE, new 
            {
                Id = user.UserId,
                CreatedBy = "Renato"
            });

            this._logger.LogDebug("Ending DeleteAsync");
        }

        public async Task ActivateDeactivateAsync(int id, bool active)
        {
            this._logger.LogDebug("Starting ActivateDeactivateAsync");

            this._logger.LogDebug("Retriving the user the user wants to activate/deactivate");

            var user = await GetAsync(id);

            this._logger.LogDebug("Activating or deactivating user");

            await _sqlService.ExecuteAsync(UserQuery.ACTIVATE_DEACTIVATE, new 
            {
                Id = user.UserId,
                Active = active,
                CreatedBy = "Renato"
            });

            this._logger.LogDebug("Ending ActivateDeactivateAsync");
        }
    }
}
