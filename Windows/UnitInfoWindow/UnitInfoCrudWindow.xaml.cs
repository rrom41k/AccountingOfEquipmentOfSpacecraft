using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using EquipmentAccountingIS.App.Data;
using EquipmentAccountingIS.App.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace EquipmentAccountingIS.App.Windows.UnitInfoWindow;

public partial class UnitInfoCrudWindow : Window
{
    private readonly ResponsibleEmployee authorizedEmployee;

    public UnitInfoCrudWindow(ResponsibleEmployee authorizedEmployee)
    {
        this.authorizedEmployee = authorizedEmployee;
        InitializeComponent();
        // проверка на уровень доступа пользователя
        if (authorizedEmployee.AccessType == 2) btDelete.Visibility = Visibility.Collapsed;
        MakingSearchComboBox();
        RefreshDataGridData();
    }

    // Метод обновления данных в таблице
    private void RefreshDataGridData()
    {
        using (var context = new EfModel()) // Создаём контекст данных БД
        {
            // Получение данных из базы данных
            var data = context.UnitInfos.Include(p => p.EquipmentTypeNavigation);

            dgUnitInfo.ItemsSource = data.ToList(); // Установка данных в DataGridView

            dgcbcType.ItemsSource =
                context.EquipmentTypes.ToList(); // Установка источника данных для ComboBox в DataGrid
            dgcbcType.DisplayMemberPath = "EquipmentTypeName"; // Установка формата вывода данных в Combobox
        }
    }

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

            var dataEquipType = context.EquipmentTypes.ToList();

            var equipmentType = new EquipmentType();
            equipmentType.EquipmentTypeName = "Без фильтрации";
            dataEquipType.Insert(0, equipmentType);
            cbEquipType.Items.Clear();
            foreach (var item in dataEquipType)
            {
                var comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = item.EquipmentTypeName;
                cbEquipType.Items.Add(comboBoxItem);
            }

            cbEquipType.SelectedIndex = 0;
        }
    }

    private void ChangeData()
    {
        try
        {
            using (var context = new EfModel())
            {
                // Получение данных из DataGridView
                var data = dgUnitInfo.ItemsSource as List<UnitInfo>;
                var errors = new StringBuilder();
                // Изменение состояния сущностей на "изменено" (Modified)
                foreach (var item in data)
                {
                    if (item.Photo == null)
                        try
                        {
                            var imagePath = "Resources\\"; // Получаем путь к фалу ихображения
                            // Создаем новый экземпляр BitmapImage с новым путем к файлу
                            var newImageSource = new BitmapImage(new Uri(imagePath));
                            var imageData =
                                ByteArrayToImageConverter.CompressImage(imagePath,
                                    24000); // Сжимаем и конвертируем изображение в массив байтов
                            item.Photo = imageData; // Записываем данные в объект БД
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message);
                        }

                    if (item.EquipmentName == "")
                        errors.AppendLine("У одной из записей было оставлено пустое поле \"Наименование\"");
                    if (item.EquipmentCodename == "")
                        errors.AppendLine($"У записи \"{item.EquipmentName}\" было оставлено пустое поле \"Код\"");
                    if (item.EquipmentPassport == "")
                        errors.AppendLine($"У записи \"{item.EquipmentName}\" было оставлено пустое поле \"Паспорт\"");
                    if (errors.Length > 0)
                    {
                        MessageBox.Show(errors.ToString());
                        return;
                    }

                    item.EquipmentType = item.EquipmentTypeNavigation.EquipmentTypeId;
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
            var data = context.UnitInfos.Where(p => p.EquipmentName.Contains(tbSearchUnitName.Text) &&
                                                    p.EquipmentCodename.Contains(tbCode.Text) &&
                                                    p.EquipmentPassport.Contains(tbPasport.Text));

            if (cbEquipType.SelectedIndex != 0)
                data = data.Where(p => p.EquipmentType == cbEquipType.SelectedIndex);

            dgUnitInfo.ItemsSource = data.ToList();
            dgcbcType.ItemsSource =
                context.EquipmentTypes.ToList(); // Установка источника данных для ComboBox в DataGrid
            dgcbcType.DisplayMemberPath = "EquipmentTypeName"; // Установка формата вывода данных в Combobox
        }
    }

    private void BtUpdate_OnClick(object sender, RoutedEventArgs e)
    {
        ChangeData();
        RefreshDataGridData();
    }

    private void BtCreate_OnClick(object sender, RoutedEventArgs e)
    {
        var addUnitInfoWindow = new AddUnitInfoWindow();
        addUnitInfoWindow.ShowDialog();
        RefreshDataGridData();
    }

    private void BtDelete_OnClick(object sender, RoutedEventArgs e)
    {
        var errors = new StringBuilder();
        if (dgUnitInfo.SelectedItem == null)
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
                var unitInfo = dgUnitInfo.SelectedItem as UnitInfo; // Получаем выбранную запись в DataGrid
                using (var context = new EfModel())
                {
                    context.UnitInfos.Remove(unitInfo);
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


    private void BtSearch_OnClick(object sender, RoutedEventArgs e)
    {
        FIlter();
    }

    private void BtClearSearch_OnClick(object sender, RoutedEventArgs e)
    {
        tbSearchUnitName.Text = "";
        tbCode.Text = "";
        tbPasport.Text = "";
        cbEquipType.SelectedIndex = 0;
        RefreshDataGridData();
    }

    private void BtBack_OnClick(object sender, RoutedEventArgs e)
    {
        var navigationMenuWindow = new NavigationMenuWindow(authorizedEmployee);
        navigationMenuWindow.Show();
        Close();
    }

    private void BtChangeImg_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var errors = new StringBuilder();
            if (dgUnitInfo.SelectedItem == null)
                errors.AppendLine("Вы не выбрали деталь!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            var unitInfo = dgUnitInfo.SelectedItem as UnitInfo; // Получаем выбранную запись в DataGrid


            var openFileDialog = new OpenFileDialog(); // Создаём объект окна для выбора файла изображения
            openFileDialog.Filter =
                "Image Files (*.png;*.jpg;*.jpeg;*.gif)|*.png;*.jpg;*.jpeg;*.gif"; // Установка фильтра файлов
            var result = openFileDialog.ShowDialog(); // Выводим окно для выбора файла изображения

            if (result == true)
            {
                var imagePath = openFileDialog.FileName; // Получаем путь к фалу ихображения
                var imageData =
                    ByteArrayToImageConverter.CompressImage(imagePath,
                        24000); // Сжимаем и конвертируем изображение в массив байтов
                unitInfo.Photo = imageData; // Записываем данные в объект БД
                using (var context = new EfModel()) // Создаём контекст данных
                {
                    context.Entry(unitInfo).State = EntityState.Modified; // Изменение данных в контексте
                    context.SaveChanges(); // Сохранение изменений в базу данных
                    MessageBox.Show("Данные успешно изменены!");
                }
            }

            RefreshDataGridData();
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
        }
    }
}