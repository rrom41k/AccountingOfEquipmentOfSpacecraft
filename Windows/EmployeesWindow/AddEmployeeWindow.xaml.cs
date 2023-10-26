using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using EquipmentAccountingIS.App.Data;
using EquipmentAccountingIS.App.Data.Entities;

namespace EquipmentAccountingIS.App.Windows.EmployeesWindow;

public partial class AddEmployeeWindow : Window
{
    private readonly ResponsibleEmployee _responsibleEmployee = new();

    public AddEmployeeWindow()
    {
        InitializeComponent();
        MakingSearchComboBox();
    }

    private void MakingSearchComboBox()
    {
        using var context = new EfModel();
        // Генерация полей для фильтрации по уровню доступа
        var dataAccessTypes = context.AccessTypes.ToList(); // Подключаем ссылки на данные таблицы Posts

        foreach (var item in dataAccessTypes)
        {
            var comboBoxItem = new ComboBoxItem();
            comboBoxItem.Content = item.AccessTypeName;
            cbAccessType.Items.Add(comboBoxItem);
        }

        cbAccessType.SelectedIndex = 0;

        // Генерация полей для фильтрации по постам
        var dataPosts = context.Posts.ToList();

        foreach (var item in dataPosts)
        {
            var comboBoxItem = new ComboBoxItem();
            comboBoxItem.Content = item.PostName;
            cbPost.Items.Add(comboBoxItem);
        }

        cbPost.SelectedIndex = 0;
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var errors = new StringBuilder();
            if (tbEmployeeLogin.Text == "")
                errors.AppendLine("Вы не ввели логин!");
            if (tbEmployeePassword.Text == "")
                errors.AppendLine("Вы не ввели пароль!");
            if (tbEmployeeFirstName.Text == "")
                errors.AppendLine("Вы не ввели имя!");
            if (tbEmployeePassword.Text == "")
                errors.AppendLine("Вы не ввели фамилию!");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            _responsibleEmployee.EmployeeLogin = tbEmployeeLogin.Text;
            _responsibleEmployee.EmployeePassword = tbEmployeePassword.Text;
            _responsibleEmployee.EmployeeFirstName = tbEmployeeFirstName.Text;
            _responsibleEmployee.EmployeeSecondName = tbEmployeeSecondName.Text;
            _responsibleEmployee.EmployeeLastName = tbEmployeeLastName.Text;
            if (tbEmployeeLastName.Text == "")
                _responsibleEmployee.EmployeeInitials = $"{_responsibleEmployee.EmployeeSecondName} " +
                                                        $"{Convert.ToString(_responsibleEmployee.EmployeeFirstName[0])}. " +
                                                        $"{Convert.ToString(_responsibleEmployee.EmployeeLastName[0])}.";
            else
                _responsibleEmployee.EmployeeInitials = $"{_responsibleEmployee.EmployeeSecondName} " +
                                                        $"{Convert.ToString(_responsibleEmployee.EmployeeFirstName[0])}.";
            _responsibleEmployee.EmployeeInitials = $"{_responsibleEmployee.EmployeeSecondName} " +
                                                    $"{Convert.ToString(_responsibleEmployee.EmployeeFirstName[0])}. " +
                                                    $"{Convert.ToString(_responsibleEmployee.EmployeeLastName[0])}.";
            _responsibleEmployee.AccessType = cbAccessType.SelectedIndex + 1;
            _responsibleEmployee.Post = cbPost.SelectedIndex + 1;
            using (var context = new EfModel())
            {
                context.ResponsibleEmployees.Add(_responsibleEmployee);
                context.SaveChanges();
                MessageBox.Show("Запись успешно добавлена");
                Close();
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    private void BtBack_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}