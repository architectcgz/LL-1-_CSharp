using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Exp2_WPF
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string filePath;
        private LL1Analysis LL1Analysiser;
        private ObservableCollection<StepInfo> _stepInfos; 
        private string _timeDay;
        private string _timeHM;
        private DispatcherTimer _timer;

        public ObservableCollection<StepInfo> StepInfos
        {
            get { return _stepInfos; }
            set {
                _stepInfos = value;
                OnPropertyChanged();
            }
        }

        public string TimeDay
        {
            get { return _timeDay; }
            set
            {
                _timeDay = value;
                OnPropertyChanged();
            }
        }

        public string TimeHM
        {
            get { return _timeHM; }
            set
            {
                _timeHM = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            LL1Analysiser = new LL1Analysis();
            _stepInfos = new ObservableCollection<StepInfo>();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }
        private void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            var input = inputTextBox.Text;
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
            {
                statusTextBlock.Foreground = Brushes.Red;
                statusTextBlock.Text = "请先输入要分析的内容";
                return;
            }
            StepInfos.Clear();
            statusTextBlock.Foreground = Brushes.Black;
            var result = LL1Analysiser.ParseInput(inputTextBox.Text);
            statusTextBlock.Foreground = result.StartsWith("ERROR") ? Brushes.Red : Brushes.Black;
            statusTextBlock.Text = result;
            foreach (var stepInfo in LL1Analysiser.StepInfos)
            {
                StepInfos.Add(stepInfo);
            }
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // 设置初始目录为相对于当前项目目录的 "grammars" 文件夹
            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string grammarDirectory = System.IO.Path.Combine(projectDirectory, @"..\..\..\grammars");
            openFileDialog.InitialDirectory = System.IO.Path.GetFullPath(grammarDirectory);

            if (openFileDialog.ShowDialog() == true)
            {
                filePath = openFileDialog.FileName;
                // 在这里处理文件选择逻辑
                statusTextBlock.Text = $"选择的文法文件: {filePath}";
                LL1Analysiser.ReadGrammarFromFile(filePath);
                LL1Analysiser.PreprocessGrammars();
                LL1Analysiser.GetFirstSet();
                LL1Analysiser.GetFollowSet();
                LL1Analysiser.CreateAnalysisTable();
                LL1Analysiser.PrintAnalysisTable();
            }
        }

        private void LookAnalysisTableButton_Click(object sender,RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                statusTextBlock.Text = "请先选择语法文件!";
                statusTextBlock.Foreground = Brushes.Red;
                return;
            }
            try
            {
                AnalysisTablePage analysisTablePage = new AnalysisTablePage(LL1Analysiser.AnalysisTable);
                statusTextBlock.Text = "查看分析表";
                statusTextBlock.Foreground = Brushes.Black;
                analysisTablePage.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing AnalysisTablePage: {ex.Message}");
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeDay = DateTime.Now.ToString("yyyy-MM-dd");
            TimeHM = DateTime.Now.ToString("HH:mm:ss");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

}