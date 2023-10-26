using System;
using System.Windows;
using System.Windows.Documents;
using EquipmentAccountingIS.App.Data;
using EquipmentAccountingIS.App.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EquipmentAccountingIS.App.Windows.StoredUnits;

public partial class DeleteStoredUnitWindow : Window
{
    private readonly StoredUnit storedUnit;

    public DeleteStoredUnitWindow(StoredUnit storedUnit)
    {
        this.storedUnit = storedUnit;
        InitializeComponent();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var textRange = new TextRange(rtbNote.Document.ContentStart, rtbNote.Document.ContentEnd);
            var content = textRange.Text;
            if (content == "")
            {
                MessageBox.Show("Вы не ввели примечание к удалению!");
                return;
            }

            storedUnit.SignOfDeleting = "Y";
            storedUnit.DateOfWriteOff = DateTime.Now;
            storedUnit.Note = content;

            using (var context = new EfModel()) // Создаём контекст данных
            {
                context.Entry(storedUnit).State = EntityState.Modified; // Изменение данных в контексте
                context.SaveChanges(); // Сохранение изменений в базу данных
                MessageBox.Show("Запись успешно удалена!");
                Close();
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
        }
    }
}