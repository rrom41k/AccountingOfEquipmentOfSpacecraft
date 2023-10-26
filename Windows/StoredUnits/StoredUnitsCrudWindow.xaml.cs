using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Aspose.Cells;
using EquipmentAccountingIS.App.Data;
using EquipmentAccountingIS.App.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace EquipmentAccountingIS.App.Windows.StoredUnits;

public partial class StoredUnitsCrudWindow : Window
{
    private readonly ResponsibleEmployee authorizedEmployee;

    public StoredUnitsCrudWindow(ResponsibleEmployee authorizedEmployee)
    {
        this.authorizedEmployee = authorizedEmployee;

        InitializeComponent();

        MakingSearchComboBox();
        // проверка на уровень доступа пользователя
        if (authorizedEmployee.AccessType == 2)
        {
            btHistory.Visibility = Visibility.Collapsed;
            btDelete.Visibility = Visibility.Collapsed;
        }

        RefreshDataGridData(); // Обновление данных таблицы
    }

    private void StoredUnitsCrudWindow_OnActivated(object? sender, EventArgs e)
    {
        if (tbSearchUnitName.Text == "" && cbSearchQuantity.SelectedIndex == 0 &&
            dpSearchDelivDateFrom.SelectedDate == null &&
            dpSearchDelivDateTo == null && cbSearchContract.SelectedIndex == 0 &&
            tbPurpose.Text == "" && cbSearchWhoAdd.SelectedIndex == 0)
            RefreshDataGridData();
    }

    private void BtBack_OnClick(object sender, RoutedEventArgs e)
    {
        var
            navigationMenuWindow = new NavigationMenuWindow(authorizedEmployee); // Создаём объект окна навигации
        navigationMenuWindow.Show(); // Выводим окно навигации
        Close(); // Закрываем текущее окно
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
                Include(p => p.WhoAddedNavigation). // Подключаем ссылки на данные таблицы ResponsibleEmployees
                Where(p => p.SignOfDeleting != "Y"); // Отсеивание удалённых записей

            dgStoredUnits.ItemsSource = data.ToList(); // Установка данных в DataGridView

            dgcbcContract.ItemsSource =
                context.SupplyContracts.ToList(); // Установка источника данных для ComboBox в DataGrid
            dgcbcContract.DisplayMemberPath = "SupplyContractCodename"; // Установка формата вывода данных в Combobox

            dgcbcWhoAdd.ItemsSource = context.ResponsibleEmployees.ToList();
            dgcbcWhoAdd.DisplayMemberPath = "EmployeeInitials";
        }
    }

    // Метод изменения данных в БД
    private void ChangeData()
    {
        try
        {
            using (var context = new EfModel())
            {
                var parse = 0;
                var errors = new StringBuilder();
                // Получение данных из DataGridView
                var data = dgStoredUnits.ItemsSource as List<StoredUnit>;
                // Изменение состояния сущностей на "изменено" (Modified)
                foreach (var item in data)
                {
                    if (!int.TryParse(Convert.ToString(item.Quantity), out parse))
                        errors.AppendLine(
                            $"У записи {item.UnitInfoNavigation.EquipmentName} неверно заполнено поле \"Количество\"!");
                    if (item.DeliveryDate == null)
                        errors.AppendLine(
                            $"У записи {item.UnitInfoNavigation.EquipmentName} неверно заполнено поле \"Дата доставки\"!");
                    if (item.Purpose == "")
                        errors.AppendLine(
                            $"У записи {item.UnitInfoNavigation.EquipmentName} неверно заполнено поле \"Назначение\"!");
                    if (errors.Length > 0)
                    {
                        MessageBox.Show(errors.ToString());
                        return;
                    }
                }

                // Изменение состояния сущностей на "изменено" (Modified)
                foreach (var item in data)
                {
                    item.SupplyContract = item.SupplyContractNavigation.SupplyContractId;
                    item.WhoAdded = item.WhoAddedNavigation.EmployeeId;
                    context.Entry(item.UnitInfoNavigation).State = EntityState.Modified;
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

    private void BtUpdate_OnClick(object sender, RoutedEventArgs e)
    {
        ChangeData();
        RefreshDataGridData();
    }

    private void BtDelete_OnClick(object sender, RoutedEventArgs e)
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

        var deleteStoredUnitWindow = new DeleteStoredUnitWindow(storedUnit);
        deleteStoredUnitWindow.ShowDialog();
        RefreshDataGridData();
    }

    private void BtHistory_OnClick(object sender, RoutedEventArgs e)
    {
        var deleateHistoryStoredUnitWindow = new DeleateHistoryStoredUnitWindow();
        deleateHistoryStoredUnitWindow.ShowDialog();
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
                    p.SignOfDeleting != "Y"); // Подключаем ссылки на данные таблицы ResponsibleEmployees
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
        }
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

    private void BtCreate_OnClick(object sender, RoutedEventArgs e)
    {
        var addStoredUnitWindow = new AddStoredUnitWindow(authorizedEmployee);
        addStoredUnitWindow.ShowDialog();
        RefreshDataGridData();
    }

    private void BtGenerateReport_OnClick(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Excel Files (*.xlsx;*.xls;*.csv;*.xlsm;*.xlsb)|*.xlsx;*.xls;*.csv;*.xlsm;*.xlsb",
            CheckFileExists = false,
            CheckPathExists = true,
            FileName = "Select Folder",
            Title = "Select Folder",
            ValidateNames = false
        };

        var result = openFileDialog.ShowDialog(); // Выводим окно для выбора файла изображения

        if (result == true)
        {
            var path = openFileDialog.FileName;
            Report(path);
        }
    }

    protected void Report(string path)
    {
        try
        {
            // Создайте экземпляр объекта Workbook, который представляет файл Excel.
            var wb = new Workbook();

            // Получите первый рабочий лист.
            var sheet = wb.Worksheets[0];

            var objectsList = dgStoredUnits.ItemsSource.Cast<StoredUnit>().ToList(); // Получение списка объектов

            // Запись заголовков
            sheet.Cells["A1"].PutValue("Наименование");
            sheet.Cells["B1"].PutValue("Количество");
            sheet.Cells["C1"].PutValue("Дата поставки");
            sheet.Cells["D1"].PutValue("Контракт поставки");
            sheet.Cells["E1"].PutValue("Назначение");
            sheet.Cells["F1"].PutValue("Добавил");
            // ...

            // Запись данных объектов
            var rowIndex = 2; // Начальная строка для данных
            foreach (var item in objectsList)
            {
                sheet.Cells["A" + rowIndex].PutValue(item.UnitInfoNavigation.EquipmentName);
                sheet.Cells["B" + rowIndex].PutValue(item.Quantity);
                sheet.Cells["C" + rowIndex].PutValue(item.DeliveryDate.ToString("yyyy-MM-dd HH:mm:ss"));
                sheet.Cells["D" + rowIndex].PutValue(item.SupplyContractNavigation.SupplyContractCodename);
                sheet.Cells["E" + rowIndex].PutValue(item.Purpose);
                sheet.Cells["F" + rowIndex].PutValue(item.WhoAddedNavigation.EmployeeInitials);
                rowIndex++;
            }

            wb.Save(path, SaveFormat.Xlsx);
        }
        catch (Exception e)
        {
            MessageBox.Show("Выберите длугое название файла");
        }
    }
}