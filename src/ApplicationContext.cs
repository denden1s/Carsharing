using System;
using System.Collections.Generic;
using System.Linq;
using Carsharing.Entities;
using Microsoft.EntityFrameworkCore;


namespace Carsharing
{
  public class ApplicationContext : DbContext
  {
    public DbSet<Transaction> Transaction { get; set; }
    public DbSet<Rent_details> Rent_details { get; set; }
    public DbSet<Rent_deal> Rent_deal { get; set; }
    public DbSet<Log> Log { get; set; }
    public DbSet<Income> Income { get; set; }
    public DbSet<Car_model> Car_model { get; set; }
    public DbSet<Car> Car { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<Client> Client { get; set; }
    public DbSet<Price_list> Price_list { get; set; }

    public ApplicationContext()
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=carsharing;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      // Set primary keys
      modelBuilder.Entity<Car_model>().HasKey(i => i.ID);
      modelBuilder.Entity<Income>().HasKey(i => i.ID);
      modelBuilder.Entity<Client>().HasKey(i => i.ID);
      //modelBuilder.Entity<User>().HasKey(i => i.ID);
      modelBuilder.Entity<Car>().HasKey(i => i.ID);
      modelBuilder.Entity<Rent_deal>().HasKey(i => i.ID);
      modelBuilder.Entity<Rent_details>().HasKey(i => i.ID);
      modelBuilder.Entity<Transaction>().HasKey(i => i.ID);
      modelBuilder.Entity<Log>().HasKey(i => i.ID);
      modelBuilder.Entity<Price_list>().HasKey(i => new { i.ID, i.Model_ID });

      // Set foreign keys
      modelBuilder.Entity<Log>()
        .HasOne<User>(log => log.User)
        .WithMany(user => user.Logs)
        .HasForeignKey(log => log.User_ID);

      modelBuilder.Entity<Price_list>()
        .HasOne<Car_model>(price => price.Model)
        .WithOne(model => model.Price)
        .HasForeignKey<Price_list>(price => price.Model_ID);

      modelBuilder.Entity<Car>()
        .HasOne<Car_model>(car => car.Model)
        .WithMany(model => model.Cars)
        .HasForeignKey(car => car.Model_ID);

      modelBuilder.Entity<Rent_details>()
        .HasOne<Car>(rent_detail => rent_detail.Car)
        .WithMany(car => car.Details)
        .HasForeignKey(rent_detail => rent_detail.Car_ID);

      modelBuilder.Entity<Transaction>()
        .HasOne<Client>(transaction => transaction.Client)
        .WithMany(client => client.Transactions)
        .HasForeignKey(transaction => transaction.User_ID);

      modelBuilder.Entity<Rent_details>()
        .HasOne<Rent_deal>(rent_details => rent_details.Rent)
        .WithOne(rent_deal => rent_deal.Details)
        .HasForeignKey<Rent_details>(rent_details => rent_details.ID);

      modelBuilder.Entity<Rent_deal>()
        .HasOne<Client>(rent_deal => rent_deal.Client)
        .WithMany(client => client.Rent_deals)
        .HasForeignKey(rent_deal => rent_deal.Client_ID);

      
      modelBuilder.Entity<Rent_deal>()
        .HasOne<Car>(rent_deal => rent_deal.Car)
        .WithMany(car => car.Rent_deals)
        .HasForeignKey(rent_deal => rent_deal.Car_ID);
    }

    private bool GenerateUsers(List<Entities.User> Users)
    {
      Users.Add(new Entities.User("Denis Nikitin", 3076433, "root", "root", 1));
      Users.Add(new Entities.User("Nikita Denisov", 2212401, "nik_d", "123"));
      Users.Add(new Entities.User("Ivanov Ivan", 2212401, "II", "Ivan"));
      using(ApplicationContext db = new ApplicationContext())
      {
        db.User.AddRange(Users);
        if(db.SaveChanges() == 0) return false;
      }
      return true;
    }

