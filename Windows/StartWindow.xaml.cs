using System.Windows;
using System.Windows.Media;
using EquipmentAccountingIS.App.Data.Entities;

namespace EquipmentAccountingIS.App.Windows;

/// <summary>
///     Логика взаимодействия для StartWindow.xaml
/// </summary>
public partial class StartWindow : Window
{
    private readonly ResponsibleEmployee authorizedEmployee;

    public StartWindow(ResponsibleEmployee respEmployee)
    {
        authorizedEmployee = respEmployee;
        InitializeComponent();
        tbFIO.Text = respEmployee.EmployeeInitials;
        tbPost.Text = respEmployee.PostNavigation.PostName;
        // Выставление разделительной линии
        if (tbFIO.Text.Length > tbPost.Text.Length)
            SetBlackUnderline();
        else
            SetBlackOverline();
    }

    // Use a Black pen for the underline text decoration.
    private void SetBlackUnderline()
    {
        // Create an underline text decoration. Default is underline.
        var myUnderline = new TextDecoration();

        // Create a solid color brush pen for the text decoration.
        myUnderline.Pen = new Pen(Brushes.Black, 1);
        myUnderline.PenThicknessUnit = TextDecorationUnit.FontRecommended;

        // Set the underline decoration to a TextDecorationCollection and add it to the text block.
        var myCollection = new TextDecorationCollection();
        myCollection.Add(myUnderline);
        tbFIO.TextDecorations = myCollection;
    }

    // Use a Black pen for the overline text decoration.
    private void SetBlackOverline()
    {
        // Create an underline text decoration. Default is underline.
        var myOverline = new TextDecoration();

        // Create a solid color brush pen for the text decoration.
        myOverline.Pen = new Pen(Brushes.Black, 1);
        myOverline.PenThicknessUnit = TextDecorationUnit.FontRecommended;
        myOverline.Location = TextDecorationLocation.OverLine;

        // Set the underline decoration to a TextDecorationCollection and add it to the text block.
        var myCollection = new TextDecorationCollection();
        myCollection.Add(myOverline);
        tbPost.TextDecorations = myCollection;
    }

    private void BtNavWindow_OnClick(object sender, RoutedEventArgs e)
    {
        var navigationMenuWindow = new NavigationMenuWindow(authorizedEmployee);
        navigationMenuWindow.Show();
        Close();
    }

    private void BtAuthWindow_OnClick(object sender, RoutedEventArgs e)
    {
        var authWindow = new AuthWindow();
        authWindow.Show();
        Close();
    }
}