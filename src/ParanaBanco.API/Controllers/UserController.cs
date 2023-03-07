using Microsoft.AspNetCore.Mvc;
using ParanaBanco.Application.DTOs;
using ParanaBanco.Application.Interfaces.Services;
using ParanaBanco.Domain.Exceptions;

namespace ParanaBanco.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAll()
        {
            var users = await _userService.GetAll(false);
            if (users == null || !users.Any())
                return NotFound("User(s) not found");

            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var produto = await _userService.GetById(id, true);

            if (produto == null)
                return NotFound("User not found");

            return Ok(produto);
        }

        [HttpGet("{includeDeleted:bool}")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllIncludeDeleted(bool includeDeleted = true)
        {
            var users = await _userService.GetAll(includeDeleted);
            if (users == null || !users.Any())
                return NotFound("User(s) not found");

            return Ok(users);
        }

        [HttpGet("onlyDeleted")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetOnlyDeleted()
        {
            var users = await _userService.GetOnlyDeleted();
            if (users == null || !users.Any())
                return NotFound("User(s) not found");

            return Ok(users);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UserDTO userDTO)
        {
            try
            {
                var id = await _userService.Add(userDTO);

                if (id == 0)
                    return BadRequest("Add error.");

                userDTO.Id = id;

                return CreatedAtAction(nameof(GetAll), new { id }, userDTO);
            }
            catch (EmailAlreadyRegisteredException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserDTO userDTO)
        {
            try
            {
                if (id != userDTO.Id)
                    return BadRequest();

                await _userService.Update(userDTO);

                return NoContent();
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (EmailAlreadyRegisteredException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var user = await _userService.GetById(id, false);
            if (user == null)
                return NotFound("User not found");

            await _userService.Delete(user);

            return NoContent();
        }

        [HttpPut("{id:int}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var user = await _userService.GetById(id, true);

            if (user == null)
                return NotFound("User not found");

            await _userService.Restore(user);

            return NoContent();
        }

        [HttpGet("{email}")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetByEmail(string email)
        {
            var user = await _userService.GetByEmail(email);
            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        [HttpDelete("{email}")]
        public async Task<ActionResult> Delete(string email)
        {
            var user = await _userService.GetByEmail(email);
            if (user == null)
                return NotFound("User not found");

            await _userService.Delete(user);

            return NoContent();
        }
    }
}
