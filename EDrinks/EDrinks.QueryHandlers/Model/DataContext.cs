using System.Collections.Generic;

namespace EDrinks.QueryHandlers.Model
{
    public interface IDataContext
    {
        List<Product> Products { get; set; }
        
        List<Tab> Tabs { get; set; }
        
        List<Order> CurrentOrders { get; set; }
        
        List<Order> AllOrders { get; set; }
        
        List<Settlement> Settlements { get; set; }
        
        Settlement CurrentSettlement { get; set; }
    }
    
    public class DataContext : IDataContext
    {
        public List<Product> Products { get; set; } = new List<Product>();
        
        public List<Tab> Tabs { get; set; } = new List<Tab>();
        
        public List<Order> CurrentOrders { get; set; } = new List<Order>();
        
        public List<Order> AllOrders { get; set; } = new List<Order>();
        
        public List<Settlement> Settlements { get; set; } = new List<Settlement>();
        
        public Settlement CurrentSettlement { get; set; } = new Settlement();
    }
}