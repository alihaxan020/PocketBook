using Microsoft.AspNetCore.Mvc;
using PocketBook.Core.IConfiguration;
using PocketBook.Models;

namespace PocketBook.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public UserController(ILogger<UserController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _unitOfWork.Users.All();
            return Ok(users);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetItem(Guid id)
        {
            var user = await _unitOfWork.Users.GetById(id);
            if (user == null)
            {
                return NotFound(); // 404 http not found error
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (ModelState.IsValid)
            {
                user.Id = Guid.NewGuid();
                await _unitOfWork.Users.Add(user);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction("GetItem", new { user.Id }, user);
            }
            return new JsonResult("Something went wrong") { StatusCode = 500 };
        }
        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateItem(Guid id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }
            await _unitOfWork.Users.Upsert(user);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var item = await _unitOfWork.Users.GetById(id);
            if (item == null)
            {
                return BadRequest();
            }
            await _unitOfWork.Users.Delete(id);
            await _unitOfWork.CompleteAsync();
            return new JsonResult(new { message = "User has been deleted successfully", id = id });
        }

    }
}