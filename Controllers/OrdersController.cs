using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OrderManagementApi.Data;
using OrderManagementApi.Models;


[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderManagementDbContext _context;

    public OrdersController(OrderManagementDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult> GetOrders(string? keywords, DateTime? orderDate)
    {
        var result = await GetOrderData(keywords, orderDate);
        return Ok(result);
    }

    [HttpGet]
    [Route("export")]
    public async Task<IActionResult> ExportOrdersToExcel(string? keywords, DateTime? orderDate)
    {
        var result = await GetOrderData(keywords, orderDate);

        using (var package = new ExcelPackage())
        {
            var workSheet = package.Workbook.Worksheets.Add("Orders");

            workSheet.Cells[1,1].Value = "No";
            workSheet.Cells[1,2].Value = "Sales Order";
            workSheet.Cells[1,3].Value = "Order Date";
            workSheet.Cells[1,4].Value = "Customer";

            int row = 2;
            foreach (var order in result)
            {
                workSheet.Cells[row, 1].Value = row - 1;
                workSheet.Cells[row, 2].Value = order.SalesOrder;
                workSheet.Cells[row, 3].Value = order.OrderDate;
                workSheet.Cells[row, 4].Value = order.Customer;
                row++;
            }

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"SalesOrders_{DateTime.Now:yyyyMMdd}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }

    private async Task<List<OrdersAllDto>> GetOrderData(string? keywords, DateTime? orderDate)
    {
        var query = from o in _context.SoOrders
                    join c in _context.ComCustomers on o.ComCustomerId equals c.ComCustomerId
                    orderby o.OrderDate descending
                    select new 
                    {
                        o.OrderNo,
                        o.OrderDate,
                        c.CustomerName
                    };

        if(!string.IsNullOrEmpty(keywords))
        {
            query = query.Where(o => o.OrderNo.Contains(keywords) || o.CustomerName.Contains(keywords));
        }

        if(orderDate.HasValue)
        {
            query = query.Where(o => o.OrderDate.Date == orderDate.Value.Date);
        }

        var result = await query.Select(o => new OrdersAllDto
        {
            SalesOrder = o.OrderNo,
            OrderDate = o.OrderDate.ToString("dd/MM/yyyy"),
            Customer = o.CustomerName
        }).ToListAsync();

        return result;
    }

    [HttpPost]
    public async Task<ActionResult<OrdersDto>> PostOrder(OrderInput input)
    {
        var orderNumber = await GenerateOrderNumber();

        var order = new SoOrder
        {
            OrderNo = orderNumber,
            OrderDate = input.OrderDate,
            ComCustomerId = input.Customer,
            Address = input.Address,
        };

        _context.SoOrders.Add(order);
        await _context.SaveChangesAsync();

        foreach (var item in input.Items)
        {
            var soItem = new SoItem
            {
                ItemName = item.ItemName,
                Quantity = item.Quantity,
                Price = item.Price,
                SoOrderId = order.SoOrderId
            };

            _context.SoItems.Add(soItem);
        }

        await _context.SaveChangesAsync();

        var query = from o in _context.SoOrders
                    join c in _context.ComCustomers on o.ComCustomerId equals c.ComCustomerId
                    join i in _context.SoItems on o.SoOrderId equals i.SoOrderId
                    where o.SoOrderId == order.SoOrderId
                    select new 
                    {
                        o.OrderNo,
                        o.OrderDate,
                        c.CustomerName,
                        i.ItemName,
                        i.Quantity,
                        i.Price
                    };

        var result = new OrdersDto
        {
            SalesOrder = orderNumber,
            OrderDate = order.OrderDate.ToString("dd/MM/yyyy"),
            Customer = query.FirstOrDefault().CustomerName,
            Address = order.Address,
            TotalAmount = query.Sum(i => i.Quantity * i.Price),
            TotalItems = query.Sum(i => i.Quantity),
            Items = query.Select(i => new ItemDto
            {
                ItemName = i.ItemName,
                Quantity = i.Quantity,
                Price = i.Price,
                Total = i.Quantity * i.Price,
            }).ToList()
        };

        return CreatedAtAction("GetOrders", new { id = order.SoOrderId }, result);
    }

    //Order number generator
    private async Task<string> GenerateOrderNumber()
    {
        var today = DateTime.Now.Date;
        string datePart = today.ToString("yyyyMMdd");

        int orderCount = await _context.SoOrders
            .Where(o => o.OrderDate.Date == today)
            .CountAsync();
        string numberOrder = $"SO_{datePart}{orderCount + 1}";

        return numberOrder;
    }

}