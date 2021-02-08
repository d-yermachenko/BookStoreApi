using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookStoreApi.Controllers {


    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase {

        public HomeController() {
        }

        // GET: api/<ValuesController>
        [HttpGet]
        [Authorize]
        public IEnumerable<string> Get() {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public string Get(int id) {
            return "value";
        }

        /// <summary>
        /// Tesp method to authorize only customer
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        [Authorize(Roles ="Administrator,Customer")]
        public void Post([FromBody] string value) {
            
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Customer")]
        public void Put(int id, [FromBody] string value) {
            ;
        }

        [Authorize(Roles = "Administrator")]
        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id) {
        }
    }
}
