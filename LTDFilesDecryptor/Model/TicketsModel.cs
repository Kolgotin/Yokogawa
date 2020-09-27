using LTDFilesDecryptor.Common;
using System.ComponentModel;
using LTDFilesDecryptionLibrary;

namespace LTDFilesDecryptor.Model
{
    public class TicketsModel : TicketInfo, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged implementation

        private bool _NeedToFind;
        public bool NeedToFind
        {
            get => _NeedToFind;
            set
            {
                _NeedToFind = value;
                RaisePropertyChanged(nameof(NeedToFind));
            }
        }
        public TicketsModel()
        {

        }

        public TicketsModel(string name)
        {
            TicketName = name;
        }
    }
}