    private bool GenerateCarModels(List<Car_model> models)
    {
      string[] brand = { "Audi", "BMW", "Mercedes", "Volvo", "Renault" };
      string[] colors = { "Black", "White", "Blue", "Yellow", "Red" };
      int[] years = new int[10];
      for(int i = 0; i < years.Length; i++)
        years[i] = 2013 + i;

      List<(string, string)> model = new List<(string brand, string model)>();
      model.Add(("Audi", "A3"));
      model.Add(("Audi", "A4"));
      model.Add(("Audi", "A6"));
      model.Add(("Audi", "A7"));
      model.Add(("Audi", "Q5"));

      model.Add(("BMW", "318i"));
      model.Add(("BMW", "320i xDrive"));
      model.Add(("BMW", "420i Coupe"));
      model.Add(("BMW", "M440i xDrive Coupe"));
      model.Add(("BMW", "840i xDrive Coupe"));

      model.Add(("Mercedes", "AMG EQS седан"));
      model.Add(("Mercedes", "AMG A-Класс хэтчбек"));
      model.Add(("Mercedes", "AMG CLA купе"));
      model.Add(("Mercedes", "AMG CLS купе"));
      model.Add(("Mercedes", "AMG E-Класс седан"));

      model.Add(("Volvo", "S60"));
      model.Add(("Volvo", "S90"));
      model.Add(("Volvo", "XC40"));
      model.Add(("Volvo", "XC60"));
      model.Add(("Volvo", "XC90"));

      model.Add(("Renault", "Duster"));
      model.Add(("Renault", "Logan"));
      model.Add(("Renault", "Fluence"));
      model.Add(("Renault", "Laguna"));
      model.Add(("Renault", "Sandero"));
      for(int i = 0; i < years.Length; i++)
      {
        for(int j = 0; j < brand.Length; j++)
        {
          List<(string, string)> concrete_model = model.Where(a => a.Item1 == brand[j]).ToList();
          for(int q = 0; q < colors.Length; q++)
          {
            for(int z = 0; z < concrete_model.Count; z++)
            {
              models.Add(new Car_model(brand[j], concrete_model[z].Item2, years[i], colors[q]));
            }
          }
        }
      }
      using(ApplicationContext db = new ApplicationContext())
      {
        db.Car_model.AddRange(models);
        if(db.SaveChanges() == 0) return false;
      }
      return true;
    }


    private string GenerateCarNumber()
    {
      Random random = new Random();
      string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
      string numbers = "0123456789";
      string car_number = "";
      for(int i = 0; i < 4; i++)
      {
        car_number += numbers[random.Next(numbers.Length - 1)];
      }
      car_number += " ";
      for(int i = 0; i < 2; i++)
      {
        car_number += letters[random.Next(letters.Length - 1)];
      }
      car_number += "-";
      car_number += numbers[random.Next(numbers.Length - 1)];

      return car_number;
    }
    private bool GenerateCars(List<Entities.Car_model> models)
    {
      using(ApplicationContext db = new ApplicationContext())
      {
        List<string> numbers = new List<string>();
        List<Car> cars = new List<Car>();
      for(int j = 0; j < db.Car_model.Count() * 2; j++)
      {
        int index = j % db.Car_model.Count();
        string car_number = "";
          bool flag = false;
        do
        {
          car_number = GenerateCarNumber();
          if(numbers.Where(i => i == (car_number)).Count() == 0)
          {
            numbers.Add(car_number);
              flag = true;
          }
        } while(!flag);
        cars.Add(new Car(models[index].ID, "Good", car_number));
        }
        db.Car.AddRange(cars);
        if(db.SaveChanges() == 0) return false;
      }
      return true;
    }

    private string GeneratePassportID()
    {
      Random random = new Random();
      string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
      string numbers = "0123456789";
      string passport_id = "";
      for(int i = 0; i < 7; i++)
      {
        passport_id += numbers[random.Next(numbers.Length - 1)];
      }
      passport_id += letters[random.Next(letters.Length - 1)];
      for(int i = 0; i < 3; i++)
      {
        passport_id += numbers[random.Next(numbers.Length - 1)];
      }
      for(int i = 0; i < 2; i++)
      {
        passport_id += letters[random.Next(letters.Length - 1)];
      }
      passport_id += numbers[random.Next(numbers.Length - 1)];
      return passport_id;
    }

    private bool GenerateLog(List<User> users)
    {
      DateTime end = DateTime.Now;
      Random random = new Random();
      List<Log> logs = new List<Log>();
      DateTime start = end.AddMonths(-3);
      for(; start != end; start = start.AddDays(1))
      {
        DateTime end_time = start.AddHours(6);
        logs.Add(new Entities.Log(users[random.Next(users.Count)].ID, start, end_time));
      }
      using(ApplicationContext db = new ApplicationContext())
      {
        db.Log.AddRange(logs);
        if(db.SaveChanges() == 0) return false;
      }
      return true;
    }

