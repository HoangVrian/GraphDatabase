using Gremlin.Net.Process.Traversal;
using Gremlin.Net.Structure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GremlinAPI.Controllers
{
    [Route("api/gremlin")]
    [ApiController]
    public class GremlinController : ControllerBase
    {
        private readonly GraphTraversalSource _g;
        public GremlinController(GraphTraversalSource g)
        {
            _g = g;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            var user = await _g.V()
                           .HasLabel("person")
                           .Project<object>("Id", "FirstName", "LastName")
                               .By(T.Id)
                               .By("firstName")
                               .By("lastName")
                           .Promise(traversal => traversal.ToList());
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUsersAsync()
        {
            try
            {
                var first = _g.AddV("person").Property("firstName", "Cena").Property("lastName", "Join").Next();
                var second = _g.AddV("person").Property("firstName", "Dwayne").Property("lastName", "Johnson").Next();
                await _g.V(first).AddE("knows").To(second).Property("weight", 0.75).Promise(t => t.Iterate());
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }

        }

        [HttpPut]
        public async Task<IActionResult> UpdateUsersAsync(int id)
        {
            var first = await _g.V(id).Property("firstName", "Cenananananana").Promise(t => t.Iterate());
            return Ok();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> DeleteUsersAsync(int id)
        {
            await _g.V().HasLabel("person").Drop().Promise(t => t.Iterate());
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            await _g.V(id).Drop().Promise(t => t.Iterate());
            return Ok();
        }
    }
}
