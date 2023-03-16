
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ListUtilsTest
{
    private int[,] matrix;
    List<int> flatList;

    [SetUp]
    public void Init()
    {
        matrix = new int[,]
        {
            {0, 1 },
            {2, 3 },
            {4, 5 }
        };

        flatList = new(6)
        {
            0, 1, 2, 3, 4, 5
        };
    }

    [Test]
    public void TestFlattenEnumerable()
    {
        Assert.AreEqual(flatList, ListUtils.FlattenToList(matrix));
    }
}
