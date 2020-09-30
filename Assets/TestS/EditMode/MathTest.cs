using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class MathTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void Multiplying_2_Numbers()
        {
            var myTest = new MyTest();
            var num1 = 2;
            var num2 = 5;
            var expectedNum = (2 * 5);
            var actualNum = myTest.GetNumber(num1, num2);

            Assert.AreEqual(actualNum, expectedNum);
        }
    }
}
