using LTDFilesDecryptor.Common;
using LTDFilesDecryptor.Model;
using LTDFilesDecryptionLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;

namespace LTDFilesDecryptor.ViewModel
{
    public class WorkSpaceViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged implementation

        #region Fields
        CancellationTokenSource CancelToken = new CancellationTokenSource();
        Task FindFilesAndTicketsTask, DecriptFilesTask, FindAverageTask;
        ExceptionLogger ExLogger = new ExceptionLogger(TxtLogger.Log);
        #endregion Fields

        #region Properties
        private string _DirectoryPath = "";
        public string DirectoryPath
        {
            get => _DirectoryPath;
            set
            {
                _DirectoryPath = value;
                RaisePropertyChanged(nameof(DirectoryPath));
            }
        }

        private bool _ButtonsAreActive = true;
        public bool ButtonsAreActive
        {
            get => _ButtonsAreActive;
            set
            {
                _ButtonsAreActive = value;
                RaisePropertyChanged(nameof(ButtonsAreActive));
                RaisePropertyChanged(nameof(CancelIsActive));
            }
        } 
        public bool CancelIsActive => !_ButtonsAreActive;

        private List<SelectedFilesModel> _FileNamesList = new List<SelectedFilesModel>();
        public List<SelectedFilesModel> FileNamesList
        {
            get => _FileNamesList;
            set
            {
                _FileNamesList = value;
                RaisePropertyChanged(nameof(FileNamesList));
            }
        }

        private string _TicketsSearch = "";
        public string TicketsSearch
        {
            get => _TicketsSearch;
            set
            {
                _TicketsSearch = value;
                RaisePropertyChanged(nameof(TicketsSearch));
                RaisePropertyChanged(nameof(TicketsList));
            }
        }

        private List<TicketsModel> _TicketsList = new List<TicketsModel>();
        public List<TicketsModel> TicketsList
        {
            get => _TicketsList.Where(x=> x.TicketName.Contains(TicketsSearch)).ToList();
            set
            {
                _TicketsList = value;
                RaisePropertyChanged(nameof(TicketsList));
            }
        }

        private int _ProgressBarValue = 0;
        public int ProgressBarValue
        {
            get => _ProgressBarValue;
            set
            {
                _ProgressBarValue = value;
                RaisePropertyChanged(nameof(ProgressBarValue));
            }
        }

        private int _ProgressBarMaxValue = 1;
        public int ProgressBarMaxValue
        {
            get => _ProgressBarMaxValue;
            private set
            {
                _ProgressBarMaxValue = value;
                RaisePropertyChanged(nameof(ProgressBarMaxValue));
            }
        }

        private Visibility _ProgressBarVisibility = Visibility.Hidden;
        public Visibility ProgressBarVisibility
        {
            get => _ProgressBarVisibility;
            private set
            {
                _ProgressBarVisibility = value;
                RaisePropertyChanged(nameof(ProgressBarVisibility));
            }
        }

        public int SelectedIndexRangePeriod { get; set; } = 0;
        private IEnumerable<RangePeriod> _RangePeriodList => 
            Enum.GetValues(typeof(RangePeriod)).Cast<RangePeriod>();
        public List<string> RangePeriodList =>
            _RangePeriodList.Select(x=> x.RangePeriodToString()).ToList();
        #endregion Properties

        #region Commands
        public RelayCommand FindFilesAndTicketsCommand => new RelayCommand(FindFilesAndTickets); 
        public RelayCommand DecriptFilesCommand => new RelayCommand(DecriptFiles); 
        public RelayCommand FindAverageCommand => new RelayCommand(FindAverage);
        public RelayCommand CancelCommand => new RelayCommand(Cancel);
        public RelayCommand LoadTicketsCommand => new RelayCommand(LoadTickets); 
        public RelayCommand SelectAllCommand => new RelayCommand(SelectAll);
        public RelayCommand ClearCommand => new RelayCommand(Clear);
        #endregion Commands

