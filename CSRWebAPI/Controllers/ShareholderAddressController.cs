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
    public class ShareholderAddressController : ControllerBase
    {
        private readonly IShareholderAddressService shareholderAddressService;
        public ShareholderAddressController(IShareholderAddressService shareholderAddressService)
        {

            this.shareholderAddressService = shareholderAddressService;
        }

        // GET: api/ShareholderAddress
        [HttpGet]
        //[Route("GetShareholderAddresses")]
        public async Task<IActionResult> Get()
        {
            var shareholderAddresses = await this.shareholderAddressService.GetAll();

            return Ok(shareholderAddresses);
        }

        // GET: api/ShareholderAddress/5
        [HttpGet("{id}")]
        //[Route("GetShareholderAddress")]
        public async Task<IActionResult> Get(int id)
        {
            var shareholderAddress = await this.shareholderAddressService.GetID(id);
            return Ok(shareholderAddress);
        }

        // POST: api/ShareholderAddress
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ShareholderAddressDto shareholderAddress)
        {
            if (ModelState.IsValid)
            {
                return Ok(await this.shareholderAddressService.CreateShareholderAddress(shareholderAddress));
            }

            return BadRequest("Invalid Data");
        }

        // PUT: api/ShareholderAddress/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ShareholderAddressDto shareholderAddress)
        {
            if (ModelState.IsValid)
            {
                return Ok(await this.shareholderAddressService.UpdateShareholderAddress(id, shareholderAddress));
            }

            return BadRequest("Invalid Data");
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await this.shareholderAddressService.DeleteShareholderAddress(id);
            return Ok();
        }
    }
}