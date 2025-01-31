using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementApi.Data;
using OrderManagementApi.Models;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly OrderManagementDbContext _context;

    public CustomersController(OrderManagementDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ComCustomer>>> GetCustomers()
    {
        var customers = await _context.ComCustomers
            .OrderBy(c => c.CustomerName)
            .ToListAsync();
            
        return Ok(customers);
    }

}