using Microsoft.AspNetCore.Mvc;
using ShopService.Models;
using ShopService.Repositories;

namespace ShopService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShopController : ControllerBase
    {
        private readonly ShopRepository _repo;

        public ShopController(ShopRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("items")]
        public IActionResult GetItems()
        {
            return Ok(_repo.GetItems());
        }

        [HttpPost("items")]
        public IActionResult AddItem([FromBody] ShopItem item)
        {
            _repo.AddItem(item);
            return CreatedAtAction(nameof(GetItems), item);
        }
    }
}
