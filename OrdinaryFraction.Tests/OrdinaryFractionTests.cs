using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrdinaryFractionLibrary;

namespace OrdinaryFraction.Tests
{
    [TestClass]
    public class OrdinaryFractionTests
    {
        /// <summary>
        /// Тестирование деления.
        /// </summary>
        [TestMethod]
        public void OperatorDivision()
        {
            //arrange
            ordinary_fraction of1 = "5/6";
            ordinary_fraction of2 = "4/3";
            ordinary_fraction expected = "5/8"; //ожидаемое значение

            //act
            ordinary_fraction actual = of1 / of2; //наблюдаемое значение

            //assert
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Тестирование умножения.
        /// </summary>
        [TestMethod]
        public void OperatorMultiplication()
        {
            //arrange
            ordinary_fraction of1 = "5/6";
            ordinary_fraction of2 = "4/3";
            ordinary_fraction expected = "10/9"; //ожидаемое значение

            //act
            ordinary_fraction actual = of1 * of2; //наблюдаемое значение

            //assert
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Тестирование вычитания.
        /// </summary>
        [TestMethod]
        public void OperatorSubtraction()
        {
            //arrange
            ordinary_fraction of1 = "5/6";
            ordinary_fraction of2 = "4/3";
            ordinary_fraction expected = "-1/2"; //ожидаемое значение

            //act
            ordinary_fraction actual = of1 - of2; //наблюдаемое значение

            //assert
            Assert.AreEqual(expected, actual);
        }
    }
}
