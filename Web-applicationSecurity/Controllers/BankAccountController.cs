using BANK.BLL.Services;
using BANK.DAL.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web_applicationSecurity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankAccountsController : ControllerBase
    {
        private readonly IBankAccountService _service;

        public BankAccountsController(IBankAccountService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BankAccount>>> GetAll()
        {
            var accounts = await _service.GetAllAsync();

            return Ok(accounts);
        }

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

        [HttpPost]
        public async Task<ActionResult<BankAccount>> Create(
            BankAccount account)
        {
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            BankAccount account)
        {
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
