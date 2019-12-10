using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using AutoFeedbackFschool.Core;

namespace AutoFeedbackFschool.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<TeacherProfileModel> Teachers { get; set; } = new ObservableCollection<TeacherProfileModel>();
        AutoFeedback _CurrentFeedbackEngine = null;

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            Teachers.Add(new TeacherProfileModel() { ClassName = "asdad" });
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_CurrentFeedbackEngine != null)
                    try { _CurrentFeedbackEngine.Close(); } catch { }

                btnFeedback.IsEnabled = false;
                pbStatus.IsIndeterminate = true;

                await Task.Run(() =>
                {
                    _CurrentFeedbackEngine = new AutoFeedback();
                    _CurrentFeedbackEngine.NavigateToLogin();
                });

                Teachers.Clear();

                foreach (var item in _CurrentFeedbackEngine.GetTeachers())
                {
                    Teachers.Add(item);
                    await Task.Delay(10);
                }

                pbStatus.IsIndeterminate = false;
                btnFeedback.IsEnabled = true;
            }
            catch (Exception err)
            {
                MessageBox.Show("Error: " + err.Message);
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Teachers.Count != 0)
                {
                    btnGetTeacher.IsEnabled = false;
                    pbStatus.IsIndeterminate = true;


                    FeedbackForms forms = new FeedbackForms
                    {
                        Comment = txbComment.Text,
                        SuDungGio = (Rating)cbxOnTime.SelectedIndex,
                        KhaNangTruyenDat = (Rating)cbxKntd.SelectedIndex,
                        KhaNangTaoKK = (Rating)cbxKntkk.SelectedIndex,
                        DamBaoKhoiLuong = (Rating)cbxDbkl.SelectedIndex,
                        HoTro = (Rating)cbxHtgv.SelectedIndex,
                        Ktra = (Rating)cbxThkt.SelectedIndex
                    };

                    await Task.Run(() => _CurrentFeedbackEngine.FeedbackTest(Teachers, forms));

                    MessageBox.Show("Finished! The application will close");
                    try { 
                        _CurrentFeedbackEngine.Close();
                    }
                    catch { }

                    Application.Current.Shutdown(0);
                }
                else
                {
                    MessageBox.Show("Can't feedback 0 teacher");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Error: " + err.Message);
            }
        }

        private void ToggleExclude(object sender, RoutedEventArgs e)
        {
            if (lsvTeacher.SelectedValue is TeacherProfileModel teacherProfile)
                teacherProfile.IsExclude = !teacherProfile.IsExclude;
        }
    }
}
