using IUR_Semestral_Work.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IUR_Semestral_Work.ViewModels
{
    public class TrashTypeInfoViewModel : ViewModelBase
    {
        private string _header;
        private string _description;
        public string Header
        {
            get => _header;
            set => SetProperty(ref _header, value);
        }
        
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
    }
}
