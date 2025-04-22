using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Users.Api.Models;
using Users.Domain.Exceptions;
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
        public async Task<ActionResult> Create(UserCreateDto userDto)
        {
            _logger?.LogInformation("Received request to create a new user: {@UserDto}", userDto);
            User user = _mapper.Map<User>(userDto);

            try
            {
                await _service.Create(user);
                _logger?.LogInformation("User created successfully: {@User}", user);
                return Created();
            }
            catch (UserAlreadyExistsException exception)
            {
                _logger?.LogWarning(exception, "User already exists: {@UserDto}", userDto);
                return Conflict(exception.Message);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, "An error occurred while creating a user: {@UserDto}", userDto);
                return BadRequest(exception.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserGetDto>> Get(Guid id)
        {
            _logger?.LogInformation("Received request to get user with ID: {UserId}", id);

            try
            {
                User user = await _service.Get(id);
                UserGetDto result = _mapper.Map<UserGetDto>(user);
                _logger?.LogInformation("User retrieved successfully: {@User}", result);
                return Ok(result);
            }
            catch (UserNotFoundException exception)
            {
                _logger?.LogWarning(exception, "User not found with ID: {UserId}", id);
                return NotFound(exception.Message);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, "An error occurred while retrieving user with ID: {UserId}", id);
                return BadRequest(exception.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserGetDto>>> GetAll()
        {
            _logger?.LogInformation("Received request to get all users");

            try
            {
                var users = await _service.GetAll();
                var result = _mapper.Map<IEnumerable<UserGetDto>>(users);
                
                _logger?.LogInformation("Successfully retrieved all users");
                return Ok(result);
            }
            catch (Exception ex) 
            {
                return HandleGenericException(ex, "An error occurred while retrieving all users");
            }
        }

        [HttpPut]
        public async Task<ActionResult<UserUpdateDto>> Update(UserUpdateDto userDto)
        {
            _logger?.LogInformation("Received request to update user: {@UserDto}", userDto);
            
            try
            {
                var user = _mapper.Map<User>(userDto);
                var updatedUser = await _service.Update(user);
                var result = _mapper.Map<UserUpdateDto>(updatedUser);
                
                _logger?.LogInformation("User updated successfully: {@User}", result);
                return Ok(result);
            }
            catch (UserNotFoundException ex)
            {
                return HandleUserNotFoundException(ex, userDto);
            }
            catch (Exception ex) 
            {
                return HandleGenericException(ex, $"An error occurred while updating user: {userDto}");
            }
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(Guid id)
        {
            _logger?.LogInformation("Received request to delete user with ID: {UserId}", id);

            try
            {
                await _service.Delete(id);
                
                _logger?.LogInformation("User deleted successfully with ID: {UserId}", id);
                return NoContent();
            }
            catch (UserNotFoundException ex)
            {
                return HandleUserNotFoundException(ex, id);
            }
            catch (Exception ex)
            {
                return HandleGenericException(ex, $"An error occurred while deleting user with ID: {id}");
            }
        }

        #region Exception Handlers
        
        private ActionResult HandleUserNotFoundException(UserNotFoundException ex, object context)
        {
            _logger?.LogWarning(ex, "User not found: {@Context}", context);
            return NotFound(ex.Message);
        }

        private ActionResult HandleUserAlreadyExistsException(UserAlreadyExistsException ex, object context)
        {
            _logger?.LogWarning(ex, "User already exists: {@Context}", context);
            return Conflict(ex.Message);
        }

        private ActionResult HandleGenericException(Exception ex, string message)
        {
            _logger?.LogError(ex, message);
            return BadRequest(ex.Message);
        }
        
        #endregion
    }
}