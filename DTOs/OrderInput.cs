public class OrderInput
{
    public DateTime OrderDate { get; set; }
    public int Customer { get; set; }
    public string Address { get; set; }
    public List<ItemInput> Items { get; set; }
}