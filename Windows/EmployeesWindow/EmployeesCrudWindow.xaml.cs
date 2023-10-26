using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using EquipmentAccountingIS.App.Data;
using EquipmentAccountingIS.App.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EquipmentAccountingIS.App.Windows.EmployeesWindow;

public partial class EmployeesCrudWindow : Window
{
    private readonly ResponsibleEmployee authorizedEmployee;

    public EmployeesCrudWindow(ResponsibleEmployee authorizedEmployee)
    {
        this.authorizedEmployee = authorizedEmployee;
        InitializeComponent();
        MakingSearchComboBox();
        RefreshDataGridData();
    }

    private void MakingSearchComboBox()
    {
        using (var context = new EfModel()) // Создаём контекст данных БД
        {
            // Генерация полей для фильтрации по уровню доступа
            var dataAccessTypes = context.AccessTypes.ToList(); // Подключаем ссылки на данные таблицы Posts

            var accessType = new AccessType();
            accessType.AccessTypeName = "Без фильтрации";
            dataAccessTypes.Insert(0, accessType);
            cbAccessType.Items.Clear();

            foreach (var item in dataAccessTypes)
            {
                var comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = item.AccessTypeName;
                cbAccessType.Items.Add(comboBoxItem);
            }

            cbAccessType.SelectedIndex = 0;

            // Генерация полей для фильтрации по постам
            var dataPosts = context.Posts.ToList();

            var post = new Post();
            post.PostName = "Без фильтрации";
            dataPosts.Insert(0, post);
            cbPost.Items.Clear();

            foreach (var item in dataPosts)
            {
                var comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = item.PostName;
                cbPost.Items.Add(comboBoxItem);
            }

            cbPost.SelectedIndex = 0;
        }
    }

    // Метод обновления данных в таблице
    private void RefreshDataGridData()
    {
        using (var context = new EfModel()) // Создаём контекст данных БД
        {
            // Получение данных из базы данных
            var data = context.ResponsibleEmployees.Include(p => p.AccessTypeNavigation).Include(p => p.PostNavigation);

            dgEmployees.ItemsSource = data.ToList();
            dgcbcAccessType.ItemsSource =
                context.AccessTypes.ToList(); // Установка источника данных для ComboBox в DataGrid
            dgcbcAccessType.DisplayMemberPath = "AccessTypeName"; // Установка формата вывода данных в Combobox
            dgcbcPost.ItemsSource = context.Posts.ToList(); // Установка источника данных для ComboBox в DataGrid
            dgcbcPost.DisplayMemberPath = "PostName"; // Установка формата вывода данных в Combobox
        }
    }

