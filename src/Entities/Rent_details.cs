using System;
using System.Collections.Generic;
using System.Linq;
using Carsharing.Interfaces;

namespace Carsharing.Entities
{
  public class Rent_details : ILoadEntity<Rent_details>
  {
    public int ID { get; private set; }
    public string Car_ID { get; private set; }
    public DateTime TakingTime { get; private set; } = DateTime.Now;
    public DateTime Expected_return_time { get; private set; }
    public DateTime Facted_return_time { get; private set; }

    public Car Car { get; private set; }
    public Rent_deal Rent { get; private set; }


    public Rent_details() { }
    public Rent_details(int iD, string car_ID, DateTime takingTime, DateTime expected_return_time, DateTime facted_return_time)
    {
      ID = iD;
      Car_ID = car_ID;
      TakingTime = takingTime;
      Expected_return_time = expected_return_time;
      Facted_return_time = facted_return_time;
    }

    public List<Rent_details> LoadData()
    {
      using(ApplicationContext db = new ApplicationContext())
      {
        return db.Rent_details.ToList();
      }
    }
    public void Add()
    {
      using(ApplicationContext db = new ApplicationContext())
      {
        db.Rent_details.Add(this);
        db.SaveChanges();
      }
    }
    public void LoadCarData()
    {
      using(ApplicationContext db = new ApplicationContext())
      {
        Car = db.Car.Where(i => Car_ID == i.ID).FirstOrDefault();
        Car.LoadModel();
      }
    }
  }
}