        #region Methods
        private void LoadTickets()
        {
            string xmlPath;
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                xmlPath = openFileDialog.FileName;
                TicketsToFile load = new TicketsToFile();
                TicketsList = load.LoadFromFile(xmlPath);
                DirectoryPath = Directory.GetParent(Directory.GetParent(xmlPath).FullName).FullName;
            }
        }

        private async void FindFilesAndTickets()
        {
            try
            {
                ButtonsAreActive = false;
                FindFilesAndTicketsTask = new Task(() =>
                {
                    FindFiles();
                    PrepareProgressBar(FileNamesList.Count);
                    FindTickets();
                }, CancelToken.Token);
                FindFilesAndTicketsTask.Start();
                await FindFilesAndTicketsTask;
                MessageBox.Show("Датчики найдены");
            }
            catch (Exception ex)
            {
                ExLogger.LogException(ex);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                ProgressBarVisibility = Visibility.Hidden;
                ButtonsAreActive = true;
            }
        }

        private void FindFiles()
        {
            var files = Directory.GetFiles(DirectoryPath, "*.LTD", SearchOption.AllDirectories);
            int index = DirectoryPath.Length;
            FileNamesList = files.Select(x => new SelectedFilesModel(x, index)).ToList();
        }

        private void FindTickets()
        {
            var findTickets = new FindTickets(DirectoryPath);
            findTickets.SetProgressAction(() => ProgressBarValue++);
            var allTickets = findTickets.Run(FileNamesList, CancelToken);
            TicketsList = new List<TicketsModel>(allTickets.Distinct().Select(x => new TicketsModel(x)));
        }

        delegate void Counter();
        private async void DecriptFiles()
        {
            try
            {
                ButtonsAreActive = false;
                DecriptFilesTask = new Task(() =>
                {
                    PrepareProgressBar(FileNamesList.Count);
                    var neededTickets = TicketsList.Where(x => x.NeedToFind).Select(x => x.TicketName).ToList();
                    var writeValuesToCSV = new WriteValuesToCSV(DirectoryPath, FileNamesList, neededTickets);
                    writeValuesToCSV.SetProgressAction(() => ProgressBarValue++);
                    writeValuesToCSV.Run(CancelToken);
                }, CancelToken.Token);
                DecriptFilesTask.Start();
                await DecriptFilesTask;
                TicketsToFile save = new TicketsToFile();
                save.SaveToFile(DirectoryPath, TicketsList);
                MessageBox.Show("Файлы расшифрованы");
            }
            catch (Exception ex)
            {
                ExLogger.LogException(ex);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                ProgressBarVisibility = Visibility.Hidden;
                ButtonsAreActive = true;
            }
        }

        private async void FindAverage()
        {
            try
            {
                ButtonsAreActive = false;
                FindAverageTask = new Task(() =>
                {
                    RangePeriod range = _RangePeriodList.ToList()[SelectedIndexRangePeriod];
                    var neededTickets = TicketsList.Where(x => x.NeedToFind).Cast<TicketInfo>().ToList();
                    var calculateAverage = new CalculateAverageValues(DirectoryPath, neededTickets, range);
                    int filesCount = calculateAverage.FindFiles();
                    PrepareProgressBar(filesCount);
                    calculateAverage.SetProgressAction(() => ProgressBarValue++);
                    calculateAverage.Run(CancelToken);
                }, CancelToken.Token);
                FindAverageTask.Start();
                await FindAverageTask;
                MessageBox.Show("Средние значения расчитаны и сохранены");
            }
            catch (Exception ex)
            {
                ExLogger.LogException(ex);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                ProgressBarVisibility = Visibility.Hidden;
                ButtonsAreActive = true;
            }
        }

        private void SelectAll()
        {
            try
            {
                if (TicketsList.All(x => x.NeedToFind))
                    TicketsList.ForEach(x => x.NeedToFind = false);
                else
                    TicketsList.ForEach(x => x.NeedToFind = true);
                RaisePropertyChanged(nameof(TicketsList));
            }
            catch (Exception ex)
            {
                ExLogger.LogException(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void Cancel()
        {
            if(!(CancelToken is null))
            {
                CancelToken.Cancel();
                CancelToken.Dispose();
                CancelToken = new CancellationTokenSource();
            }
        }

        private void Clear()
        {
            TicketsList = new List<TicketsModel>();
            DirectoryPath = "";
            TicketsSearch = "";
        }

        private void PrepareProgressBar(int maxValue)
        {
            ProgressBarValue = 0;
            ProgressBarMaxValue = maxValue;
            ProgressBarVisibility = Visibility.Visible;
        }
        /*public string RangePeriodToString(RangePeriod period)
        {
            switch (period)
            {
                case RangePeriod.Hour:
                    return "Час";
                case RangePeriod.Day:
                    return "День";
                case RangePeriod.Month:
                    return "Месяц";
                case RangePeriod.AllTime:
                    return "Всё время";
                default:
                    return "";
            }
        }*/
        #endregion Methods
    }
}