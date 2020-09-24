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
    public class CountryController : ControllerBase
    {
        private readonly ICountryService countryService;
        public CountryController(ICountryService countryService)
        {

            this.countryService = countryService;
        }

        // GET: api/Country
        [HttpGet]
        //[Route("GetCountries")]
        public async Task<IActionResult> Get()
        {
            var Countries = await this.countryService.GetAll();

            return Ok(Countries);
        }

        // GET: api/Country/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var country = await this.countryService.GetID(id);
            return Ok(country);
        }

        // POST: api/Country
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CountryDto country)
        {
            if (ModelState.IsValid)
            {
                return Ok(await this.countryService.CreateCountry(country));
            }

            return BadRequest("Invalid Data");
        }

        // PUT: api/Country/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CountryDto country)
        {
            if (ModelState.IsValid)
            {
                return Ok(await this.countryService.UpdateCountry(id, country));
            }

            return BadRequest("Invalid Data");
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await this.countryService.DeleteCountry(id);
            return Ok();
        }
    }
}