using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Carsharing.Entities;
using Application = System.Windows.Application;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace Carsharing
{
  public partial class AnalyticWindows : Window
  {
    private Log log;
    private List<Rent_deal> deals;
    private List<Car_model> models;
    private List<Income> income;
  
    public AnalyticWindows(Log log)
    {
      InitializeComponent();
      this.log = log;
      Car_model model = new Car_model();
      models = model.LoadData();
      Income _income = new Income();
      income = _income.LoadData();
      income.OrderBy(i => i.Date);

      // Load income data in datagrid
      DataTable dataTable = new DataTable();
      dataTable.Columns.Add("Date");
      dataTable.Columns.Add("Value");
      foreach(var i in income)
      {
        DataRow dataRow1 = dataTable.NewRow();
        dataRow1["Date"] = i.Date.ToShortDateString();
        dataRow1["Value"] = i.Earning.ToString();
        dataTable.Rows.Add(dataRow1);
      }
      income_data_grid.ItemsSource = dataTable.DefaultView;
      string val = income.Sum(i => i.Earning).ToString();
      Full_income.Content = $"Full income: {val}";

      Rent_deal _deals = new Rent_deal();
      List<int> model_id = new List<int>(); 
      deals = _deals.LoadData();
      foreach(var i in deals)
      {
        Car car = i.GetCar();
        model_id.Add(car.Model_ID);
      }
      // Load data about car popularity in format: model - count
      var dict = model_id.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count()).OrderByDescending(x => x.Value);
      DataTable dataTable_pop = new DataTable();
      dataTable_pop.Columns.Add("Model");
      dataTable_pop.Columns.Add("Count");
      foreach(var i in dict)
      {
        DataRow dataRow1 = dataTable_pop.NewRow();
        dataRow1["Model"] = models.Where(q => q.ID == i.Key).SingleOrDefault().ToString();
        dataRow1["Count"] = i.Value.ToString();
        dataTable_pop.Rows.Add(dataRow1);
      }
      data_grid_popular.ItemsSource = dataTable_pop.DefaultView;

      foreach(Car_model m in models)
        m.SetPrice();

      UpdatePrice();
    }

    // Need to update price list after setting price on auto
    private void UpdatePrice()
    {
      Car_model model = new Car_model();
      models.Clear();
      models = model.LoadData();
      foreach(Car_model m in models)
        m.SetPrice();

      List<Car_model> unsetted_price = models.Where(i => i.Price.Price_per_hour == Decimal.Zero).ToList();
      List<Car_model> setted_price = models.Except(unsetted_price).ToList();
      Models_listBox.Items.Clear();
      ListBoxItem listBoxItem = new ListBoxItem();
      if(unsetted_price.Count > 0)
      {
        foreach(Car_model m in unsetted_price)
        {
          listBoxItem = new ListBoxItem();
          listBoxItem.Background = Brushes.Red;
          listBoxItem.Content = m.ToString();
          Models_listBox.Items.Add(listBoxItem);
        }
      }
      foreach(Car_model m in setted_price)
      {
        listBoxItem = new ListBoxItem();
        listBoxItem.Content = m.ToString();
        Models_listBox.Items.Add(listBoxItem);
      }
    }

    private void Button_close_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      log.End_time = DateTime.Now;
      log.Add();
      Application.Current.Shutdown();      
    }

    private void Button_close_MouseEnter(object sender, MouseEventArgs e)
    {
      Uri imageUri = new Uri("pack://application:,,,/img/close_icon.png");
      BitmapImage bitmapImage = new BitmapImage(imageUri);
      Button_close.Source = bitmapImage;
    }

    private void Button_close_MouseLeave(object sender, MouseEventArgs e)
    {
      Uri imageUri = new Uri("pack://application:,,,/img/close_icon.png");
      BitmapImage bitmapImage = new BitmapImage(imageUri);
      Button_close.Source = bitmapImage;
    }
    private void drag_and_drop_MouseDown(object sender, MouseButtonEventArgs e) => this.DragMove();

    private void Button_enterPrice_MouseEnter(object sender, MouseEventArgs e) => Button_enterPrice.Background = new SolidColorBrush(Color.FromRgb(53, 230, 81));

    private void Button_enterPrice_MouseLeave(object sender, MouseEventArgs e) => Button_enterPrice.Background = new SolidColorBrush(Color.FromRgb(149, 240, 164));

    private void Models_listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if(e.AddedItems.Count > 0)
      {
        ListBoxItem selectedItem = (ListBoxItem)e.AddedItems[0];
        string data = selectedItem.Content.ToString();
        Car_model current_model = models.Where(i => i.ToString().Equals(data)).SingleOrDefault();
        TextBox_price.Text = current_model.Price.Price_per_hour.ToString();
      }
    }

    private void Button_enterPrice_Click(object sender, RoutedEventArgs e)
    {
      var selectedItem = Models_listBox.SelectedItem.ToString();
      string[] split = selectedItem.Split(' ');
      Car_model current_model = models.Where(i => i.Brand == split[1] && i.Name == split[2] && i.Year == Convert.ToInt32(split[4]) && i.Color == split[3]).FirstOrDefault();
      current_model.Price.SetPrice(Convert.ToDecimal(TextBox_price.Text));
      using(ApplicationContext db = new ApplicationContext())
      {
        db.Price_list.Update(current_model.Price);
        db.SaveChanges();
      }
      TextBox_price.Clear();
      UpdatePrice();
    }
  }
}
