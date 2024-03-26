using GeekShopping.ProductAPI.Data.ValueObjects;
using GeekShopping.ProductAPI.Repository;
using GeekShopping.ProductAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.ProductAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository repository;

        public ProductController(IProductRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductVO>>> FindAll()
        {
            var products = await repository.FindAll();

            return Ok(products);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductVO>> FindById(long id)
        {
            var product = await repository.FindById(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ProductVO>> Create([FromBody] ProductVO vo)
        {
            if (vo == null)
            {
                return BadRequest();
            }

            var product = await repository.Create(vo);
            return Ok(product);
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult<ProductVO>> Update([FromBody] ProductVO vo)
        {
            if (vo == null)
            {
                return BadRequest();
            }

            var product = await repository.Update(vo);
            return Ok(product);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var status = await repository.Delete(id);
            if (!status)
            {
                return BadRequest();
            }
            return Ok(status);
        }
    }
}
