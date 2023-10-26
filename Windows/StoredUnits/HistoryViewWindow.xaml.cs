using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using EquipmentAccountingIS.App.Data;
using EquipmentAccountingIS.App.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EquipmentAccountingIS.App.Windows.StoredUnits;

public partial class DeleateHistoryStoredUnitWindow : Window
{
    public DeleateHistoryStoredUnitWindow()
    {
        InitializeComponent();
        MakingSearchComboBox();
        RefreshDataGridData();
    }

    // Генерируем поля для фильтрации
    private void MakingSearchComboBox()
    {
        using (var context = new EfModel()) // Создаём контекст данных БД
        {
            // Получение данных из базы данных
            var dataContract = context.SupplyContracts.Include(p =>
                p.ManufacturerShortNameNavigation).ToList(); // Подключаем ссылки на данные таблицы SupplyContract
            var supplyContract = new SupplyContract();
            supplyContract.SupplyContractCodename = "Без фильтрации";
            dataContract.Insert(0, supplyContract);

            var dataWhoAdd = context.ResponsibleEmployees.Include(p =>
                    p.PostNavigation). // Подключаем ссылки на данные таблицы ResponsibleEmployees
                Include(p => p.AccessTypeNavigation).ToList();

            var responsibleEmployee = new ResponsibleEmployee();
            responsibleEmployee.EmployeeInitials = "Без фильтрации";
            dataWhoAdd.Insert(0, responsibleEmployee);
            cbSearchWhoAdd.Items.Clear();
            foreach (var item in dataWhoAdd)
            {
                var comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = item.EmployeeInitials;
                cbSearchWhoAdd.Items.Add(comboBoxItem);
            }

            // Добавляем поля в ComboBox поска по контракту
            cbSearchContract.ItemsSource = dataContract;
            cbSearchContract.DisplayMemberPath = "SupplyContractCodename";
            cbSearchContract.SelectedIndex = 0;
            cbSearchWhoAdd.SelectedIndex = 0;
            // Добавляем поля в ComboBox поиска по автору добавления
            /*cbSearchWhoAdd.ItemsSource = dataWhoAdd;
            cbSearchWhoAdd.DisplayMemberPath = "EmployeeInitials";*/
        }
    }

    private void FIlter()
    {
        using (var context = new EfModel()) // Создаём контекст данных БД
        {
            // Получение данных из базы данных
            var data = context.StoredUnits.Include(p => p.UnitInfoNavigation)
                . // Подключаем ссылки на данные таблицы UnitInfo
                Include(p => p.SupplyContractNavigation). // Подключаем ссылки на данные таблицы SupplyContract
                Include(p => p.WhoAddedNavigation).Where(p =>
                    p.UnitInfoNavigation.EquipmentName.Contains(tbSearchUnitName.Text) &&
                    p.Purpose.Contains(tbPurpose.Text) &&
                    p.SignOfDeleting == "Y"); // Подключаем ссылки на данные таблицы ResponsibleEmployees
            switch (cbSearchQuantity.SelectedIndex)
            {
                case 1:
                    data = data.Where(p => p.Quantity < 10);
                    break;
                case 2:
                    data = data.Where(p => 10 <= p.Quantity && p.Quantity < 25);
                    break;
                case 3:
                    data = data.Where(p => 25 <= p.Quantity && p.Quantity < 100);
                    break;
                case 4:
                    data = data.Where(p => 100 <= p.Quantity);
                    break;
            }

            if (dpSearchDelivDateFrom.SelectedDate != null)
                data = data.Where(p => dpSearchDelivDateFrom.SelectedDate <= p.DeliveryDate);
            if (dpSearchDelivDateTo.SelectedDate != null)
                data = data.Where(p => p.DeliveryDate <= dpSearchDelivDateTo.SelectedDate);
            if (cbSearchContract.SelectedIndex != 0 && cbSearchContract.SelectedIndex != -1)
                data = data.Where(p => p.SupplyContractNavigation == cbSearchContract.SelectedItem);
            if (cbSearchWhoAdd.SelectedIndex != 0 && cbSearchWhoAdd.SelectedIndex != -1)
                data = data.Where(p => p.WhoAdded == cbSearchWhoAdd.SelectedIndex);

            dgStoredUnits.ItemsSource = data.ToList(); // Установка данных в DataGridView

            MakingSearchComboBox();
            dgcbcContract.ItemsSource =
                context.SupplyContracts.ToList(); // Установка источника данных для ComboBox в DataGrid
            dgcbcContract.DisplayMemberPath = "SupplyContractCodename"; // Установка формата вывода данных в Combobox

            dgcbcWhoAdd.ItemsSource = context.ResponsibleEmployees.ToList();
            dgcbcWhoAdd.DisplayMemberPath = "EmployeeInitials";

            // прошлая, почему-то не рабочая версия
            /*dgcbcContract.ItemsSource = context.SupplyContracts.ToList(); // Установка источника данных для ComboBox в DataGrid
            dgcbcContract.DisplayMemberPath = "SupplyContractCodename"; // Установка формата вывода данных в Combobox

            cbSearchWhoAdd.ItemsSource = context.ResponsibleEmployees.ToList(); // Установка источника данных для ComboBox в DataGrid
            cbSearchWhoAdd.DisplayMemberPath = "EmployeeInitials"; // Установка формата вывода данных в Combobox*/
        }
    }

