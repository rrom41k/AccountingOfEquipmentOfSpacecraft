using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using EquipmentAccountingIS.App.Data;
using EquipmentAccountingIS.App.Data.Entities;
using Microsoft.Win32;

namespace EquipmentAccountingIS.App.Windows.UnitInfoWindow;

public partial class AddUnitInfoWindow : Window
{
    private readonly UnitInfo unitInfo = new();

    public AddUnitInfoWindow()
    {
        InitializeComponent();
        var imagePath =
            Path.Combine(Environment.CurrentDirectory,
                "Resources\\ImageNotFound.png"); // Получаем путь к фалу ихображения
        // Создаем новый экземпляр BitmapImage с новым путем к файлу
        var newImageSource = new BitmapImage(new Uri(imagePath));
        // Устанавливаем новый источник изображения в свойство Image.Source
        imgUnitInfo.Source = newImageSource;
        MakeComboBox();
    }

    private void MakeComboBox()
    {
        using (var context = new EfModel()) // Создаём контекст данных
        {
            var data = context.EquipmentTypes.ToList();
            cbType.ItemsSource = data;
            cbType.DisplayMemberPath = "EquipmentTypeName";
            cbType.SelectedIndex = 0;
        }
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var errors = new StringBuilder();
            // Изменение состояния сущностей на "изменено" (Modified)
            if (tbName.Text == "")
                errors.AppendLine("Не заполнено поле \"Название\"!");
            if (tbCode.Text == "")
                errors.AppendLine("Не заполнено поле \"Код\"!");
            if (tbPassport.Text == "")
                errors.AppendLine("Не заполнено поле \"Паспорт\"!");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if (unitInfo.Photo == null)
            {
                var imageData =
                    ByteArrayToImageConverter.CompressImage(
                        Path.Combine(Environment.CurrentDirectory, "Resources\\ImageNotFound.png"),
                        24000); // Сжимаем и конвертируем изображение в массив байтов
                unitInfo.Photo = imageData; // Записываем данные в объект БД
            }

            unitInfo.EquipmentName = tbName.Text;
            unitInfo.EquipmentCodename = tbCode.Text;
            unitInfo.EquipmentPassport = tbPassport.Text;
            unitInfo.EquipmentType = cbType.SelectedIndex + 1;
            using (var context = new EfModel()) // Создаём контекст данных
            {
                context.UnitInfos.Add(unitInfo); // Изменение данных в контексте
                context.SaveChanges(); // Сохранение изменений в базу данных
                MessageBox.Show("Запись успешно добавлена!");
            }

            Close();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    private void BtBack_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }


    private void BtChangeImage_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var openFileDialog = new OpenFileDialog(); // Создаём объект окна для выбора файла изображения
            openFileDialog.Filter =
                "Image Files (*.png;*.jpg;*.jpeg;*.gif)|*.png;*.jpg;*.jpeg;*.gif"; // Установка фильтра файлов
            var result = openFileDialog.ShowDialog(); // Выводим окно для выбора файла изображения

            if (result == true)
            {
                var imagePath = openFileDialog.FileName; // Получаем путь к фалу ихображения
                // Создаем новый экземпляр BitmapImage с новым путем к файлу
                var newImageSource = new BitmapImage(new Uri(imagePath));
                // Устанавливаем новый источник изображения в свойство Image.Source
                imgUnitInfo.Source = newImageSource;

                var imageData =
                    ByteArrayToImageConverter.CompressImage(imagePath,
                        24000); // Сжимаем и конвертируем изображение в массив байтов
                unitInfo.Photo = imageData; // Записываем данные в объект БД
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
        }
    }
}