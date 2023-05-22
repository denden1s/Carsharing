using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Carsharing.Interfaces;

namespace Carsharing.Entities
{
  public class Transaction : ILoadEntity<Transaction>
  {
    public int ID { get; private set; }
    public int User_ID { get; private set; }

    public DateTime Date { get; private set; } = DateTime.Now;

    //true - user, false - client
    public bool Role { get; private set; } = true;

    public Client Client { get; private set; }

    public Transaction()
    {
      //выборка имени из БД
    }
    public Transaction(int user_ID, DateTime date, bool role)
    {
      User_ID = user_ID;
      Date = date;
      Role = role;
    }

    public bool CreateTransaction()
    {
      return true;
    }
    public List<Transaction> LoadData()
    {
      return new List<Transaction>();
    }
    public void Add()
    {
      using(ApplicationContext db = new ApplicationContext())
      {
        db.Transaction.Add(this);
        db.SaveChanges();
      }
    }
  }
}

