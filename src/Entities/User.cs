using System;
using System.Collections.Generic;
using Carsharing.Interfaces;
using testing_system.Classes;

namespace Carsharing.Entities
{
  public class User : Client, ILoadEntity<User>
  {
    private Encryption encryption = new Encryption();
    public string Login { get; private set; }
    public string Password { get; private set; }//шифрование есть в тестирующем модуле

    public List<Log> Logs { get; private set; }
    public User():base() 
    {
      
      Role = 2;
      Login = "test";
      Password = "test";
      Name= "test";
    }
    public User(string name, int number, string login, string password, int Role = 2): base(name, number)
    {
      Login = login;
      Password = encryption.EncryptPassword(password, login);
      this.Role = Role;
        }

    List<User> ILoadEntity<User>.LoadData()
    {
      throw new NotImplementedException();
    }
  }
}
