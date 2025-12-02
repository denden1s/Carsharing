using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Carsharing.Interfaces;

namespace Carsharing.Entities
{
  public class Rent_deal : ILoadEntity<Rent_deal>
  {
    public int ID { get; private set; }
    public int User_ID { get; private set; }
    public int Client_ID { get; private set; }
    public string Car_ID { get; private set; }
    public DateTime Date { get; private set; } = DateTime.Now;
    public string? Document { get; private set; }

    public Client Client { get; private set; }

    public Rent_details Details { get; set; }

    public Rent_deal() { }

    public Rent_deal(int user_ID, int client_ID, string car_ID, DateTime date, string document)
    {
      User_ID = user_ID;
      Client_ID = client_ID;
      Car_ID = car_ID;
      Date = date;
      Document = document;
      //LoadCar();
      LoadDetails();
    }

    public List<Rent_deal> LoadData()
    {
      using(ApplicationContext db = new ApplicationContext())
      {
        return db.Rent_deal.ToList();
      }
    }
    public Car GetCar()
    {
       Car car = new Car();
      using(ApplicationContext db = new ApplicationContext())
      {
        car = db.Car.Where(i => Car_ID == i.ID).FirstOrDefault();
      }
      return car;
    }

    public void LoadDetails()
    {
      using(ApplicationContext db = new ApplicationContext())
      {
        Details = db.Rent_details.Where(i => ID == i.ID).FirstOrDefault();
      }
    }
    public void Add()
    {
      using(ApplicationContext db = new ApplicationContext())
      {
        db.Rent_deal.Add(this);
        db.SaveChanges();
      }
    }
  }
}
