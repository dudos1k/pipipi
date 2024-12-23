using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace pipipi
{
    public partial class MainWindow : Window
    {
        string FilePath;
        int[] temperatures;
        public MainWindow()
        {
            InitializeComponent();
            FishKindSel.Items.Add("Сёмга");
            FishKindSel.Items.Add("Минтай");
            FishKindSel.Items.Add("Окунь");
            FishKindSel.Items.Add("Осётр");
            FishKindSel.Items.Add("Щука");
        }

        private void LoadDataBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Выберите файл",
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
            }
            ReadData();
        }
        public void ReadData()
        {
            DateTime startDateTime;
            if (!File.Exists(FilePath))
            {
                MessageBox.Show("Ошибка: файл не найден.");
                return;
            }
            string[] fileLines = File.ReadAllLines(FilePath);
            if (fileLines.Length != 2)
            {
                MessageBox.Show("Ошибка: файл должен содержать ровно две строки.");
                return;
            }
            string[] temperatureStrings = fileLines[1].Split(' ');
            try
            {
                temperatures = temperatureStrings.Select(temp => int.Parse(temp.Trim())).ToArray();
            }
            catch (FormatException)
            {
                MessageBox.Show("Ошибка: вторая строка должна содержать целые числа");
                return;
            }
            startDateTime = DateTime.Parse(fileLines[0]);
            ShipmentDateTxt.Text = startDateTime.ToString();
            TempChangeTxt.Text = string.Join(" ", temperatures);

        }

        private void FishKindSel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FishKindSel.SelectedIndex == 0)
            {
                MaxTempTxt.Text = "5";
                MinTempTxt.Text = "-3";
                OvercoldTimeTxt.Text = "60";
                OverheatTimeTxt.Text = "20";
            }
            if (FishKindSel.SelectedIndex == 1)
            {
                MaxTempTxt.Text = "-5";
                MinTempTxt.Text = "-180";
                OvercoldTimeTxt.Text = "5";
                OverheatTimeTxt.Text = "120";
            }
            if (FishKindSel.SelectedIndex == 2)
            {
                MaxTempTxt.Text = "-5";
                MinTempTxt.Text = "-10";
                OvercoldTimeTxt.Text = "5";
                OverheatTimeTxt.Text = "120";
            }
            if (FishKindSel.SelectedIndex == 3)
            {
                MaxTempTxt.Text = "-6";
                MinTempTxt.Text = "-14";
                OvercoldTimeTxt.Text = "5";
                OverheatTimeTxt.Text = "120";
            }
            if (FishKindSel.SelectedIndex == 4)
            {
                MaxTempTxt.Text = "-8";
                MinTempTxt.Text = "-180";
                OvercoldTimeTxt.Text = "9";
                OverheatTimeTxt.Text = "120";
            }
        }

        private void CheckBtn_Click(object sender, RoutedEventArgs e)
        {
            string[] temperatureStrings = TempChangeTxt.Text.Split(' ');
            temperatures = temperatureStrings.Select(temp => int.Parse(temp.Trim())).ToArray();
            int maxTemp = int.Parse(MaxTempTxt.Text);
            int minTemp = int.Parse(MinTempTxt.Text);
            int maxTempTime = int.Parse(OverheatTimeTxt.Text);
            int minTempTime = int.Parse(OvercoldTimeTxt.Text);
            DateTime shipmentDt = DateTime.Parse(ShipmentDateTxt.Text);
            int OverheatTime = 0;
            int OvercoldTime = 0;
            DateTime CurTime = shipmentDt;
            if (maxTemp < minTemp)
            {
                int buffer = maxTemp;
                maxTemp = minTemp;
                minTemp = buffer;
                buffer = maxTempTime;
                maxTempTime = minTempTime;
                minTempTime = buffer;
            }
            string res = $"Дата отгрузки: {shipmentDt}\n";
            foreach (int t in temperatures)
            {
                CurTime.AddMinutes(10);
                if (t > maxTemp)
                {
                    OverheatTime += 10;
                    res += $"Текущее время: {CurTime}; Текущее отклонение: {t - maxTemp}\n";
                }
                if (t < minTemp)
                {
                    OvercoldTime += 10;
                    res += $"Текущее время: {CurTime}; Текущее отклонение: {t - minTemp}\n";
                }
                if (OvercoldTime > minTempTime)
                {
                    res += $"Текущее время: {CurTime}; Рыба была переохлаждена более {minTempTime} минут!\n";
                }
                if (OverheatTime > maxTempTime)
                {
                    res += $"Текущее время: {CurTime}; Рыба была перегрета более {maxTempTime} минут!\n";
                }
            }
            ReportResTxt.Text = res;
        }

        private void SaveReport_Click(object sender, RoutedEventArgs e)
        {
            SaveResultToFile(ReportResTxt.Text);
        }

        public void SaveResultToFile(string res)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Сохранить результат",
                FileName = "Fish_Result.txt"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, res);
                MessageBox.Show("Результат сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OverheatTimeTxt_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void MinTempTxt_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ShipmentDateTxt_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}