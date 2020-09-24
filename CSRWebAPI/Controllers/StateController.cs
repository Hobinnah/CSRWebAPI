using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSRWebAPI.Repositories.DTO;
using CSRWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CSRWebAPI.Controllers
{
    [EnableCors("EnableCORS")]
    [Route("api/[controller]")]
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly IStateService stateService;
        public StateController(IStateService stateService)
        {

            this.stateService = stateService;
        }

        // GET: api/State
        [HttpGet]
        //[Route("GetStates")]
        public async Task<IActionResult> Get()
        {
            var states = await this.stateService.GetAll();

            return Ok(states);
        }

        //GET: api/State/5
        [HttpGet("{id}")]
       // [Route("GetState")]
        public async Task<IActionResult> Get(int id)
        {
            var state = await this.stateService.GetID(id);
            return Ok(state);
        }

        // POST: api/State
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] StateDto state)
        {
            if (ModelState.IsValid)
            {
                return Ok(await this.stateService.CreateState(state));
            }

            return BadRequest("Invalid Data");
        }

        // PUT: api/State/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] StateDto state)
        {
            if (ModelState.IsValid)
            {
                return Ok(await this.stateService.UpdateState(id, state));
            }

            return BadRequest("Invalid Data");
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await this.stateService.DeleteState(id);
            return Ok();
        }
    }
}