using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Carsharing.Entities;

namespace Carsharing
{
  public partial class AdminWindows : Window
  {
    private User admin;
    private Log log;
    private List<Client> clients;
    private List<Car_model> models;
    private List<Car_model> free_models;
    private List<Rent_details> details;
    private List<Price_list> prices;
    List<Car> free_cars;
    public AdminWindows(User user, Log log)
    {
      InitializeComponent();
      admin = user;
      this.log = log;
      // Models list contain only free to rent models
      UpdateModelsList();
      Client client = new Client();
      clients = client.LoadData();
      foreach(var i in clients)
        Clients_listBox.Items.Add(i.Name);
      
      Price_list price = new Price_list();
      prices = price.LoadData();
      ShowAddingClientElements(Visibility.Visible, Visibility.Hidden);
    }

    // Need to view ui elements for adding new client they switch with client listbox
    private void ShowAddingClientElements(Visibility ClientList, Visibility AddingList)
    {
      Client_passport.Visibility = AddingList;
      Client_number.Visibility = AddingList;
      Client_number1.Visibility = AddingList;
      Client_passport1.Visibility = AddingList;
      Client_name.Visibility = AddingList;
      Client_name1.Visibility = AddingList;
      Button_add_client.Visibility = AddingList;

      Clients_listBox.Visibility = ClientList;
      search_client.Visibility = ClientList;
      Search_client_textBox.Visibility = ClientList;
    }

    private void drag_and_drop_MouseDown(object sender, MouseButtonEventArgs e) => this.DragMove();

    // Need to update models list after creating rent deal
    private void UpdateModelsList()
    {
      Car_model model = new Car_model();
      Rent_details detail = new Rent_details();
      models = model.LoadData();
      details = detail.LoadData();
      foreach(var item in details)
        item.LoadCarData();
      
      Car car = new Car();
      List<Car> cars = car.LoadData();
      free_cars = new List<Car>();
      foreach(var item in cars)
        if(details.Where(i => i.Car_ID == item.ID && i.TakingTime < DateTime.Now && i.Expected_return_time < DateTime.Now).Count() != 0)
          free_cars.Add(item);

      free_models = new List<Car_model>();
      foreach(var item in free_cars)
      {
        item.LoadModel();
        if(free_models.Where(i => i.ID == item.Model_ID).Count() == 0)
          free_models.Add(item.Model);
      }
      Models_listBox.Items.Clear();
      free_models = free_models.OrderBy(i => i.ToString()).ToList();
      foreach(var i in free_models)
        Models_listBox.Items.Add(i.ToString());
    }

    private void Button_close_MouseEnter(object sender, MouseEventArgs e)
    {
      Uri imageUri = new Uri("pack://application:,,,/img/close icon on.png");
      BitmapImage bitmapImage = new BitmapImage(imageUri);
      Button_close.Source = bitmapImage;
    }

    private void Button_close_MouseLeave(object sender, MouseEventArgs e)
    {
      Uri imageUri = new Uri("pack://application:,,,/img/close_icon.png");
      BitmapImage bitmapImage = new BitmapImage(imageUri);
      Button_close.Source = bitmapImage;
    }

    private void Button_close_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      log.End_time = DateTime.Now;
      log.Add();
      Application.Current.Shutdown();
    }

