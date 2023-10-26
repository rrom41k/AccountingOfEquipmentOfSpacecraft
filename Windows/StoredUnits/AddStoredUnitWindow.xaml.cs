using System;
using System.Linq;
using System.Text;
using System.Windows;
using EquipmentAccountingIS.App.Data;
using EquipmentAccountingIS.App.Data.Entities;
using EquipmentAccountingIS.App.Windows.ContractsWindow;
using EquipmentAccountingIS.App.Windows.UnitInfoWindow;

namespace EquipmentAccountingIS.App.Windows.StoredUnits;

public partial class AddStoredUnitWindow : Window
{
    private readonly ResponsibleEmployee authorizeEmployee;
    private readonly StoredUnit storedUnit = new();

    public AddStoredUnitWindow(ResponsibleEmployee responsibleEmployee)
    {
        authorizeEmployee = responsibleEmployee;
        InitializeComponent();
        MakeComboBoxes();
    }

    private void MakeComboBoxes()
    {
        using (var context = new EfModel()) // Создаём контекст данных БД
        {
            var unitInfo = new UnitInfo();
            unitInfo.EquipmentName = "Новый";
            var dataUnitInfo = context.UnitInfos.ToList();
            dataUnitInfo.Insert(0, unitInfo);
            cbUnitInfo.ItemsSource = dataUnitInfo;
            cbUnitInfo.DisplayMemberPath = "EquipmentName";
            cbUnitInfo.SelectedIndex = 0;

            var supplyContract = new SupplyContract();
            supplyContract.SupplyContractCodename = "Новый";
            var dataSuplContract = context.SupplyContracts.ToList();
            dataSuplContract.Insert(0, supplyContract);
            cbSupplyContract.ItemsSource = dataSuplContract;
            cbSupplyContract.DisplayMemberPath = "SupplyContractCodename";
            cbSupplyContract.SelectedIndex = 0;
        }
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            // Переменные для проверки 
            var parse = 0;
            var dateTime = DateTime.Now;

            var errors = new StringBuilder();
            if (cbUnitInfo.SelectedIndex == -1)
                errors.AppendLine("Выберите пункт Информация о единице!");
            if (!int.TryParse(tbQuantity.Text, out parse))
                errors.AppendLine("Данные в поле Количество введены в неверном формате!");
            if (dpDateDelivery.SelectedDate == null)
                errors.AppendLine("Выберите дату доставки!");
            if (tbPurpose.Text == "")
                errors.AppendLine("Введите назначение");
            if (cbSupplyContract.SelectedIndex == -1)
                errors.AppendLine("Выберите Контракт на поставку!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if (cbUnitInfo.SelectedIndex == 0)
            {
                var addUnitInfoWindow = new AddUnitInfoWindow();
                addUnitInfoWindow.ShowDialog();
                MakeComboBoxes();
            }

            if (cbSupplyContract.SelectedIndex == 0)
            {
                var addContractWindow = new AddContractWindow();
                addContractWindow.ShowDialog();
                MakeComboBoxes();
            }

            if (cbUnitInfo.SelectedIndex == 0 || cbSupplyContract.SelectedIndex == 0)
                return;

            using (var context = new EfModel()) // Создаём контекст данных БД
            {
                storedUnit.Quantity = Convert.ToInt32(tbQuantity.Text);
                storedUnit.DeliveryDate = (DateTime)dpDateDelivery.SelectedDate;
                storedUnit.Purpose = tbPurpose.Text;
                storedUnit.WhoAdded = authorizeEmployee.EmployeeId;
                var unitInfo = cbUnitInfo.SelectedItem as UnitInfo;
                storedUnit.UnitInfo = unitInfo.UnitInfoId;
                var supplyContract = cbSupplyContract.SelectedItem as SupplyContract;
                storedUnit.SupplyContract = supplyContract.SupplyContractId;
                context.StoredUnits.Add(storedUnit);
                context.SaveChanges();
            }

            MessageBox.Show("Запись успешно добавлена!");
            Close();
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
}