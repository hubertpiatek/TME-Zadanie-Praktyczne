using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;

namespace TME_Zadanie_Praktyczne
{
    public partial class MainWindow : MetroWindow
    {
        ApplicationDbContext applicationDbContext = new ApplicationDbContext();
        BackgroundWorker backgroundWorker = new BackgroundWorker();
        int totalRecords;
        int totalUsedNumbers;
        int totalFreeNumbers;
        int userNumber;
        Boolean progress;
        public MainWindow()
        {
            InitializeComponent();
            
            InitDb();
            totalFreeNumbers = applicationDbContext.Numbers.Where(n => n.Status == "false").Count();
            Console.WriteLine(totalFreeNumbers);
            totalRecords = applicationDbContext.Numbers.Count<Numbers>();
            this.applicationDbContext.Database.CommandTimeout = 180;
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += saveNumbersToDb;
            backgroundWorker.ProgressChanged += changeFrontend;
            backgroundWorker.RunWorkerCompleted += clearUI;
            calculateUsage();
        }

        public void InitDb()
        {
            
            int tableSize = 9000000;
            int tableNumberValue = 1000000;
            if (!applicationDbContext.Numbers.Any())
            {
                Console.WriteLine("Creating Table...");
                List<Numbers> numbers = new List<Numbers>();
                for (int i = 0; i < tableSize; i++)
                {
                    numbers.Add(new Numbers(tableNumberValue));
                    if (tableNumberValue % 100000 == 0)
                    {
                        applicationDbContext.BulkInsert(numbers);
                        numbers.Clear();
                    }
                    tableNumberValue++;
                }
                applicationDbContext.BulkInsert(numbers);
                numbers.Clear();
            }
        }
        private void numberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void calculateUsage()
        {
            totalUsedNumbers = applicationDbContext.Numbers.Where(n => n.Status == "true").Count();
            percentUsage.Text = (Convert.ToDouble(totalUsedNumbers) / Convert.ToDouble(totalRecords)).ToString("0.0000%");
            totalUsage.Text = totalUsedNumbers + "/" + totalRecords;
        }

        public void clearUI(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Cancelled) {
                percentUsage.Text = (Convert.ToDouble(totalUsedNumbers) / Convert.ToDouble(totalRecords)).ToString("0.0000%");
                totalUsage.Text = totalUsedNumbers + "/" + totalRecords;
                errorTextBlock.Foreground = Brushes.Black;
                errorTextBlock.Text = "Pomyślnie dodano numery do bazy";
                progressBarStatus.Visibility = Visibility.Collapsed;
                progressBarStatus.Value = 0;
                progress = false;
            }
        }
        public void changeFrontend(object sender, ProgressChangedEventArgs e)
        {
            if (progress) progressBarStatus.Value = e.ProgressPercentage;
            else if (amountOfNumberValue.Text.Length == 0)
            {
                errorTextBlock.Visibility = Visibility.Visible;
                errorTextBlock.Foreground = Brushes.Red;
                errorTextBlock.Text = "Puste pole, wpisz ilość liczb do wylosowania!";
                backgroundWorker.CancelAsync();
            }
            else if (amountOfNumberValue.Text.Length > 9)
            {
                errorTextBlock.Visibility = Visibility.Visible;
                errorTextBlock.Foreground = Brushes.Red;
                errorTextBlock.Text = "Podana wartość jest zbyt duża";
                backgroundWorker.CancelAsync();
            }
            else
            {
                try
                {
                    userNumber = Convert.ToInt32(amountOfNumberValue.Text);
                    if (userNumber == 0 || amountOfNumberValue.Text.StartsWith("0"))
                    {
                        errorTextBlock.Visibility = Visibility.Visible;
                        errorTextBlock.Foreground = Brushes.Red;
                        errorTextBlock.Text = "Wartość liczbowa musi być większa od 0";
                        backgroundWorker.CancelAsync();
                    }
                    else if (userNumber > totalFreeNumbers)
                    {
                        errorTextBlock.Visibility = Visibility.Visible;
                        errorTextBlock.Foreground = Brushes.Red;
                        errorTextBlock.Text = "Podana wartość jest zbyt duża";
                        backgroundWorker.CancelAsync();
                    }
                    else
                    {
                        progressBarStatus.Visibility = Visibility.Visible;
                        errorTextBlock.Visibility = Visibility.Visible;
                        errorTextBlock.Foreground = Brushes.Black;
                        errorTextBlock.Text = "Losuję numery, proszę czekać...";
                    }
                }
                catch (FormatException err)
                {
                    errorTextBlock.Visibility = Visibility.Visible;
                    errorTextBlock.Foreground = Brushes.Red;
                    errorTextBlock.Text = "Podana wartość nie jest liczbą";
                    backgroundWorker.CancelAsync();
                    Console.WriteLine(err);
                }
            }
        }

        public void updateStatusInDB(int numberOfRecords) {
            applicationDbContext.Numbers
                .Where(s => s.Status == "false")
                .OrderBy(x => Guid.NewGuid())
                .Take(numberOfRecords)
                .UpdateFromQuery(x => new Numbers { Status = "true" });
        }
        public void saveNumbersToDb(object sender, DoWorkEventArgs e)
        {
            backgroundWorker.ReportProgress(1);
            Thread.Sleep(100);
            if (!backgroundWorker.CancellationPending)
            {
                if (userNumber > 0)
                {
                    progress = true;
                    backgroundWorker.ReportProgress(5);
                    totalFreeNumbers = applicationDbContext.Numbers.Where(n => n.Status == "false").Count();
                    backgroundWorker.ReportProgress(10);
                    int queryStep = 100000;
                    
                    if (userNumber > queryStep)
                    {
                        progress = true;
                        int numberOfsteps = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(userNumber) / queryStep));
                        for (int i = 0; i < numberOfsteps; i++)
                        {
                           backgroundWorker.ReportProgress(Convert.ToInt32(((double)(i + 1) / (double)numberOfsteps) * 100));
                           if (i == numberOfsteps - 1) { updateStatusInDB(userNumber - (numberOfsteps - 1) * queryStep); }
                           else { updateStatusInDB(queryStep); }
                        }
                    }
                    else {
                        progress = true;
                        backgroundWorker.ReportProgress(20);
                        updateStatusInDB(userNumber); }
                }
                totalUsedNumbers = applicationDbContext.Numbers.Where(n => n.Status == "true").Count();
                totalFreeNumbers = totalRecords - totalUsedNumbers;
                backgroundWorker.CancelAsync();
                e.Cancel = true;
            }   
        }
        private void drawNumbers(object sender, EventArgs e)
        {
            if (!backgroundWorker.IsBusy) backgroundWorker.RunWorkerAsync();
            else backgroundWorker.CancelAsync();
        }
    }
}
