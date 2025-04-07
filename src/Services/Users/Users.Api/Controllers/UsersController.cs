using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
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

        public UsersController(IUsersService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> Create(UserCreateDto userDto)
        {
            User user = _mapper.Map<User>(userDto);

            try
            {
                await _service.Create(user);
                return Created();
            }
            catch (UserAlreadyExistsException exception)
            {
                return Conflict(exception.Message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserGetDto>> Get(Guid id)
        {
            try
            {
                User user = await _service.Get(id);
                UserGetDto result = _mapper.Map<UserGetDto>(user);
                return Ok(result);
            }
            catch (UserNotFoundException exception)
            {
                return NotFound(exception.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserGetDto>>> GetAll()
        {
            IEnumerable<User> users = await _service.GetAll();
            IEnumerable<UserGetDto> result = _mapper.Map<IEnumerable<UserGetDto>>(users);
            return Ok(result);
        }

        [HttpPut]
        public async Task<ActionResult<UserUpdateDto>> Update(UserUpdateDto userDto)
        {
            try
            {
                User user = _mapper.Map<User>(userDto);
                User updatedUser = await _service.Update(user);
                UserUpdateDto result = _mapper.Map<UserUpdateDto>(updatedUser);
                return Ok(result);
            }
            catch (UserNotFoundException exception) {
                return NotFound(exception.Message);
            }
        }
    }
}