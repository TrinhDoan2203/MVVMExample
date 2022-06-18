using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
namespace MVVMExample
{
    [Serializable]
    public class Model : INotifyPropertyChanged
    {
        private string firstValue = "";
        public string FirstValue 
        { 
            get { return firstValue; }
            set
            {
                firstValue = value;
                OnPropertyChange("FirstValue");
            } 
        }
        private string secondValue = "";
        public string SecondValue
        {
            get { return secondValue; }
            set
            {
                secondValue = value;
                OnPropertyChange("SecondValue");
            }
        }
        private string result="";
        public string Result
        {
            get { return result; }
            set
            {
                result = value;
                OnPropertyChange("Result");
            }
        }
        private string arithmetics="";
        public string Arithmetics
        {
            get { return arithmetics; }
            set
            {
                arithmetics = value;
                OnPropertyChange("Arithmetics");
            }
        }
        public string FVHash { get; set; }
        public string SVHash { get; set; }
        public string ArithHash { get; set; }
        public string ResultHash { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