    private void ChangeData()
    {
        try
        {
            using (var context = new EfModel())
            {
                var errors = new StringBuilder();
                // Получение данных из DataGridView
                var data = dgEmployees.ItemsSource as List<ResponsibleEmployee>;
                // Изменение состояния сущностей на "изменено" (Modified)
                foreach (var item in data)
                {
                    if (item.EmployeeLogin == "")
                        errors.AppendLine($"У записи {item.EmployeeInitials} не заполнено поле \"Логин\"!");
                    if (item.EmployeePassword == "")
                        errors.AppendLine($"У записи {item.EmployeeInitials} не заполнено поле \"Пароль\"!");
                    if (item.EmployeeFirstName == "")
                        errors.AppendLine($"У записи {item.EmployeeInitials} не заполнено поле \"Имя\"!");
                    if (item.EmployeeSecondName == "")
                        errors.AppendLine($"У записи {item.EmployeeInitials} не заполнено поле \"Фамилия\"!");
                    if (errors.Length > 0)
                    {
                        MessageBox.Show(errors.ToString());
                        return;
                    }

                    item.AccessType = item.AccessTypeNavigation.AccessTypeId;
                    item.Post = item.PostNavigation.PostId;
                    if (item.EmployeeLastName == "" || item.EmployeeLastName == null)
                        item.EmployeeInitials =
                            $"{item.EmployeeSecondName} {Convert.ToString(item.EmployeeFirstName[0])}.";
                    else
                        item.EmployeeInitials =
                            $"{item.EmployeeSecondName} {Convert.ToString(item.EmployeeFirstName[0])}. {Convert.ToString(item.EmployeeLastName[0])}.";
                    context.Entry(item).State = EntityState.Modified;
                }

                // Сохранение изменений в базу данных
                context.SaveChanges();
                MessageBox.Show("Данные успешно изменены!");
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
        }
    }

    private void FIlter()
    {
        using (var context = new EfModel()) // Создаём контекст данных БД
        {
            var data = context.ResponsibleEmployees.Include(p => p.AccessTypeNavigation).Include(p => p.PostNavigation)
                .Where(p => p.EmployeeLogin.Contains(tbEmployeeLogin.Text) &&
                            p.EmployeePassword.Contains(tbEmployeePassword.Text) &&
                            p.EmployeeFirstName.Contains(tbEmployeeFirstName.Text) &&
                            p.EmployeeSecondName.Contains(tbEmployeeSecondName.Text) &&
                            p.EmployeeLastName.Contains(tbEmployeeLastName.Text));

            if (cbAccessType.SelectedIndex != 0)
                data = data.Where(p => p.AccessType == cbAccessType.SelectedIndex);

            if (cbPost.SelectedIndex != 0)
                data = data.Where(p => p.Post == cbPost.SelectedIndex);

            dgEmployees.ItemsSource = data.ToList();
            dgcbcAccessType.ItemsSource =
                context.AccessTypes.ToList(); // Установка источника данных для ComboBox в DataGrid
            dgcbcAccessType.DisplayMemberPath = "AccessTypeName"; // Установка формата вывода данных в Combobox
            dgcbcPost.ItemsSource = context.Posts.ToList(); // Установка источника данных для ComboBox в DataGrid
            dgcbcPost.DisplayMemberPath = "PostName"; // Установка формата вывода данных в Combobox
        }
    }

    private void BtUpdate_OnClick(object sender, RoutedEventArgs e)
    {
        ChangeData();
        RefreshDataGridData();
    }

    private void BtCreate_OnClick(object sender, RoutedEventArgs e)
    {
        var addEmployeeWindow = new AddEmployeeWindow();
        addEmployeeWindow.ShowDialog();
        RefreshDataGridData();
    }

    private void BtDelete_OnClick(object sender, RoutedEventArgs e)
    {
        var errors = new StringBuilder();
        if (dgEmployees.SelectedItem == null)
            errors.AppendLine("Вы не выбрали запись!");

        if (errors.Length > 0)
        {
            MessageBox.Show(errors.ToString());
            return;
        }


        if (MessageBox.Show("Вы точно хотите удалить эту запись?", "Внимание", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
            try
            {
                var responsibleEmployee =
                    dgEmployees.SelectedItem as ResponsibleEmployee; // Получаем выбранную запись в DataGrid
                using (var context = new EfModel())
                {
                    context.ResponsibleEmployees.Remove(responsibleEmployee);
                    context.SaveChanges();
                    MessageBox.Show("Запись успешно удалена!");
                }

                RefreshDataGridData();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                MessageBox.Show(
                    "Нельзя удалить эту запись потому что к ней привязаны записи из окна StoredUnitsCrudWindow");
            }
    }

    private void BtBack_OnClick(object sender, RoutedEventArgs e)
    {
        var navigationMenuWindow = new NavigationMenuWindow(authorizedEmployee);
        navigationMenuWindow.Show();
        Close();
    }

    private void BtSearch_OnClick(object sender, RoutedEventArgs e)
    {
        FIlter();
    }

    private void BtClearSearch_OnClick(object sender, RoutedEventArgs e)
    {
        tbEmployeeLogin.Text = null;
        tbEmployeePassword.Text = null;
        tbEmployeeFirstName.Text = null;
        tbEmployeeSecondName.Text = null;
        tbEmployeeLastName.Text = null;
        cbAccessType.SelectedIndex = 0;
        cbPost.SelectedIndex = 0;
        RefreshDataGridData();
    }
}