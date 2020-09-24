using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CSRWebAPI.Repositories.DTO;
using CSRWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CSRWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        
        private readonly IBankService bankService;
        public BankController(IBankService bankService)
        {
            
            this.bankService = bankService;
        }

        // GET: api/Bank
        [HttpGet]
        //[Route("GetBanks")]
        public async Task<IActionResult> Get()
        {
            var banks = await this.bankService.GetAll();

            return Ok(banks);
        }

        // GET: api/Bank/5
        [HttpGet("{id}")]
       // [Route("GetBank")]
        public async Task<IActionResult> Get(int id)
        {
            var bank = await this.bankService.GetID(id);
            return Ok(bank);
        }

        // POST: api/Bank
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BankDto bank)
        {
            if (ModelState.IsValid)
            {
                return Ok(await this.bankService.CreateBank(bank));
            }

            return BadRequest("Invalid Data");
        }

        // PUT: api/Bank/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] BankDto bank)
        {
            if (ModelState.IsValid)
            {
                return Ok(await this.bankService.UpdateBank(id, bank));
            }

            return BadRequest("Invalid Data");
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await this.bankService.DeleteBank(id);
            return Ok();
        }
    }
}
