using BANK.BLL.Services;
using BANK.DAL.Entity;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web_applicationSecurity.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BankAccountsController : ControllerBase
    {
        private readonly IBankAccountService _service;
        private readonly IAntiforgery _antiforgery;
        public BankAccountsController(
     IBankAccountService service,
     IAntiforgery antiforgery)
        {
            _service = service;
            _antiforgery = antiforgery;
        }
        [Authorize(Roles = "Admin,Manager,User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BankAccount>>> GetAll()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);

            Response.Headers["X-CSRF-TOKEN"] = tokens.RequestToken;

            var accounts = await _service.GetAllAsync();

            return Ok(accounts);
        }
        [Authorize(Roles = "Admin,Manager,User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<BankAccount>> GetById(int id)
        {
            var account = await _service.GetByIdAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }
        [Authorize(Roles = "Admin,Manager")]

        [HttpPost]
        public async Task<ActionResult<BankAccount>> Create(
            BankAccount account)
        {
            await _antiforgery.ValidateRequestAsync(HttpContext);

            try
            {
                var createdAccount =
                    await _service.CreateAsync(account);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdAccount.Id },
                    createdAccount);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
        }
        [Authorize(Roles = "Admin,Manager")]

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            BankAccount account)
        {
            await _antiforgery.ValidateRequestAsync(HttpContext);

            try
            {
                var updated =
                    await _service.UpdateAsync(id, account);

                if (!updated)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [Authorize(Roles = "Admin")]

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _antiforgery.ValidateRequestAsync(HttpContext);

            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("admin-only")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminOnly()
        {
            return Ok("Only Admin can access this route");
        }

        [HttpGet("manager")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult ManagerAccess()
        {
            return Ok("Admin or Manager access");
        }
    }
}
