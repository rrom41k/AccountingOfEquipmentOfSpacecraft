using System.Windows;
using EquipmentAccountingIS.App.Data.Entities;
using EquipmentAccountingIS.App.Windows.EmployeesWindow;
using EquipmentAccountingIS.App.Windows.StoredUnits;
using EquipmentAccountingIS.App.Windows.UnitInfoWindow;

namespace EquipmentAccountingIS.App.Windows;

public partial class NavigationMenuWindow : Window
{
    private readonly ResponsibleEmployee authorizedEmployee;

    public NavigationMenuWindow(ResponsibleEmployee authorizedEmployee)
    {
        this.authorizedEmployee = authorizedEmployee;
        InitializeComponent();
        if (this.authorizedEmployee.AccessType == 2)
            btEmployeesCrudWindow.IsEnabled = false;
    }

    private void BtBack_OnClick(object sender, RoutedEventArgs e)
    {
        var startWindow = new StartWindow(authorizedEmployee);
        startWindow.ShowDialog();
        Close();
    }

    private void BtStoredUnitsCrudWindow_OnClick(object sender, RoutedEventArgs e)
    {
        var storedUnitsWindow = new StoredUnitsCrudWindow(authorizedEmployee);
        storedUnitsWindow.Show();
        Close();
    }

    private void BtUnitInfoCrudWindow_OnClick(object sender, RoutedEventArgs e)
    {
        var unitInfoCrudWindow = new UnitInfoCrudWindow(authorizedEmployee);
        unitInfoCrudWindow.Show();
        Close();
    }

    private void BtEmployeesCrudWindow_OnClick(object sender, RoutedEventArgs e)
    {
        var employeesCrudWindow = new EmployeesCrudWindow(authorizedEmployee);
        employeesCrudWindow.ShowDialog();
        Close();
    }
}