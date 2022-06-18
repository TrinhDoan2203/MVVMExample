using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using System.Linq;
using System.IO;
using System.Windows;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace MVVMExample
{
    public class MyViewModel : INotifyPropertyChanged
    {
        private string filePath= Directory.GetCurrentDirectory()+"\\SerialData.xml";
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; NotifyPropertyChanged("FilePath"); }
        }
        private string hashVerify = "";
        public string HashVerify
        {
            get { return hashVerify; }
            set { hashVerify = value; NotifyPropertyChanged("HashVerify"); }
        }
        private Model user;
        public Model User
        {
            get { return user; }
            set { user = value; NotifyPropertyChanged("User"); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public MyViewModel()
        {
            User = new Model();
            UpdateCommand = new RelayCommand(new Action<object>(UpdateAge));
            XmlSerializeCommand = new RelayCommand(new Action<object>(XmlSerialize));
            XmlDeserializeCommand = new RelayCommand(new Action<object>(XmlDeserialize));
            LongrunCommand = new RelayCommand(new Action<object>(LongrunCalculation));
        }
        public ICommand UpdateCommand { get; set; }
        public ICommand XmlSerializeCommand { get; set; }
        public ICommand XmlDeserializeCommand { get; set; }
        public ICommand LongrunCommand { get; set; }

        public void UpdateAge(object obj)
        {
            if (User == null) User = new Model();
            User.Result = Calculate(User.FirstValue, User.SecondValue, User.Arithmetics);
        }
        public string Calculate(string firstValue, string secondValue, string arithmetic)
        {
            double number3, number1, number2;
            string result;
            if (!string.IsNullOrEmpty(firstValue) && !string.IsNullOrEmpty(secondValue) && !string.IsNullOrEmpty(arithmetic))
            {
                if (Double.TryParse(firstValue, out number1))
                {
                    if (Double.TryParse(secondValue, out number2))
                    {
                        try
                        {
                            switch (arithmetic)
                            {
                                case "+":
                                    number3 = number1 + number2;
                                    result = string.Format("{0:N2}", number3);
                                    break;
                                case "-":
                                    number3 = number1 - number2;
                                    result = string.Format("{0:N2}", number3);
                                    break;
                                case "*":
                                    number3 = number1 * number2;
                                    result = string.Format("{0:N2}", number3);
                                    break;
                                case "/":
                                    number3 = number1 / number2;
                                    result = string.Format("{0:N2}", number3);
                                    break;
                                default:
                                    result = "Could not use the arithmetic: " + arithmetic;
                                    break;
                            }
                        }
                        catch (Exception exc)
                        {
                            result = "Exception";
                            MessageBox.Show(exc.ToString());
                        }
                    }
                    else
                    {
                        result = "Unable to parse " + secondValue;
                    }
                }
                else
                {
                   result="Unable to parse " + firstValue;
                }
            }
            else
            {
                result = "Input value(s) is null or empty";
            }
            return result;
        }
        public void XmlSerialize(object obj)
        {
            XmlSerializeMethod(typeof(Model), User, FilePath);
        }
        public void XmlDeserialize(object obj)
        {
            User = XmlDeserializeMethod(typeof(Model), FilePath);
        }
        public bool XmlSerializeMethod(Type datatype, Model model, string file)
        {
            bool successful = true;
            try
            {
                object data = model;
                model.FVHash = CalculateHash(model.FirstValue);
                model.SVHash = CalculateHash(model.SecondValue);
                model.ArithHash = CalculateHash(model.Arithmetics);
                model.ResultHash = CalculateHash(model.Result);
                string ext = Path.GetExtension(file);
                if (ext == ".xml")
                {
                    TextWriter writer = null;
                    try
                    {
                        if (File.Exists(file)) File.Delete(file);
                        XmlSerializer xmlSerial = new XmlSerializer(datatype);
                        writer = new StreamWriter(file);
                        xmlSerial.Serialize(writer, data);
                        writer.Close();
                    }
                    catch (Exception e)
                    {
                        successful = false;
                        MessageBox.Show(e.ToString());
                    }
                }
                else
                {
                    successful = false;
                    User.Result="Extension file is not .xml";
                }
            }
            catch (Exception e)
            {
                successful = false;
                MessageBox.Show(e.ToString());
            }
            
            return successful;
        }
        public string CalculateHash(string value)
        {
            byte[] hashValue;
            UnicodeEncoding ue = new UnicodeEncoding();
            byte[] messageBytes = ue.GetBytes(value);
            SHA256 shHash = SHA256.Create();
            hashValue = shHash.ComputeHash(messageBytes);
            return BitConverter.ToString(hashValue);
        }
        public Model XmlDeserializeMethod(Type datatype, string file)
        {
            Model model = new Model();
            try
            {
                object obj = model;
                string ext = Path.GetExtension(file);
                if (ext == ".xml")
                {
                    if (File.Exists(file))
                    {
                        TextReader reader;
                        try
                        {
                            XmlSerializer xmlSerial = new XmlSerializer(datatype);
                            reader = new StreamReader(file);
                            obj = xmlSerial.Deserialize(reader);
                            reader.Close();
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString());
                        }
                    }
                    else
                    {
                        User.Result="File Path does not exist!";
                    }
                }
                else
                {
                    User.Result="Extension file is not .xml";
                }
                model = obj as Model;
                HashVerify = "";
                VerifyHashcode("First value",model.FirstValue, model.FVHash);
                VerifyHashcode("Second value",model.SecondValue, model.SVHash);
                VerifyHashcode("Arithmetic", model.Arithmetics, model.ArithHash);
                VerifyHashcode("Result", model.Result, model.ResultHash);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return model;
        }
        public void VerifyHashcode(string name,string input, string hash)
        {
            HashVerify += name + " ";
            string hashcalculated = CalculateHash(input);
            if(hash==hashcalculated) HashVerify += "Hash codes verified\n";
            else
            {
                HashVerify += "Hash codes do not match\n";
            }
        }
        public async void LongrunCalculation(object obj)
        {
            if (User == null) User = new Model();
            string value1 = User.FirstValue, value2 = User.SecondValue, arithmetic = User.Arithmetics;
            await Task.Delay(10000);
            User.Result = "Long-running calculation: " + value1 +" "+ arithmetic+" "+value2+" = " ;
            User.Result += Calculate(value1, value2, arithmetic);
        }
    }
}
