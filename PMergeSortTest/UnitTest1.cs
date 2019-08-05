using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests
{
    public class Tests
    {
        List<int> testGroup;

        [SetUp]
        public void Setup()
        {
            Random rnd = new Random();
            testGroup = new List<int>();
            for(int i=0; i<1000000; i++)
            {
                testGroup.Add(rnd.Next(671248));
            }
        }

        [Test]
        public void Test1()
        {
            List<int> res = ParallelMergeSort.PMergeSort<int>.Sort(testGroup, true, 100000);
            foreach(var e in res)
            {
                Console.WriteLine(e);
            }
            Assert.Pass();
        }
    }
}