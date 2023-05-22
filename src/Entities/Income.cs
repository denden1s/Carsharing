

using System;
using System.Collections.Generic;
using System.Linq;
using Carsharing.Interfaces;

namespace Carsharing.Entities
{
  public class Income : ILoadEntity<Income>
  {
    public int ID { get; private set; }
    public DateTime Date { get; private set; } = DateTime.Now;
    public decimal Earning { get; private set; }

    public Income() { }
    public Income(DateTime date, decimal earning)
    {
      Date = date;
      Earning = earning;
    }
    public List<Income> LoadData()
    {
      using(ApplicationContext db = new ApplicationContext())
      {
        return db.Income.ToList();
      }
    }
    public void Add()
    {
      using(ApplicationContext db = new ApplicationContext())
      {
        db.Income.Add(this);
        db.SaveChanges();
      }
    }
  }
}
