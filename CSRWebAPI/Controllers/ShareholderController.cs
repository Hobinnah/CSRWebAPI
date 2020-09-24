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
    [Route("api/[controller]")]
    [ApiController]
    public class ShareholderController : ControllerBase
    {
        private readonly IShareholderService shareholderService;
        public ShareholderController(IShareholderService shareholderService)
        {
            this.shareholderService = shareholderService;
        }

        // GET: api/Shareholder
        [HttpGet]
        //[Route("GetShareholders")]
        public async Task<IActionResult> Get()
        {
            var shareholders = await this.shareholderService.GetAll();

            return Ok(shareholders);
        }

        // GET: api/Shareholder/5
        [HttpGet("{id}")]
        //[Route("GetShareholder")]
        public async Task<IActionResult> Get(int id)
        {
            var shareholder = await this.shareholderService.GetID(id);
            return Ok(shareholder);
        }

        // POST: api/Shareholder
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ShareholderDto shareholder)
        {
            if (ModelState.IsValid)
            {
                return Ok(await this.shareholderService.CreateShareholder(shareholder));
            }

            return BadRequest("Invalid Data");
        }

        // PUT: api/Shareholder/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ShareholderDto shareholder)
        {
            if (ModelState.IsValid)
            {
                return Ok(await this.shareholderService.UpdateShareholder(id, shareholder));
            }

            return BadRequest("Invalid Data");
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await this.shareholderService.DeleteShareholder(id);
            return Ok();
        }
    }
}