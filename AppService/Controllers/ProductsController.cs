using AppService.Microservices.StorageService;
using AppService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AppService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Seller")]
public class ProductsController(IStorageService storgeService,
                                ILogger<ProductsController> logger) : ControllerBase
{
    private static readonly List<ProductDto> Products = [];


    [HttpGet("{id}", Name = "GetById")]
    public IActionResult GetProducById(string id)
    {
        logger.LogInformation("Get product request for ProductId: {ProductId}", id);

        var product = Products.FirstOrDefault(p => p.Id == id);

        if (product is null)
            return NotFound();

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(ProductCreationDto product)
    {
        logger.LogInformation("Create product request for Title: {Title}", product.Title);

        var isImageExists = await storgeService.CheckFileExistanceAsync(product.ImageId);

        if (!isImageExists)
            return BadRequest($"Image with id {product.ImageId} does not exist.");

        var isProductExist = Products.FirstOrDefault(p => p.ImageId == product.ImageId);

        if (isProductExist is not null)
            return Conflict($"Product with image id {product.ImageId} already exists.");


        var newProduct = new ProductDto
        {
            Id = Guid.CreateVersion7().ToString("N"),
            Title = product.Title,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId,
            StockQuantity = product.StockQuantity,
            ImageId = product.ImageId
        };

        Products.Add(newProduct);

        logger.LogInformation("Product created successfully. ProductId: {ProductId}", newProduct.Id);

        return CreatedAtRoute("GetById", new { id = newProduct.Id }, newProduct);
    }
}



