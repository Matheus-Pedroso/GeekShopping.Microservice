using GeekShopping.ProductAPI.Data.ValueObjects;
using GeekShopping.ProductAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.ProductAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ProductController(IProductRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductVO>>> GetAll()
    {
        IEnumerable<ProductVO> result = await repository.FindAll();

        return Ok(result);
    }
    [HttpGet("Id")]
    public async Task<ActionResult<ProductVO>> GetById(long id)
    {
        ProductVO result = await repository.FindById(id);
        if (result is null)
            return NotFound();
        return Ok(result);
    }
    [HttpPost]
    public async Task<ActionResult<ProductVO>> Create([FromBody] ProductVO productVO)
    {
        if (productVO is null)
            return BadRequest();
        
        ProductVO result = await repository.Create(productVO);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
    [HttpPut]
    public async Task<ActionResult<ProductVO>> Update([FromBody] ProductVO productVO)
    {
        if (productVO is null)
            return BadRequest();
        
        ProductVO result = await repository.Update(productVO);
        if (result is null)
            return BadRequest();
        
        return Ok(result);
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        bool isDeleted = await repository.Delete(id);
        if (!isDeleted)
            return BadRequest();
        
        return NoContent();
    }
}
