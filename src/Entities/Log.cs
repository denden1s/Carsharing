using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Carsharing.Interfaces;

namespace Carsharing.Entities
{
  public class Log : ILoadEntity<Log>
  {
    public int ID { get; private set; }
    public int User_ID { get; set; }
    public DateTime Enter_time { get; set; } = DateTime.Now;
    public DateTime End_time { get; set; }

    public User User { get; set; }

    public Log() { }
    public Log(int user_id, DateTime start_time)
    {
      User_ID = user_id;
      Enter_time = start_time;
    }
    public Log(int user_id, DateTime start_time, DateTime end_time)
    {
      User_ID = user_id;
      Enter_time = start_time;
      End_time = end_time;
    }

    public List<Log> LoadData()
    {
      throw new NotImplementedException();
    }

    public void Add()
    {
      using(ApplicationContext db = new ApplicationContext())
      {
        db.Log.Add(this);
        db.SaveChanges();
      }
    }
  }
}
