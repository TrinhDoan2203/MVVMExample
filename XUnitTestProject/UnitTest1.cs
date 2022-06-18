using System;
using System.IO;
using Xunit;

namespace XUnitTestProject
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("1234", "123", "+", "1,357.00")]
        [InlineData("1234", "123", "-", "1,111.00")]
        [InlineData("1234", "123", "*", "151,782.00")]
        [InlineData("1234", "123", "/", "10.03")]
        [InlineData("1234", "123", "'", "Could not use the arithmetic: '")]
        [InlineData("1234", "123ee", "-", "Unable to parse 123ee")]
        [InlineData("1234ee", "123", "*", "Unable to parse 1234ee")]
        [InlineData("", "123", "'", "Input value(s) is null or empty")]
        public void CalculateTest(string value1, string value2, string arithmetic, string expected)
        {
            MVVMExample.MyViewModel vmTest = new MVVMExample.MyViewModel();
            string result = vmTest.Calculate(value1, value2,arithmetic);
            Assert.Equal(expected, result);
        }
        [Theory]
        [InlineData(typeof(MVVMExample.Model), "12", "56.7", "68.7", "+", "SerialData.xml",true)]
        [InlineData(typeof(MVVMExample.Model), "12", "56.7", "68.7", "+", "SerialData.txt", false)]

        public void SerializeTest(Type datatype, string fvalue, string svalue, string result, string arithmetic, string file, bool expected)
        {
            string filePath = Directory.GetCurrentDirectory() + "\\" + file;
            MVVMExample.MyViewModel vmTest = new MVVMExample.MyViewModel();
            MVVMExample.Model User = new MVVMExample.Model();
            User.FirstValue = fvalue;
            User.SecondValue = svalue;
            User.Result = result;
            User.Arithmetics = arithmetic;
            bool sucessful=vmTest.XmlSerializeMethod(datatype,User,filePath);
            Assert.Equal(expected,sucessful);
        }
        [Theory]
        [InlineData(typeof(MVVMExample.Model),"SerialData2.xml", "123","456","579.00","+")]
        [InlineData(typeof(MVVMExample.Model), "SerialDataError.xml", "", "", "", "")]
        [InlineData(typeof(MVVMExample.Model), "asaSerialData2.xml", "", "", "", "")]
        public void DeserializeTest(Type datatype,  string file, string fvalue, string svalue, string result, string arithmetic)
        {
            string filePath = Directory.GetCurrentDirectory() + "\\" + file;
            //string filePath = "C:\\Users\\trinh\\source\\repos\\MVVMExample\\MVVMExample\\bin\\Debug\\netcoreapp3.1\\SerialData2.xml";
            MVVMExample.MyViewModel vmTest = new MVVMExample.MyViewModel();
            vmTest.User = vmTest.XmlDeserializeMethod(datatype, filePath);
            Assert.Equal(fvalue, vmTest.User.FirstValue);
            Assert.Equal(svalue, vmTest.User.SecondValue);
            Assert.Equal(result, vmTest.User.Result);
            Assert.Equal(arithmetic, vmTest.User.Arithmetics);
        }
    }
}
