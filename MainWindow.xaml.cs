using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using MahApps.Metro.Controls;

namespace TME_Zadanie_Praktyczne
{
    public partial class MainWindow : MetroWindow
    {
        ApplicationDbContext applicationDbContext = new ApplicationDbContext();
        BackgroundWorker backgroundWorker = new BackgroundWorker();
        int totalRecords;

        public MainWindow()
        {
            InitializeComponent();
            InitDb();
            totalRecords = applicationDbContext.Numbers.Count<Numbers>();
            this.applicationDbContext.Database.CommandTimeout = 180;
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            calculateUsage();
        }



        public void InitDb()
        {
           //applicationDbContext.Configuration.AutoDetectChangesEnabled = false;
          // applicationDbContext.Configuration.ValidateOnSaveEnabled = false;
            int tableSize = 9000000;
            int tableNumberValue = 1000000;
            if (!applicationDbContext.Numbers.Any())
            {
                List<Numbers> numbers = new List<Numbers>();
                for (int i = 0; i < tableSize; i++)
                {
                    numbers.Add(new Numbers(tableNumberValue));
                    if (tableNumberValue % 150000 == 0)
                    {
              
                        applicationDbContext.BulkInsert(numbers);
                       // applicationDbContext.SaveChanges();
                        numbers.Clear();
                    }
                    tableNumberValue++;
                }
                //applicationDbContext.Numbers.InsertFromQuery(numbers, x => new Numbers{ x.Id = numbers.Id,x.Value,x.Status });
                applicationDbContext.BulkInsert(numbers);
                //applicationDbContext.SaveChanges();
                numbers.Clear();
            }
        }
        private void numberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private int inputValidation(int userNumber,int totalFreeNumbers)
        {
            //errorTextBlock.Visibility = Visibility.Collapsed;
            if (amountOfNumberValue.Text.Length == 0)
            {
                errorTextBlock.Visibility = Visibility.Visible;
                errorTextBlock.Text = "Puste pole, wpisz ilość liczb do wylosowania!";
                return 0;
            }
            try
            {
                userNumber = Convert.ToInt32(amountOfNumberValue.Text);
                if (userNumber == 0 || amountOfNumberValue.Text.StartsWith("0"))
                {
                    errorTextBlock.Visibility = Visibility.Visible;
                    errorTextBlock.Text = "Wartość liczbowa musi być większa od 0";
                    return 0;
                }
                else if (userNumber > totalFreeNumbers)
                {
                    errorTextBlock.Visibility = Visibility.Visible;
                    errorTextBlock.Text = "Podana wartość jest zbyt duża";
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


            return userNumber;
        }
        private void calculateUsage()
        {
            int totalUsedNumbers = applicationDbContext.Numbers.Where(n => n.Status == "true").Count();
            percentUsage.Text = (Convert.ToDouble(totalUsedNumbers) / Convert.ToDouble(totalRecords)).ToString("0.0000%");
            totalUsage.Text = totalUsedNumbers + "/" + totalRecords;
        }

        public void changeFrontend()
        {
            progressBarStatus.Visibility = Visibility.Visible;
            errorTextBlock.Visibility = Visibility.Visible;
            errorTextBlock.Foreground = Brushes.Black;
            errorTextBlock.Text = "Losuje numery, proszę czekać...";
        }

        public void updateStatusInDB(int numberOfRecords) {
           
            applicationDbContext.Numbers
                .Where(s => s.Status == "false")
                .OrderBy(x => Guid.NewGuid())
                .Take(numberOfRecords)
                .UpdateFromQuery(x => new Numbers { Status = "true" });
        }
        public void saveNumbersToDb()
        {
            int userNumber = 0;
            int queryStep = 150000;
            int totalFreeNumbers = applicationDbContext.Numbers.Where(n => n.Status == "false").Count();
            userNumber = inputValidation(userNumber, totalFreeNumbers);
            if (userNumber > 0)
            {
                int valueFromTextField = Convert.ToInt32(amountOfNumberValue.Text);
                if (valueFromTextField > queryStep)
                {
                    int numberOfsteps = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(valueFromTextField) / queryStep));
                    for (int i = 0; i < numberOfsteps; i++) {
                        if (i == numberOfsteps - 1){updateStatusInDB(valueFromTextField - (numberOfsteps - 1) * queryStep);}
                        else { updateStatusInDB(queryStep); }
                    }
                }
                else {updateStatusInDB(valueFromTextField);}
            }
        }
        private void drawNumbers(object sender, EventArgs e)
        {
            changeFrontend();
            saveNumbersToDb();
            calculateUsage();
        }
    }
}
