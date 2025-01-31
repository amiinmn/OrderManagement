public class OrdersDto
{

    public string SalesOrder { get; set; }
    public string OrderDate { get; set; }
    public string Customer { get; set; }
    public string Address { get; set; }
    public double TotalAmount { get; set; } 
    public int TotalItems { get; set; }
    public List<ItemDto>Items { get; set; }
}

public class OrdersAllDto
{
    public string SalesOrder { get; set; }
    public string OrderDate { get; set; }
    public string Customer { get; set; }
}