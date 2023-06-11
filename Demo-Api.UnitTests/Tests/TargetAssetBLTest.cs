using Demo_API.BusinessLogic;
using Demo_API.Models;
using Demo_API.UnitTests.Stubs;

namespace Demo_API.UnitTests.Tests
{
    [TestClass]
    public class TargetAssetBLTest
    {
        private FakeTargetAssetService _fakeService;
        private FakeDateTimeProvider _fakeDateTimeProvider;
        private TargetAssetBL _bl;

        [TestInitialize]
        public void TestInitialize()
        {
            _fakeService = new FakeTargetAssetService(null);
            _fakeDateTimeProvider = new FakeDateTimeProvider();
            _bl = new TargetAssetBL(_fakeService, _fakeDateTimeProvider);
        }

        [TestMethod]
        public async Task ProcessTargetAssets_ShouldCalculateFields()
        {

            var assets = new List<TargetAsset>
        {
            new TargetAsset { Id = 1, ParentId = null, Status = "Running" },
            new TargetAsset { Id = 2, ParentId = 1, Status = "Stopped" },
            new TargetAsset { Id = 3, ParentId = 2, Status = "Running" },
        };

            _fakeService._targetAssets = assets;
            _fakeDateTimeProvider.Now = new DateTime(2023, 6, 3);
            // Act
            var result = await _bl.ProcessTargetAssets();

            // Assert
            foreach (var asset in result)
            {
                if (asset.Status == "Running")
                {
                    Assert.IsTrue(asset.IsStartable);
                }
                else
                {
                    Assert.IsFalse(asset.IsStartable);
                }
                Assert.AreEqual(asset.Id - 1, asset.ParentTargetAssetCount);
            }
        }

        [TestMethod]
        public async Task ProcessTargetAssets_ShouldRemoveNull()
        {
            var assets = new List<TargetAsset>
        {
            new TargetAsset { Id = 1, ParentId = null, Status = "Running" },
            null,
            new TargetAsset { Id = 2, ParentId = 1, Status = "Stopped" },
            new TargetAsset { Id = 3, ParentId = 2, Status = "Running" },
        };

            _fakeService._targetAssets = assets;
            _fakeDateTimeProvider.Now = new DateTime(2023, 6, 3);

            var result = await _bl.ProcessTargetAssets();

            Assert.AreEqual(result.Count, 3);
        }

        [TestMethod]
        public async Task ProcessTargetAssets_HandleInfiniteLoop()
        {
            var assets = new List<TargetAsset>
        {
            new TargetAsset { Id = 1, ParentId = 2, Status = "Running" },
            new TargetAsset { Id = 2, ParentId = 3, Status = "Stopped" },
            new TargetAsset { Id = 3, ParentId = 4, Status = "Running" },
            new TargetAsset { Id = 4, ParentId = 1, Status = "Running" },
        };

            _fakeService._targetAssets = assets;
            _fakeDateTimeProvider.Now = new DateTime(2023, 6, 3);

            var result = await _bl.ProcessTargetAssets();

            foreach (var asset in result)
            {
                Assert.AreEqual(3, asset.ParentTargetAssetCount); //each item will have 3 "parents"
            }
        }

        [TestMethod]
        public async Task ProcessTargetAssets_NoParentChildRelationships()
        {
            var assets = new List<TargetAsset>
    {
        new TargetAsset { Id = 1, ParentId = null, Status = "Running" },
        new TargetAsset { Id = 2, ParentId = null, Status = "Stopped" },
        new TargetAsset { Id = 3, ParentId = null, Status = "Running" },
    };

            _fakeService._targetAssets = assets;
            _fakeDateTimeProvider.Now = new DateTime(2023, 6, 3);

            var result = await _bl.ProcessTargetAssets();

            foreach (var asset in result)
            {
                Assert.AreEqual(0, asset.ParentTargetAssetCount);
            }
        }

        [TestMethod]
        public async Task ProcessTargetAssets_MultipleParentChildChains()
        {
            var assets = new List<TargetAsset>
    {
        new TargetAsset { Id = 1, ParentId = null, Status = "Running" },
        new TargetAsset { Id = 2, ParentId = 1, Status = "Stopped" },
        new TargetAsset { Id = 3, ParentId = null, Status = "Running" },
        new TargetAsset { Id = 4, ParentId = 3, Status = "Stopped" },
    };

            _fakeService._targetAssets = assets;
            _fakeDateTimeProvider.Now = new DateTime(2023, 6, 3);

            var result = await _bl.ProcessTargetAssets();

            Assert.AreEqual(0, result[0].ParentTargetAssetCount);
            Assert.AreEqual(1, result[1].ParentTargetAssetCount);
            Assert.AreEqual(0, result[2].ParentTargetAssetCount);
            Assert.AreEqual(1, result[3].ParentTargetAssetCount);
        }

