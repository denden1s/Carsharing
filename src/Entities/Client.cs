using System;
using System.Collections.Generic;
using System.Linq;
using Carsharing.Interfaces;

namespace Carsharing.Entities
{
  public class Client : ILoadEntity<Client>
  {
    public int ID { get; protected set; }
    public string Name { get; protected set; }
    public int Number { get; protected set; }
    
    public string Passport { get; set; }
    public int Role { get; protected set; }

   
    public List<Transaction> Transactions { get; protected set; }
    public List<Rent_deal> Rent_deals { get; protected set; }

    public Client() { }
    public Client(string name, int number, int role = 3)
    {
      Name = name;
      Number = number;
      Role = role;
    }
    public List<Client> LoadData()
    {
      using(ApplicationContext db = new ApplicationContext())
      {
        return db.Client.Where(i => i.Role ==3).ToList();
      }
    }
    public void Add()
    {
      using (ApplicationContext db = new ApplicationContext())
      {
        db.Client.Add(this);
        db.SaveChanges();
      }
    }
  }
}
