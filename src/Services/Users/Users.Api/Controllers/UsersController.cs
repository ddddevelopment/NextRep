using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Users.Api.Models;
using Users.Domain.Models;
using Users.Domain.Services;

namespace Users.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<UsersController>? _logger;

        public UsersController(IUsersService service, IMapper mapper, ILogger<UsersController> logger = null)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> Create(UserCreateRequest userDto)
        {
            _logger?.LogInformation("Received request to create a new user: {@UserDto}", userDto);
            User user = _mapper.Map<User>(userDto);

            Result createResult = await _service.Create(user);

            if (createResult.IsSuccess)
            {
                _logger?.LogInformation("User created successfully: {@User}", user);
                return Created();
            }
            else
            {
                switch (createResult.Error?.Type)
                {
                    case ErrorType.Conflict:
                        {
                            _logger?.LogWarning(createResult.Error?.Message, "User already exists: {@UserDto}", userDto);
                            return Conflict(createResult.Error?.Message);
                        }
                    case ErrorType.Validation:
                        {
                            _logger?.LogWarning(createResult.Error?.Message, "Invalid user data: {@UserDto}", userDto);
                            return BadRequest(createResult.Error?.Message);
                        }
                    case ErrorType.Unknown:
                    default:
                        {
                            _logger?.LogError(createResult.Error?.Message, "An error occurred while creating a user: {@UserDto}", userDto);
                            return BadRequest(createResult.Error?.Message);
                        }
                }
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserGetResponse>> GetById(Guid id)
        {
            _logger?.LogInformation("Received request to get user with ID: {UserId}", id);

            Result<User> getResult = await _service.GetById(id);

            if (getResult.IsSuccess)
            {
                var response = _mapper.Map<UserGetResponse>(getResult.Value);
                _logger?.LogInformation("User retrieved successfully: {@User}", response);
                return Ok(response);
            }
            else
            {
                switch (getResult.Error?.Type)
                {
                    case ErrorType.NotFound:
                        {
                            _logger?.LogWarning("User not found with ID: {UserId}", id);
                            return NotFound();
                        }
                    case ErrorType.Validation:
                        {
                            _logger?.LogWarning(getResult.Error?.Message, "Invalid user data for get by id: {UserId}", id);
                            return BadRequest(getResult.Error?.Message);
                        }
                    case ErrorType.Unknown:
                    default:
                        {
                            _logger?.LogError(getResult.Error?.Message, "An error occurred while retrieving user with ID: {UserId}", id);
                            return BadRequest(getResult.Error?.Message);
                        }
                }
            }
        }

        [HttpGet("by-email/{email}")]
        public async Task<ActionResult<UserGetResponse>> GetByEmail(string email)
        {
            _logger?.LogInformation("Received request to get user with email: {Email}", email);


            Result<User> getResult = await _service.GetByEmail(email);

            if (getResult.IsSuccess)
            {
                UserGetResponse response = _mapper.Map<UserGetResponse>(getResult.Value);
                _logger?.LogInformation("User retrieved successfully: {@User}", response);
                return Ok(response);
            }
            else
            {
                switch (getResult.Error?.Type)
                {
                    case ErrorType.NotFound:
                        {
                            _logger?.LogWarning("User not found with email: {Email}", email);
                            return NotFound();
                        }
                    case ErrorType.Validation:
                        {
                            _logger?.LogWarning(getResult.Error?.Message, "Invalid user data for get by email: {Email}", email);
                            return BadRequest(getResult.Error?.Message);
                        }
                    case ErrorType.Unknown:
                    default:
                        {
                            _logger?.LogError(getResult.Error?.Message, "An error occurred while retrieving user with email: {Email}", email);
                            return BadRequest(getResult.Error?.Message);
                        }
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserGetResponse>>> GetAll()
        {
            _logger?.LogInformation("Received request to get all users");

            Result<IEnumerable<User>> getAllResult = await _service.GetAll();

            if (getAllResult.IsSuccess)
            {
                IEnumerable<UserGetResponse> response = _mapper.Map<IEnumerable<UserGetResponse>>(getAllResult.Value);
                _logger?.LogInformation("Successfully retrieved all users");
                return Ok(response);
            }
            else
            {
                switch (getAllResult.Error?.Type)
                {
                    case ErrorType.Validation:
                        {
                            _logger?.LogWarning(getAllResult.Error?.Message, "Invalid user data for get all");
                            return BadRequest(getAllResult.Error?.Message);
                        }
                    case ErrorType.Unknown:
                    default:
                        {
                            _logger?.LogError(getAllResult.Error?.Message, "An error occurred while retrieving all users");
                            return BadRequest(getAllResult.Error?.Message);
                        }
                }
            }
        }

        [HttpPut]
        public async Task<ActionResult> Update(UserUpdateDto userDto)
        {
            _logger?.LogInformation("Received request to update user: {@UserDto}", userDto);

            User user = _mapper.Map<User>(userDto);
            Result<User> updateResult = await _service.Update(user);

            if (updateResult.IsSuccess)
            {
                UserUpdateDto response = _mapper.Map<UserUpdateDto>(updateResult.Value);
                _logger?.LogInformation("User updated successfully: {@User}", response);
                return Ok(response);
            }
            else
            {
                switch (updateResult.Error?.Type)
                {
                    case ErrorType.Validation:
                        _logger?.LogWarning(updateResult.Error?.Message, "Invalid user data for update: {@UserDto}", userDto);
                        return BadRequest(updateResult.Error?.Message);
                    case ErrorType.NotFound:
                        {
                            _logger?.LogWarning("User not found for update: {@UserDto}", userDto);
                            return NotFound();
                        }
                    case ErrorType.Unknown:
                    default:
                        {
                            _logger?.LogError(updateResult.Error?.Message, "An error occurred while updating user: {@UserDto}", userDto);
                            return BadRequest(updateResult.Error?.Message);
                        }
                }
            }
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(Guid id)
        {
            _logger?.LogInformation("Received request to delete user with ID: {UserId}", id);

            Result deleteResult = await _service.Delete(id);
            if (deleteResult.IsSuccess)
            {
                _logger?.LogInformation("User deleted successfully with ID: {UserId}", id);
                return NoContent();
            }
            else
            {
                switch (deleteResult.Error?.Type)
                {
                    case ErrorType.NotFound:
                        {
                            _logger?.LogWarning("User not found for deletion with ID: {UserId}", id);
                            return NotFound();
                        }
                    case ErrorType.Validation:
                        {
                            _logger?.LogWarning(deleteResult.Error?.Message, "Invalid user data for delete: {UserId}", id);
                            return BadRequest(deleteResult.Error?.Message);
                        }
                    case ErrorType.Unknown:
                    default:
                        {
                            _logger?.LogError(deleteResult.Error?.Message, "An error occurred while deleting user with ID: {UserId}", id);
                            return BadRequest(deleteResult.Error?.Message);
                        }
                }
            }
        }
    }
}
