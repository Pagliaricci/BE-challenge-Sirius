using Microsoft.AspNetCore.Mvc;
using EmailService.Modules.Users.Models;
using EmailService.Modules.Users.Services;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailService.Modules.Users.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Returns all users.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<UserDTO>>> GetAllUsers()
        {
            return await _userService.GetAllUsersAsync();
        }


        /// <summary>
        /// Returns a user by ID.
        /// </summary>
        /// <param name="id">Should include the user's id</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            return user;
        }

        /// <summary>
        /// Lets a user register.
        /// </summary>
        /// <param name="user">Should have a username,password,role and email </param>
        [HttpPost("register")]
        public async Task<ActionResult<User>> CreateUser([FromBody] CreateUser user)
        {
            var newUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
        }

        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        /// <param name="id">Should include the user's id.</param>
        /// 
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}