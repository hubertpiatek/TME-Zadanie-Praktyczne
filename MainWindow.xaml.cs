using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using EntityFramework.BulkInsert.Extensions;
using MahApps.Metro.Controls;

namespace TME_Zadanie_Praktyczne
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : MetroWindow
    {
        int dbSize = 9000000;
        int numberValue = 1000000;
        ApplicationDbContext applicationDbContext = new ApplicationDbContext();

        public MainWindow()
        {
            InitializeComponent();
            InitDb();
            var entity = applicationDbContext.Numbers.FirstOrDefault(item => item.Value == 1000001);
            entity.Status = "false";
            applicationDbContext.Numbers.Attach(entity);
            applicationDbContext.Entry(entity).Property(p => p.Status).IsModified = true;
            applicationDbContext.SaveChanges();

        }

        public void InitDb()
        {
            applicationDbContext.Configuration.AutoDetectChangesEnabled = false;
            applicationDbContext.Configuration.ValidateOnSaveEnabled = false;
            if (!applicationDbContext.Numbers.Any())
            {
                List<Numbers> numbers = new List<Numbers>();
                for (int i = 0; i < dbSize; i++)
                {
                    numbers.Add(new Numbers(numberValue));
                    if (numberValue % 500000 == 0)
                    {
                        applicationDbContext.BulkInsert(numbers);
                        applicationDbContext.SaveChanges();
                        numbers.Clear();
                    }
                    //Console.WriteLine(numberValue);
                    numberValue++;

                }
                applicationDbContext.BulkInsert(numbers);
                applicationDbContext.SaveChanges();
                numbers.Clear();
            }
        }
        private void numberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
           Regex regex = new Regex("[^0-9]+");
           e.Handled = regex.IsMatch(e.Text);
        }
        private int inputValidation(int amountOfNumber)
        {
            errorTextBlock.Visibility = Visibility.Collapsed;
            if (amountOfNumberValue.Text.Length == 0)
            {
                errorTextBlock.Visibility = Visibility.Visible;
                errorTextBlock.Text = "Puste pole, wpisz ilość liczb do wylosowania!";
                return 0;
            }

            else
            {
                try
                {
                    amountOfNumber = Convert.ToInt32(amountOfNumberValue.Text);
                    Console.WriteLine(amountOfNumberValue.Text);
                    if (amountOfNumber == 0 || amountOfNumberValue.Text.StartsWith("0"))
                    {
                        errorTextBlock.Visibility = Visibility.Visible;
                        errorTextBlock.Text = "Wartość liczbowa musi być większa od 0";
                        return 0;
                    }
                }
                catch (FormatException err)
                {
                    errorTextBlock.Visibility = Visibility.Visible;
                    errorTextBlock.Text = "Podana wartość nie jest liczbą";
                    Console.WriteLine(err);
                    return 0;
                    
                }

            }
            return amountOfNumber;
        }
        private void DrawNumbers(object sender, RoutedEventArgs e)
        {
            int amountOfNumber = 0;
            amountOfNumber = inputValidation(amountOfNumber);
            //if (amountOfNumber > 0) {
              //  Random r = new Random();
             //   int randomValue = 0;
             //   List<int> randomNumvers = new List<int>();
               // for (int i = 0; i < amountOfNumber; i++) {
               //     randomValue = r.Next(numberValue, numberValue + dbSize - 1);
               // }
            //}
            Console.WriteLine(amountOfNumber);
            
        }
    }
    
}
