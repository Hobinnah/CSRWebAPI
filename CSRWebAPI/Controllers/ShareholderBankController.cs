using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSRWebAPI.Repositories.DTO;
using CSRWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CSRWebAPI.Controllers
{
    [Route("api/Shareholder/[controller]")]
    [ApiController]
    public class ShareholderBankController : ControllerBase
    {
        private readonly IShareholderBankService shareholderBankService;
        public ShareholderBankController(IShareholderBankService shareholderBankService)
        {
            this.shareholderBankService = shareholderBankService;
        }

        // GET: api/ShareholderBank
        [HttpGet]
        //[Route("GetShareholderBanks")]
        public async Task<IActionResult> Get()
        {
            var shareholderBanks = await this.shareholderBankService.GetAll();

            return Ok(shareholderBanks);
        }

        // GET: api/ShareholderBank/5
        [HttpGet("{id}")]
        //[Route("GetShareholderBank")]
        public async Task<IActionResult> Get(int id)
        {
            var shareholderBank = await this.shareholderBankService.GetID(id);
            return Ok(shareholderBank);
        }

        // POST: api/ShareholderBank
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ShareholderBankDto shareholderBank)
        {
            if (ModelState.IsValid)
            {
                return Ok(await this.shareholderBankService.CreateShareholderBank(shareholderBank));
            }

            return BadRequest("Invalid Data");
        }

        // PUT: api/ShareholderBank/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ShareholderBankDto shareholderBank)
        {
            if (ModelState.IsValid)
            {
                return Ok(await this.shareholderBankService.UpdateShareholderBank(id, shareholderBank));
            }

            return BadRequest("Invalid Data");
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await this.shareholderBankService.DeleteShareholderBank(id);
            return Ok();
        }
    }
}