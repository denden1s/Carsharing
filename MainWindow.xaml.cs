using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Carsharing.Entities;
using testing_system.Classes;

namespace Carsharing
{
  public partial class MainWindow : Window
  {
    private Encryption encryption;
    private List<User> users;
    private Log log;
    private AnalyticWindows analyticWindow;
    private AdminWindows adminWindow;
    public MainWindow()
    {
        ApplicationContext ddd = new ApplicationContext();
        ddd.GenerateDatabaseContent();
            using (ApplicationContext db = new ApplicationContext())
      {
        users = db.User.ToList();
      } 
      encryption = new Encryption();
      InitializeComponent();
    }

    private void Button_login_Click(object sender, RoutedEventArgs e)
    {
      User current_user = users.Where(i => i.Login == TextBox_login.Text).SingleOrDefault();
      if(current_user != null)
      {
        if(current_user.Password == encryption.EncryptPassword(TextBox_password.Password, current_user.Login))
        {
          log = new Log(current_user.ID, DateTime.Now);
          if(current_user.Role == 1)
          {
            analyticWindow = new AnalyticWindows(log);
            this.Hide();
            analyticWindow.Show();
          }
          else if(current_user.Role == 2)
          {
            adminWindow = new AdminWindows(current_user, log);
            this.Hide();
            adminWindow.Show();
          }
        }
        else MessageBox.Show("Incorrect password!");
      }
      else MessageBox.Show("Incorrect login!");
    }

    private void Button_close_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => Application.Current.Shutdown();

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

        private void Button_login_MouseEnter(object sender, MouseEventArgs e) => Button_login.Background = new SolidColorBrush(Color.FromRgb(53, 230, 81));

    private void Button_login_MouseLeave(object sender, MouseEventArgs e) => Button_login.Background = new SolidColorBrush(Color.FromRgb(149, 240, 164));

    private void drag_and_drop_MouseDown(object sender, MouseButtonEventArgs e) => this.DragMove();
  }
}
