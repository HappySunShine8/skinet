using System;
using Core;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductRepository repo) : ControllerBase
{
    // private readonly StoreContext context;

    // public ProductsController(StoreContext context)
    // {
    //     this.context = context;
    // }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string? type,
    string? brand, string? sort)
    {
        // return await context.Products.ToListAsync();
        // Using the repository pattern to get products
        // This abstracts the data access logic and allows for easier testing and maintenance
        // var products = await repo.GetProductsAsync();
        // return Ok(products);
        return Ok(await repo.GetProductsAsync(type, brand, sort));
        // This returns a 200 OK response with the list of products
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        // var product = await context.Products.FindAsync(id);
        var product = await repo.GetProductByIdAsync(id);
        // This retrieves a product by its ID using the repository pattern
        if (product == null) return NotFound();
        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        // context.Products.Add(product);
        // await context.SaveChangesAsync();
        repo.AddProduct(product);
        if (await repo.SaveChangesAsync())
        {
            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
            // This returns a 201 Created response with the location of the new product
        }
        return BadRequest("Problem creating product");
        // This returns a 400 Bad Request response if there was a problem creating the product
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (product.Id != id || !ProductExist(id))
        {
            return BadRequest("Product ID mismatch or product does not exist");
        }
        // context.Entry(product).State = EntityState.Modified;
        // await context.SaveChangesAsync();
        repo.UpdateProduct(product);
        if (await repo.SaveChangesAsync())
        {
            return NoContent();
            // This returns a 204 No Content response if the update was successful
        }
        // This updates the product using the repository pattern    
        return BadRequest("Problem updating product");
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        return Ok(await repo.GetBrandsAsync());
    }
    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        return Ok(await repo.GetTypesAsync());
    }
    private bool ProductExist(int id)
    {
        //return context.Products.Any(x => x.Id == id);
        return repo.ProductExists(id);
        // This checks if a product exists by its ID using the repository pattern
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await repo.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        // context.Products.Remove(product);
        // await context.SaveChangesAsync();
        repo.DeleteProduct(product);
        if (await repo.SaveChangesAsync())
            return NoContent();
        return BadRequest("Problem deleting product");
    }
}
