using System.Linq;
using System.Windows;
using EquipmentAccountingIS.App.Data;
using Microsoft.EntityFrameworkCore;

namespace EquipmentAccountingIS.App.Windows;

/// <summary>
///     Interaction logic for AuthWindow.xaml
/// </summary>
public partial class AuthWindow : Window
{
    public AuthWindow()
    {
        InitializeComponent();
    }

    private void checkbPassHideShow_Checked(object sender, RoutedEventArgs e)
    {
        tbPassword.Text = pbPassword.Password;
        tbPassword.Visibility = Visibility.Visible;
        pbPassword.Visibility = Visibility.Hidden;
    }

    private void checkbPassHideShow_Unchecked(object sender, RoutedEventArgs e)
    {
        pbPassword.Password = tbPassword.Text;
        tbPassword.Visibility = Visibility.Hidden;
        pbPassword.Visibility = Visibility.Visible;
    }

    private void btLogin_Click(object sender, RoutedEventArgs e)
    {
        // Получаем контекст таблицы с сотрудниками, включая ссылки на объекты таблиц, связанных FK
        var respEmployees = EfModel.Init().ResponsibleEmployees.Include(p => p.PostNavigation)
            .Include(p => p.AccessTypeNavigation);
        // Находим объект, соответсвующий введённому логину и паролю 
        var ResponsibleEmployee = respEmployees.FirstOrDefault(u => u.EmployeeLogin == tbLogin.Text
                                                                    && (u.EmployeePassword == tbPassword.Text
                                                                        || u.EmployeePassword == pbPassword.Password));

        // Проверяем уровень доступа пользователя, и взависимости от этого открываем окно с возможностями
        if (ResponsibleEmployee != null)
        {
            var startWindow = new StartWindow(ResponsibleEmployee);
            startWindow.Show();
            Close();
        }
        else
        {
            MessageBox.Show("Некорректный логин или пароль", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            tbLogin.Clear();
            tbPassword.Clear();
            pbPassword.Clear();
        }
    }

    private void BtClose_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}