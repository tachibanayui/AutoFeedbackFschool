using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AutoFeedbackFschool.Core
{
    public class TeacherProfileModel : INotifyPropertyChanged
    {
        private bool _IsExclude;
        public bool IsExclude
        {
            get => _IsExclude;
            set
            {
                if(_IsExclude != value)
                {
                    _IsExclude = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ClassName { get; set; }
        public string AcademicYear { get; set; }
        public string Term { get; set; }
        public string Teacher { get; set; }
        public string FeedbackFor { get; set; }
        public string FeedbackLink { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}