using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Carsharing.Interfaces;

namespace Carsharing.Entities
{
  public class Price_list : ILoadEntity<Price_list>
  {
    public int ID { get; private set; }
    public int Model_ID { get; private set; }
    public decimal Price_per_hour { get; private set; }
    public decimal Price_per_day { get; private set; }

    public Car_model Model { get; private set; }

    public Price_list() { }
    public Price_list(int model_ID, decimal price_per_hour, decimal price_per_day)
    {
      Model_ID = model_ID;
      Price_per_hour = price_per_hour;
      Price_per_day = price_per_day;
    }

    public void SetPrice(decimal price)
    {
      Price_per_hour = price; 
      Price_per_day = price * Convert.ToDecimal(0.8);
    }
    public List<Price_list> LoadData()
    {
      using(ApplicationContext db = new ApplicationContext())
      {
        return db.Price_list.ToList();
      }
    }
  }
}