        [TestMethod]
        public async Task ProcessTargetAssets_MultipleIntersectingCircularDependencies()
        {
            var assets = new List<TargetAsset>
    {
        new TargetAsset { Id = 1, ParentId = 2, Status = "Running" },
        new TargetAsset { Id = 2, ParentId = 3, Status = "Stopped" },
        new TargetAsset { Id = 3, ParentId = 4, Status = "Running" },
        new TargetAsset { Id = 4, ParentId = 1, Status = "Running" },
        new TargetAsset { Id = 5, ParentId = 1, Status = "Stopped" },
        new TargetAsset { Id = 6, ParentId = 5, Status = "Running" },
        new TargetAsset { Id = 7, ParentId = 6, Status = "Running" },
    };

            _fakeService._targetAssets = assets;
            _fakeDateTimeProvider.Now = new DateTime(2023, 6, 3);

            var result = await _bl.ProcessTargetAssets();

            Assert.AreEqual(3, result[0].ParentTargetAssetCount);
            Assert.AreEqual(3, result[1].ParentTargetAssetCount);
            Assert.AreEqual(3, result[2].ParentTargetAssetCount);
            Assert.AreEqual(3, result[3].ParentTargetAssetCount);
            Assert.AreEqual(4, result[4].ParentTargetAssetCount);
            Assert.AreEqual(5, result[5].ParentTargetAssetCount);
            Assert.AreEqual(6, result[6].ParentTargetAssetCount);
        }


        [TestMethod]
        public async Task ProcessTargetAssets_NotThirdDayOfTheMonth()
        {
            var assets = new List<TargetAsset>
    {
        new TargetAsset { Id = 1, ParentId = null, Status = "Running" },
        new TargetAsset { Id = 2, ParentId = 1, Status = "Stopped" },
        new TargetAsset { Id = 3, ParentId = 2, Status = "Running" },
    };

            _fakeService._targetAssets = assets;
            _fakeDateTimeProvider.Now = new DateTime(2023, 6, 1);

            var result = await _bl.ProcessTargetAssets();

            foreach (var asset in result)
            {
                Assert.IsFalse(asset.IsStartable);
            }
        }

        [TestMethod]
        public async Task ProcessTargetAssets_ParentIdDoesNotExistInAssetsList()
        {
            var assets = new List<TargetAsset>
            {
                new TargetAsset { Id = 1, ParentId = null, Status = "Running" },
                new TargetAsset { Id = 2, ParentId = 50, Status = "Stopped" },  // ParentId 50 doesn't exist in the list
            };

            _fakeService._targetAssets = assets;
            _fakeDateTimeProvider.Now = new DateTime(2023, 6, 3);

            var result = await _bl.ProcessTargetAssets();

            Assert.AreEqual(0, result[0].ParentTargetAssetCount);
            Assert.AreEqual(0, result[1].ParentTargetAssetCount); // Should be 0 as ParentId 50 doesn't exist
        }


        [TestMethod]
        public async Task ProcessTargetAssets_ParentIdIsSameAsId()
        {
            var assets = new List<TargetAsset>
            {
                new TargetAsset { Id = 1, ParentId = null, Status = "Running" },
                new TargetAsset { Id = 2, ParentId = 2, Status = "Stopped" },  // ParentId is the same as Id
            };

            _fakeService._targetAssets = assets;
            _fakeDateTimeProvider.Now = new DateTime(2023, 6, 3);

            var result = await _bl.ProcessTargetAssets();

            Assert.AreEqual(0, result[0].ParentTargetAssetCount);
            Assert.AreEqual(0, result[1].ParentTargetAssetCount); // Should be 0 as it should not count itself as a parent
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException),"Endpoint returned invalid data")]
        public async Task ProcessTargetAssets_WillThrowErrorOnNull()
        {
            _fakeService._targetAssets = null;
            _fakeDateTimeProvider.Now = new DateTime(2023, 6, 3);

            var result = await _bl.ProcessTargetAssets();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Endpoint returned invalid data")]
        public async Task ProcessTargetAssets_WillThrowErrorOnEmpty()
        {
            _fakeService._targetAssets = new List<TargetAsset>();
            _fakeDateTimeProvider.Now = new DateTime(2023, 6, 3);

            var result = await _bl.ProcessTargetAssets();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Endpoint returned invalid data")]
        public async Task ProcessTargetAssets_WillThrowErrorOnOnlyNulls()
        {
            var assets = new List<TargetAsset>
        {
            null,
            null
        };
            _fakeService._targetAssets = assets;
            _fakeDateTimeProvider.Now = new DateTime(2023, 6, 3);

            var result = await _bl.ProcessTargetAssets();
        }




        // Add more tests here for other cases (e.g., empty list, null list, etc.)
    }

}
