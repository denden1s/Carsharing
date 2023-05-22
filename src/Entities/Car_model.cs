using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Carsharing.Interfaces;

namespace Carsharing.Entities
{
  public class Car_model : ILoadEntity<Car_model>
  {
    public int ID { get; private set; }
    public string Brand { get; private set; }
    public string Name { get; private set; }
    public int Year { get; private set; }
    public string Color { get; private set; }
 
    public Price_list Price { get; private set; }
    public List<Car> Cars { get; private set; }

    public Car_model() { }
    public Car_model( string brand, string name, int year, string color)
    {
      Brand = brand;
      Name = name;
      Year = year;
      Color = color;
    }

    public void SetPrice()
    {
      using(ApplicationContext db = new ApplicationContext())
      {
        Price = db.Price_list.Where(i => i.Model_ID == ID).SingleOrDefault();
      }
    }
    public List<Car_model> LoadData()
    {
      List<Car_model> data = new List<Car_model>();
      using(ApplicationContext db = new ApplicationContext())
      {
        data = db.Car_model.ToList();
      }
      return data;
    }
    public override string ToString()
    {
      return $"{Brand} {Name} {Color} {Year}";
    }
  }
}
