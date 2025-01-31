using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementApi.Data;
using OrderManagementApi.Models;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly OrderManagementDbContext _context;

    public ItemsController(OrderManagementDbContext context)
    {
        _context = context;
    }

    [HttpGet("{orderId}")]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetItems(long orderId)
    {
        var items = await _context.SoItems
            .Where(i => i.SoOrderId == orderId)
            .Select(i => new ItemDto
            {
                ItemName = i.ItemName,
                Price = i.Price,
                Quantity = i.Quantity,
                Total = i.Price * i.Quantity
            }).ToListAsync();

        if (items == null || items.Count == 0)
        {
            return NotFound();
        }

        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<ItemDto>> AddItem(ItemInput input)
    {
        var order = await _context.SoOrders.FindAsync(input.SoOrderId);
        if (order == null)
        {
            return NotFound();
        }

        var item = new SoItem
        {
            ItemName = input.ItemName,
            Price = input.Price,
            Quantity = input.Quantity,
            SoOrderId = input.SoOrderId
        };

        _context.SoItems.Add(item);
        await _context.SaveChangesAsync();

        var itemDto = new ItemDto
        {
            ItemName = item.ItemName,
            Price = item.Price,
            Quantity = item.Quantity,
            Total = item.Price * item.Quantity
        };

        return CreatedAtAction(nameof(GetItems), new { orderId = item.SoOrderId }, itemDto);

    }
    [HttpDelete("{itemId}")]
    public async Task<ActionResult<ItemDto>> DeleteItem(long itemId)
    {
        var item = await _context.SoItems.FindAsync(itemId);
        if (item == null)
        {
            return NotFound();
        }

        _context.SoItems.Remove(item);
        await _context.SaveChangesAsync();

        var itemDto = new ItemDto
        {
            ItemName = item.ItemName,
            Price = item.Price,
            Quantity = item.Quantity,
            Total = item.Price * item.Quantity
        };

        return itemDto;
    }

    [HttpPut("{itemId}")]
    public async Task<ActionResult<ItemDto>> UpdateItem(long itemId, ItemInput input)
    {
        var item = await _context.SoItems.FindAsync(itemId);
        if (item == null)
        {
            return NotFound();
        }

        item.ItemName = input.ItemName;
        item.Price = input.Price;
        item.Quantity = input.Quantity;

        await _context.SaveChangesAsync();

        var itemDto = new ItemDto
        {
            ItemName = item.ItemName,
            Price = item.Price,
            Quantity = item.Quantity,
            Total = item.Price * item.Quantity
        };

        return itemDto;
    }
    
    
}