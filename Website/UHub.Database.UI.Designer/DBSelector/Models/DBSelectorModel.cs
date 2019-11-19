using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UHub.Database.UI.Designer.DBSelector.Models
{
    public class DBSelectorModel : INotifyPropertyChanged
    {
        // boiler-plate
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }


        //ServerSearch
        private string _currentSearch;
        public string CurrentSearch
        {
            get => _currentSearch;
            set => SetField(ref _currentSearch, value);
        }


        //ServerDbList
        private IEnumerable<string> _serverDbList;
        public IEnumerable<string> ServerDbList
        {
            get => _serverDbList;
            set => SetField(ref _serverDbList, value);
        }



        //SelectedDb
        private string _selectedDb;
        public string SelectedDb
        {
            get => _selectedDb;
            set => SetField(ref _selectedDb, value);
        }
    }
}
