using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Carsharing.Interfaces;

namespace Carsharing.Entities
{
  public class Car : ILoadEntity<Car>
  {
    public string ID { get; private set; }
    public int Model_ID { get; private set; }
    public string Statement { get; private set; }

    public Car_model Model { get; private set; }
    public List<Rent_details> Details { get; private set; }
    public List<Rent_deal> Rent_deals { get; private set; }

    public Car() { }
    public Car(int model_ID, string statement, string number)
    {
      Model_ID = model_ID;
      Statement = statement;
      ID = number;
    }

    public List<Car> LoadData()
    {
      using(ApplicationContext db = new ApplicationContext())
      {
        return db.Car.ToList();
      }
    }
    public void LoadModel()
    {
      using(ApplicationContext db = new ApplicationContext())
      {
        Model = db.Car_model.Where(i => i.ID == Model_ID).SingleOrDefault();
      } 
    }
    //set car model
  }
}