    // Searching work without case sensitivity and find substring in string
    private void Search_client_textBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      if(Search_client_textBox.Text == "")
      {
        Clients_listBox.Items.Clear();
        foreach(var i in clients)
          Clients_listBox.Items.Add(i.Name);
      }
      else
      {
        Clients_listBox.Items.Clear();
        foreach(var i in clients.Where(i => i.Name.ToLower().Contains(Search_client_textBox.Text.ToLower())))
          Clients_listBox.Items.Add(i.Name);
      }
    }

    // Searching work without case sensitivity and find substring in string
    private void Search_car_textBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      if(Search_car_textBox.Text == "")
      {
        Models_listBox.Items.Clear();
        foreach(var i in free_models)
          Models_listBox.Items.Add(i.ToString());
      }
      else
      {
        Models_listBox.Items.Clear();
        foreach(var i in free_models.Where(i => i.ToString().ToLower().Contains(Search_car_textBox.Text.ToLower())))
          Models_listBox.Items.Add(i.ToString());
      }
    }

    private void Button_new_client_MouseEnter(object sender, MouseEventArgs e) => Button_new_client.Background = new SolidColorBrush(Color.FromRgb(53, 230, 81));

    private void Button_rent_MouseEnter(object sender, MouseEventArgs e) => Button_rent.Background = new SolidColorBrush(Color.FromRgb(53, 230, 81));

    private void Button_rent_MouseLeave(object sender, MouseEventArgs e) => Button_rent.Background = new SolidColorBrush(Color.FromRgb(149, 240, 164));

    private void Button_new_client_MouseLeave(object sender, MouseEventArgs e) => Button_new_client.Background = new SolidColorBrush(Color.FromRgb(149, 240, 164));

    private void Button_new_client_Click(object sender, RoutedEventArgs e)
    {
      if(Button_new_client.Content.Equals("Add client"))
      {
        ShowAddingClientElements(Visibility.Hidden, Visibility.Visible);
        Client_passport.Text = "";
        Client_number.Text = "";
        Client_name.Text = "";
        Button_new_client.Content = "Hide";
      }
      else
      {
        ShowAddingClientElements(Visibility.Visible, Visibility.Hidden);
        Button_new_client.Content = "Add client";
      }
    }

    private void Button_rent_Click(object sender, RoutedEventArgs e)
    {
      if(Models_listBox.SelectedIndex != -1)
      {
        if(Clients_listBox.SelectedIndex != -1)
        {
          Client client = clients.Where(i => i.Name == Clients_listBox.SelectedItem.ToString()).SingleOrDefault();
          Car_model model = free_models.Where(i => i.ToString() == Models_listBox.SelectedItem.ToString()).FirstOrDefault();
          string car_id = free_cars.Where(i => model.ID == i.Model_ID).FirstOrDefault().ID;
          int hours;
          bool isNumber = int.TryParse(Hours_textBox.Text, out hours);
          if(client.ID != 0 && client.Passport != string.Empty && car_id != string.Empty && hours != 0 && isNumber)
          {
            DateTime day = DateTime.Now;
            Rent_deal new_deal = new Rent_deal(admin.ID, client.ID, car_id, day, client.Passport);
            Rent_details detail = new Rent_details(new_deal.ID, car_id, day, day.AddHours(hours), day.AddHours(hours));
            new_deal.Details = detail;
            new_deal.Add();
            decimal price = hours >= 24 ? prices.Where(i => i.Model_ID == model.ID).FirstOrDefault().Price_per_hour : prices.Where(i => i.Model_ID == model.ID).FirstOrDefault().Price_per_day;
            price *= hours;
            Income income = new Income(day, price);
            income.Add();
            Transaction transaction = new Transaction(admin.ID, day, true);
            Transaction transaction_form_client = new Transaction(client.ID, day, false);
            transaction.Add();
            transaction_form_client.Add();
            MessageBox.Show($"Car: {model.ToString()} with number: {car_id} was given to rent for " +
              $"{client.Name} on {hours} hours.\n Price: {price} bel rub.");
            UpdateModelsList();
          }
          else MessageBox.Show("Cant create rent deal!");
        }
        else MessageBox.Show("You need select a client!");
      }
      else MessageBox.Show("You need select a car!");
    }

    private void Button_add_client_MouseEnter(object sender, MouseEventArgs e) => Button_add_client.Background = new SolidColorBrush(Color.FromRgb(53, 230, 81));

    private void Button_add_client_MouseLeave(object sender, MouseEventArgs e) => Button_add_client.Background = new SolidColorBrush(Color.FromRgb(149, 240, 164));

    private void Button_add_client_Click(object sender, RoutedEventArgs e)
    {
      if(Client_name.Text != string.Empty && Client_number.Text != string.Empty && Client_passport.Text != string.Empty) 
      {
        int number;
        bool isNumber = int.TryParse(Client_number.Text, out number);
        if(isNumber)
        {
          using(ApplicationContext db = new ApplicationContext())
          {
            int identical = db.Client.Where(i => i.Role == 3 && i.Name.Equals(Client_name.Text)).Count();
            if(identical > 1)
              MessageBox.Show("This client contains in data base");
            else
            {
              Client client = new Client(Client_name.Text, number);
              client.Passport = Client_passport.Text;
              client.Add();
              MessageBox.Show("Client was added");
              clients.Clear();
              clients = client.LoadData();
              Clients_listBox.Items.Clear();
              foreach(var i in clients)
                Clients_listBox.Items.Add(i.Name);
              
              Button_new_client_Click(sender, e);
            }
          }
        }
        else MessageBox.Show("Number invalid");
      }
      else MessageBox.Show("Cant add client!");
    }

    // Need to generate rent cost before creating rent
    private void Hours_textBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      int number = 0;
      bool tryParse = int.TryParse(Hours_textBox.Text, out number);
      if(tryParse)
      {
        if(Models_listBox.SelectedItem != null)
        {
          string selectedValue = Models_listBox.SelectedItem.ToString();

          int model_id = free_models.Where(i => i.ToString() == selectedValue).FirstOrDefault().ID;
          if(model_id != 0)
          {
            decimal price = number >= 24 ? prices.Where(i => i.Model_ID == model_id).FirstOrDefault().Price_per_hour : prices.Where(i => i.Model_ID == model_id).FirstOrDefault().Price_per_day;
            price *= number;
            Price_label.Content = $"Price: {price.ToString()}";
          }
          else Price_label.Content = $"Price: cant find car";
        }
        else Price_label.Content = $"Price: set car model";

      }
      else Price_label.Content = $"Price: cant calculate";
    }
  }
}
