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
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService companyService;
        public CompanyController(ICompanyService companyService)
        {

            this.companyService = companyService;
        }

        // GET: api/Company
        [HttpGet]
        //[Route("GetCompanies")]
        public async Task<IActionResult> Get()
        {
            var companies = await this.companyService.GetAll();

            return Ok(companies);
        }

        // GET: api/Company/5
        [HttpGet("{id}")]
        //[Route("GetCompany")]
        public async Task<IActionResult> Get(int id)
        {
            var company = await this.companyService.GetID(id);
            return Ok(company);
        }

        // POST: api/Company
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CompanyDto company)
        {
            if (ModelState.IsValid)
            {
                return Ok(await this.companyService.CreateCompany(company));
            }

            return BadRequest("Invalid Data");
        }

        // PUT: api/Company/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CompanyDto company)
        {
            if (ModelState.IsValid)
            {
                return Ok(await this.companyService.UpdateCompany(id, company));
            }

            return BadRequest("Invalid Data");
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await this.companyService.DeleteCompany(id);
            return Ok();
        }
    }
}