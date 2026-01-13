using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Authorization;
using UserManagement.Entity;
using UserManagement.Interfaces;
using UserManagement.Model;

namespace UserManagement.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("Paginate")]

        public async Task<IActionResult> Paginate([FromQuery] PagingParameter pagingParameter)
        {
            var result = await _userService.Paginate(pagingParameter);
            return new OkObjectResult(result);
        }

        [HttpGet("All")]

        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetUsers();
            return new OkObjectResult(result);
        }

        [HttpPost("Save")]

        public async Task<IActionResult> Save([FromBody] User user)
        {
            var result = await _userService.Save(user);
            return new OkObjectResult(result);
        }

        [HttpPost("Update")]

        public async Task<IActionResult> Update([FromBody] User user)
        {
            var result = await _userService.Update(user);
            return new OkObjectResult(result);
        }

        [HttpPost("UserProfileEdit")]

        public async Task<IActionResult> UserProfileEdit([FromBody] User user)
        {
            var result = await _userService.Update(user);
            return new OkObjectResult(result);
        }

        [HttpDelete("Delete/{id}")]

        public async Task<IActionResult> Delete(long id)
        {
            var result = await _userService.Delete(id);
            return new OkObjectResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var result = await _userService.GetById(id, token);
            return new OkObjectResult(result);
        }

        [HttpGet("UserAvatarUpdate/{id}/{fileId}")]

        public async Task<IActionResult> UserAvatarUpdate(long id, long fileId)
        {
            var result = await _userService.UserAvatarUpdate(id, fileId);
            return new OkObjectResult(result);
        }

        [HttpGet("GetUserPermissions")]
        

        public async Task<IActionResult> GetUserPermissions()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var result = await _userService.GetUserPermissions(token);
            return new OkObjectResult(result);
        }

        [HttpGet("UserAddressList/{userId}")]
        

        public async Task<IActionResult> GetUserAddresses(long userId)
        {
            var result = await _userService.GetUserAddresses(userId);
            return new OkObjectResult(result);
        }

        [HttpPost("UserAddressSave")]
        

        public async Task<IActionResult> UserAddressSave([FromBody] UserAddress userAddress)
        {
            var result = await _userService.UserAddressSave(userAddress);
            return new OkObjectResult(result);
        }

        [HttpPost("UserAddressUpdate")]
        

        public async Task<IActionResult> UserAddressUpdate([FromBody] UserAddress userAddress)
        {
            var result = await _userService.UserAddressUpdate(userAddress);
            return new OkObjectResult(result);
        }

        [HttpDelete("UserAddressDelete/{id}")]
        

        public async Task<IActionResult> UserAddressDelete(long id)
        {
            var result = await _userService.UserAddressDelete(id);
            return new OkObjectResult(result);
        }

        [HttpGet("UserAddressById/{id}")]
        

        public async Task<IActionResult> GetUserAddressById(long id)
        {
            var result = await _userService.GetUserAddressById(id);
            return new OkObjectResult(result);
        }

        [HttpGet("GetPrimaryUserAddressById/{userId}")]
        

        public async Task<IActionResult> GetPrimaryUserAddressById(long userId)
        {
            var result = await _userService.GetPrimaryUserAddressById(userId);
            return new OkObjectResult(result);
        }
    }
}
