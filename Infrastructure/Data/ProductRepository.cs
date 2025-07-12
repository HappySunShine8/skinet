using System;
using System.IO.Compression;
using Core;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Data;

public class ProductRepository(StoreContext context) : IProductRepository
{
    public void AddProduct(Core.Product product)
    {
        context.Products.Add(product);
    }

    public void DeleteProduct(Product product)
    {
        context.Products.Remove(product);
    }

    public async Task<IReadOnlyList<string>> GetBrandsAsync()
    {
        return await context.Products
        .Select(p => p.Brand)
        .Distinct()
        .ToListAsync();
    }

    public async Task<Core.Product?> GetProductByIdAsync(int id)
    {
        return await context.Products.FindAsync(id);
    }

    public async Task<IReadOnlyList<Core.Product>> GetProductsAsync(string? type,
    string? brand, string? sort)
    {
        var query = context.Products.AsQueryable();
        if (!string.IsNullOrWhiteSpace(brand))
            query = query.Where(p => p.Brand == brand);

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(p => p.Type == type);

        query = sort switch
        {
            "priceAsc" => query.OrderBy(p => p.Price),
            "priceDesc" => query.OrderByDescending(p => p.Price),
            _ => query.OrderBy(p => p.Name)
        };
        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<string>> GetTypesAsync()
    {
        return await context.Products
        .Select(x => x.Type)
        .Distinct()
        .ToListAsync();
    }

    public bool ProductExists(int id)
    {
        return context.Products.Any(x => x.Id == id);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await context.SaveChangesAsync() > 0;
        // SaveChangesAsync returns the number of state entries written to the database
    }

    public void UpdateProduct(Product product)
    {
        // context.Products.Update(product);
        context.Entry(product).State = EntityState.Modified;
        // This is more efficient than using Update as it only marks the entity as modified
    }
}
