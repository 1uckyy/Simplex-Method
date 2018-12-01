using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using simplex_method;

namespace TestConvertDouble.Tests
{
    [TestClass]
    public class DoubleConvert
    {
        [TestMethod]
        public void ConvertTest()
        {
            //arrange
            double a = -3;
            string expected = "-1/4";

            //act
            string actual = DoubleToFraction.Convert(a); //наблюдаемое значение

            //assert
            Assert.AreEqual(expected, actual);
        }
    }
}
