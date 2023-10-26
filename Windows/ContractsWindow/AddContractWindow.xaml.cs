using System;
using System.Linq;
using System.Windows;
using EquipmentAccountingIS.App.Data;
using EquipmentAccountingIS.App.Data.Entities;

namespace EquipmentAccountingIS.App.Windows.ContractsWindow;

public partial class AddContractWindow : Window
{
    private readonly SupplyContract newSupplyContract = new();

    public AddContractWindow()
    {
        InitializeComponent();
        MakeComboBox();
    }

    private void MakeComboBox()
    {
        using (var context = new EfModel()) // Создаём контекст данных
        {
            var data = context.Manufacturers.ToList();
            cbManufactures.ItemsSource = data;
            cbManufactures.DisplayMemberPath = "ManufacturerShortName";
            cbManufactures.SelectedIndex = 0;
        }
    }

    private void BtBack_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            if (tbCodeName.Text == "")
            {
                MessageBox.Show("Вы не заполнили поле \"Код\"!");
                return;
            }

            newSupplyContract.SupplyContractCodename = tbCodeName.Text;
            newSupplyContract.ManufacturerShortName = cbManufactures.SelectedIndex + 1;
            using (var context = new EfModel())
            {
                context.SupplyContracts.Add(newSupplyContract);
                context.SaveChanges();
                MessageBox.Show("Запись успешно добавлена!");
                Close();
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
        }
    }
}