    // Метод обновления данных в таблице
    private void RefreshDataGridData()
    {
        using (var context = new EfModel()) // Создаём контекст данных БД
        {
            // Получение данных из базы данных
            var data = context.StoredUnits.Include(p => p.UnitInfoNavigation)
                . // Подключаем ссылки на данные таблицы UnitInfo
                Include(p => p.SupplyContractNavigation). // Подключаем ссылки на данные таблицы SupplyContract
                Include(p => p.WhoAddedNavigation)
                .Where(p => p.SignOfDeleting == "Y"); // Подключаем ссылки на данные таблицы ResponsibleEmployees

            dgStoredUnits.ItemsSource = data.ToList(); // Установка данных в DataGridView

            dgcbcContract.ItemsSource =
                context.SupplyContracts.ToList(); // Установка источника данных для ComboBox в DataGrid
            dgcbcContract.DisplayMemberPath = "SupplyContractCodename"; // Установка формата вывода данных в Combobox

            dgcbcWhoAdd.ItemsSource = context.ResponsibleEmployees.ToList();
            dgcbcWhoAdd.DisplayMemberPath = "EmployeeInitials";
        }
    }

    private void BtRecovery_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var errors = new StringBuilder();
            if (dgStoredUnits.SelectedItem == null)
                errors.AppendLine("Вы не выбрали деталь!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            var storedUnit = dgStoredUnits.SelectedItem as StoredUnit; // Получаем выбранную запись в DataGrid

            storedUnit.SignOfDeleting = null;
            storedUnit.DateOfWriteOff = null;
            storedUnit.Note = null;

            using (var context = new EfModel()) // Создаём контекст данных
            {
                context.Entry(storedUnit).State = EntityState.Modified; // Изменение данных в контексте
                context.SaveChanges(); // Сохранение изменений в базу данных
                MessageBox.Show("Данные успешно изменены!");
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
        }
    }

    private void BtBack_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void BtSearch_OnClick(object sender, RoutedEventArgs e)
    {
        FIlter();
    }

    private void BtClearSearch_OnClick(object sender, RoutedEventArgs e)
    {
        tbSearchUnitName.Text = "";
        cbSearchQuantity.SelectedIndex = 0;
        dpSearchDelivDateFrom.SelectedDate = null;
        dpSearchDelivDateTo.SelectedDate = null;
        cbSearchContract.SelectedIndex = 0;
        tbPurpose.Text = "";
        cbSearchWhoAdd.SelectedIndex = 0;
        RefreshDataGridData();
    }
}