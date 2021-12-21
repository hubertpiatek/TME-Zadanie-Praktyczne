using System;
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
    }
    
}