    private bool GenerateClients()
    {
      string numbers = "0123456789";
      List<Client> Clients = new List<Client>();
      string[] man_names = { "Денис", "Николай", "Александр", "Илья", "Михаил",
        "Андрей", "Константин", "Павел", "Артем", "Дмитрий" };

      string[] female_names = { "Елена", "Анастасия", "Ольга", "Юлия", "Ирина",
        "Ксения", "Дарья", "Екатерина", "Светлана", "Мария" };

      string[] second_female_names = { "Ростова", "Никитина", "Соловьева", "Ковалева", "Беляева",
        "Богданова", "Григорьева", "Кузнецова", "Шестакова", "Орлова" };

      string[] second_male_names = { "Круглов", "Соколов", "Ширяев", "Тихонов", "Макаров",
        "Львов", "Федотов", "Пономарев", "Дроздов", "Агафонов" };

      string[] last_female_names = { "Николаевна", "Андреевна", "Денисовна", "Викторовна", "Алексеевна",
        "Станиславовна", "Альбертовна", "Федоровна", "Арсеньевна", "Григорьевна" };

      string[] last_male_names = { "Николаевич", "Андреевич", "Денисович", "Викторович", "Алексеевич",
        "Станиславович", "Альбертович", "Федорович", "Арсеньевич", "Григорьевич" };

      for(int i = 0; i < man_names.Length; i++)
        for(int j = 0; j < second_male_names.Length; j++)
          for(int q = 0; q < last_male_names.Length; q++)
          {
            string name = $"{second_male_names[j]} {man_names[i]} {last_male_names[q]}";
            Random random = new Random();
            Clients.Add(new Entities.Client(name, random.Next(1000000+i+q, 9999999-j-q+i)));
          }
      using(ApplicationContext db = new ApplicationContext())
      {
        db.Client.AddRange(Clients);
        if(db.SaveChanges() == 0) return false;
      }
      return true;
    }

    private bool GeneratePrices()
    {
      using(ApplicationContext db = new ApplicationContext())
      {
        List<Entities.Car_model> models = db.Car_model.ToList();
        List<Price_list> prices = new List<Price_list>();
        Random random = new Random();
        foreach(Car_model model in models)
        {
          decimal price_in_hour = random.Next(30, 2000);
          decimal price_in_days = price_in_hour * Convert.ToDecimal(24) * Convert.ToDecimal(0.8);
          Entities.Price_list price = new Price_list(model.ID, price_in_hour, price_in_days);
          prices.Add(price);
        }
        db.Price_list.AddRange(prices);
        if(db.SaveChanges() == 0) return false;
      }
      return true;
    }

    private bool GenerateRentDeals()
    {
      using(ApplicationContext db = new ApplicationContext())
      {
        Random random = new Random();
        List<Log> logs = db.Log.ToList();
        List<User> users = db.User.ToList();
        List<Client> clients = db.Client.ToList();
        List<Car> cars = db.Car.ToList();
        DateTime end = DateTime.Now;
        for(DateTime start = end.AddMonths(-3); start != end;start = start.AddDays(1))
        {
          var log = logs.Where(i => i.Enter_time <= start && i.End_time >= start).ToList();
          int user_id = log[random.Next(log.Count - 1)].User_ID;
          int client_id = clients[random.Next(clients.Count - 1)].ID;
          int car_id_in_list = random.Next(cars.Count - 1);
          string car_id = cars[car_id_in_list].ID;
          Rent_deal deal = new Rent_deal(user_id, client_id, car_id, start, GeneratePassportID());
          db.Rent_deal.Add(deal);
          if(db.SaveChanges() == 0) return false;

          int deal_id = db.Rent_deal.Where(i => i == deal).Single().ID;
          int hours = random.Next(3, 2000);
          DateTime leave_car = start.AddHours(hours);
          Rent_details details = new Rent_details(deal_id, car_id, start, leave_car, leave_car);
          db.Rent_details.Add(details);
          if(db.SaveChanges() == 0) return false;

          Transaction transaction_from_user = new Transaction(user_id, start, true);
          Transaction transaction_from_client = new Transaction(client_id, start, false);
          db.Transaction.AddRange(transaction_from_user, transaction_from_client);
          if(db.SaveChanges() == 0) return false;

          //генерим доходность
          Price_list price = db.Price_list.Where(i => i.Model_ID == cars[car_id_in_list].Model_ID).Single();
          decimal earning = hours > 24 ? hours * price.Price_per_day : hours * price.Price_per_hour;
          Income income = new Income(start, earning);
          db.Income.Add(income);
          if(db.SaveChanges() == 0) return false;
          
        }
      }
        return true;
    }
    public bool GenerateDatabaseContent()
    {
      Database.EnsureDeleted();
      Database.EnsureCreated();
      List<User> users = new List<User>();
      // Adding users to DB
      if(!GenerateUsers(users))
        return false;

      List<Car_model> models = new List<Car_model>();
      //Adding Car models to DB
      if(!GenerateCarModels(models))
        return false;

      // Adding prices (no need models because i take it from db)
      // take from db because db set id value which i need 
      if(!GeneratePrices())
        return false;

      // Adding cars 
      if(!GenerateCars(models))
        return false;

      // Adding clients
      if(!GenerateClients())
        return false;

      if(!GenerateLog(users)) 
        return false;

      if(!GenerateRentDeals())
        return false;

      return true;
    }
  }
}
