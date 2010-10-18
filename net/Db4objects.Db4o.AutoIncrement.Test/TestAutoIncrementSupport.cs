using Db4objects.Db4o.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Db4objects.Db4o.AutoIncrement.Test
{
    [TestClass]
    public class TestAutoIncrementSupport
    {
        private MemoryStorage storage;

        [TestInitialize]
        public void Setup()
        {
            storage = new MemoryStorage();
        }

        [TestMethod]
        public void CreatesIds()
        {
            var container = NewInstance();
            var toTest = StoreNewId(container);
            Assert.AreEqual(toTest.GeneratedIds, 1);
            Assert.AreEqual(toTest.OthterInt, 0);
        }

        [TestMethod]
        public void IncrementsIds()
        {
            var container = NewInstance();
            Assert.AreEqual(1, StoreNewId(container).GeneratedIds);
            Assert.AreEqual(2, StoreNewId(container).GeneratedIds);
            Assert.AreEqual(3, StoreNewId(container).GeneratedIds);
        }
        [TestMethod]
        public void IncrementsIdsOnProperty()
        {
            var container = NewInstance();
            Assert.AreEqual(1, StoreNewIdProperty(container).GeneratedIds);
            Assert.AreEqual(2, StoreNewIdProperty(container).GeneratedIds);
            Assert.AreEqual(3, StoreNewIdProperty(container).GeneratedIds);
        }

        [TestMethod]
        public void StoringExistingObjectDoesNotIncrementId()
        {
            var container = NewInstance();
            var obj = StoreNewId(container);
            container.Store(obj);
            container.Store(obj);
            Assert.AreEqual(obj.GeneratedIds, 1);
        }

        [TestMethod]
        public void PersistIdState()
        {
            using (var container1 = NewInstance())
            {
                StoreNewId(container1);
                StoreNewId(container1);
            }
            var container2 = NewInstance();
            Assert.AreEqual(StoreNewId(container2).GeneratedIds, 3);
        }

        [TestMethod]
        public void KeepIncrementingAfterRollback()
        {
            var container = NewInstance();
            StoreNewId(container);
            container.Commit();
            StoreNewId(container);
            container.Rollback();
            container.Close();

            var newContainer = NewInstance();
            Assert.AreEqual(StoreNewId(newContainer).GeneratedIds, 3);
        }

        [TestMethod]
        public void DontLooseIdsOnTermination()
        {
            var container = NewInstance();
            StoreNewId(container);
            StoreNewId(container);
            container.Rollback();
            Assert.AreEqual(StoreNewId(container).GeneratedIds, 3);
        }

        [TestMethod]
        public void IncrementInheritedIDs()
        {
            var container = NewInstance();
            var toTest = new InheritedId();
            container.Store(toTest);
            Assert.AreEqual(toTest.GeneratedIds, 1);
        }

        [TestMethod]
        public void DontTouchOtherObjects()
        {
            var container = NewInstance();
            var toTest = new WithoutAutoId();
            container.Store(toTest);
            Assert.AreEqual(toTest.OthterInt, 0);
        }

        [TestMethod]
        public void NoDoublicatesEntriesOverTime()
        {
            using (var container = NewInstance())
            {
                StoreEntry(container);
                StoreEntry(container);
            }
            var container2 = NewInstance();
            var id = StoreNewId(container2).GeneratedIds;
            Assert.AreEqual(3,id);
        }


        private static void StoreEntry(IObjectContainer rootContainer)
        {
            using (var container = rootContainer.Ext().OpenSession())
            {
                var toTest = new WithAutoIDs();
                container.Store(toTest);
            }
        }

        private static WithAutoIDs StoreNewId(IObjectContainer container)
        {
            var toTest = new WithAutoIDs();
            container.Store(toTest);
            return toTest;
        }

        private static WithAutoIdsOnProperty StoreNewIdProperty(IObjectContainer container)
        {
            var toTest = new WithAutoIdsOnProperty();
            container.Store(toTest);
            return toTest;
        }


        private IObjectContainer NewInstance()
        {
            var configuration = Db4oEmbedded.NewConfiguration();
            configuration.File.Storage = storage;
            var container = Db4oEmbedded.OpenFile(configuration, "!No:File");
            AutoIncrementSupport.Install(container);
            return container;
        }
    }
}