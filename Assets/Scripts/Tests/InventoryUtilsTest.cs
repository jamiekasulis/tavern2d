using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InventoryUtilsTest
{
    private Item testItem1, testItem2;

    [SetUp] public void Init()
    {
        // Set up test items
        testItem1 = ScriptableObject.CreateInstance<Item>();
        testItem1.itemName = "test item 1";
        testItem1.description = "this is test item 1";
        testItem1.id = "1";

        testItem2 = ScriptableObject.CreateInstance<Item>();
        testItem2.itemName = "test item 2";
        testItem2.description = "description 2";
        testItem2.id = "2";
    }

    #region GetIndexInInventory

    [Test]
    public void TestGetIndexInInventory_WhenItemIsPresent()
    {
        List<ItemQuantity> inv = new()
        {
            new ItemQuantity()
            {
                item = testItem1,
                quantity = 1
            },
            new ItemQuantity()
            {
                item = testItem2,
                quantity = 1
            }
        };
        Assert.AreEqual(
            1,
            InventoryUtils.GetIndexInInventory(testItem2, inv)
        );
    }

    [Test]
    public void TestGetIndexInInventory_WhenItemIsNotPresent()
    {
        List<ItemQuantity> inv = new()
        {
            new ItemQuantity()
            {
                item = testItem1,
                quantity = 1
            }
        };
        Assert.AreEqual(
            -1,
            InventoryUtils.GetIndexInInventory(testItem2, inv)
        );
    }

    #endregion

    #region ContainsItem

    [Test]
    public void TestContainsItem()
    {
        List<ItemQuantity> inv = new()
        {
            new ItemQuantity()
            {
                item = testItem1,
                quantity = 1
            }
        };

        Assert.AreEqual(
            InventoryUtils.ContainsItem(testItem1, inv),
            true
        );
        Assert.AreEqual(
            InventoryUtils.ContainsItem(testItem2, inv),
            false
        );
    }

    #endregion

    #region Add

    [Test]
    public void TestAdd_WhenItemNotPresent()
    {
        List<ItemQuantity> inv = new()
        {
            new ItemQuantity()
            {
                item = testItem1,
                quantity = 1
            }
        };

        InventoryUtils.Add(new() { item = testItem2, quantity = 1 }, inv, false);
        Assert.AreEqual(inv.Count, 2);
        Assert.AreEqual(inv[1].item, testItem2);
        Assert.AreEqual(inv[1].quantity, 1);
    }

    [Test]
    public void TestAdd_WhenItemIsPresent()
    {
        List<ItemQuantity> inv = new()
        {
            new ItemQuantity()
            {
                item = testItem1,
                quantity = 1
            }
        };

        InventoryUtils.Add(new() { item = testItem1, quantity = 1 }, inv, false);
        Assert.AreEqual(inv.Count, 1);
        Assert.AreEqual(inv[0].item, testItem1);
        Assert.AreEqual(inv[0].quantity, 2);
    }

    [Test]
    public void TestAdd_ThrowsException_IfInventoryIsFull_AndIsStrict()
    {
        List<ItemQuantity> inv = new(1);
        inv.Add(new() { item = testItem1, quantity = 1 });

        Assert.Throws<InventoryFullException>(() =>
            InventoryUtils.Add(
                new() { item = testItem2, quantity = 100 },
                inv,
                true
            )
        );
    }

    [Test]
    public void TestAdd_DoesNotThrowException_IfUnstrict()
    {
        List<ItemQuantity> inv = new(1);
        inv.Add(new() { item = testItem1, quantity = 1 });


        InventoryUtils.Add(
            new() { item = testItem2, quantity = 100 },
            inv,
            false
        );
        Assert.AreEqual(inv.Count, 1);
    }

    #endregion

    #region Remove

    [Test]
    public void TestRemove_WhenItemIsPresent_Unstrict_Success()
    {
        List<ItemQuantity> inv = new()
        {
            new ItemQuantity()
            {
                item = testItem1,
                quantity = 1
            }
        };

        InventoryUtils.Remove(
            new() { item = testItem1, quantity = 1 },
            inv,
            false
        );
        Assert.AreEqual(inv.Count, 0);
    }

    [Test]
    public void TestRemove_WhenQuantityTooSmall_Strict_ThrowsException()
    {
        List<ItemQuantity> inv = new()
        {
            new ItemQuantity()
            {
                item = testItem1,
                quantity = 1
            }
        };

        Assert.Throws<InvalidQuantityException>(() =>
            InventoryUtils.Remove(
                new() { item = testItem1, quantity = 100 },
                inv,
                true
            )
        );
    }

    [Test]
    public void TestRemove_WhenItemNotPresent_Strict_ThrowsException()
    {
        List<ItemQuantity> inv = new()
        {
            new ItemQuantity()
            {
                item = testItem1,
                quantity = 1
            }
        };

        Assert.Throws<InvalidQuantityException>(() =>
            InventoryUtils.Remove(
                new() { item = testItem2, quantity = 1 },
                inv,
                true
            )
        );
    }

    [Test]
    public void TestRemove_WhenItemIsPresent_Strict_Success()
    {
        List<ItemQuantity> inv = new()
        {
            new ItemQuantity()
            {
                item = testItem1,
                quantity = 1
            }
        };

        InventoryUtils.Remove(
            new() { item = testItem1, quantity = 1 },
            inv,
            true
        );
        Assert.AreEqual(inv.Count, 0);
    } 

    [Test]
    public void TestRemove_WhenItemIsNotPresent_Unstrict_Success()
    {
        List<ItemQuantity> inv = new()
        {
            new ItemQuantity()
            {
                item = testItem1,
                quantity = 1
            }
        };

        InventoryUtils.Remove(
            new() { item = testItem2, quantity = 1 },
            inv,
            false
        );
        Assert.AreEqual(inv.Count, 1);
    }

    #endregion

    #region HasAtLeast

    [Test]
    public void TestHasAtLeast_WhenItemPresent_ButQuantityTooSmall()
    {
        List<ItemQuantity> inv = new()
        {
            new ItemQuantity()
            {
                item = testItem1,
                quantity = 1
            }
        };

        Assert.AreEqual(
            false,
            InventoryUtils.HasAtLeast(
                new() { item = testItem1, quantity = 2 },
                inv
            )
        );
    }

    [Test]
    public void TestHasAtLeast_WhenItemNotPresent()
    {
        List<ItemQuantity> inv = new()
        {
            new ItemQuantity()
            {
                item = testItem1,
                quantity = 1
            }
        };

        Assert.AreEqual(
            false,
            InventoryUtils.HasAtLeast(
                new() { item = testItem2, quantity = 1 },
                inv
            )
        );
    }

    [Test]
    public void TestHasAtLeast_WhenItemPresent_AndQuantityOK()
    {
        List<ItemQuantity> inv = new()
        {
            new ItemQuantity()
            {
                item = testItem1,
                quantity = 5
            }
        };

        Assert.AreEqual(
            true,
            InventoryUtils.HasAtLeast(
                new() { item = testItem1, quantity = 2 },
                inv
            )
        );
        Assert.AreEqual(
            true,
            InventoryUtils.HasAtLeast(
                new() { item = testItem1, quantity = 5 },
                inv
            )
        );
    }

    #endregion

}