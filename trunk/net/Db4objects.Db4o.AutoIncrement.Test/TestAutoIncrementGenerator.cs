using Db4objects.Db4o.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Db4objects.Db4o.AutoIncrement.Test
{
    [TestClass]
    public class TestAutoIncrementGenerator
    {
        private IdGenerator toTest;
        private MemoryStorage fileSystem = new MemoryStorage();

        [TestInitialize]
        public void Setup()
        {
            fileSystem = new MemoryStorage();
            this.toTest = new IdGenerator();
        }

        [TestMethod]
        public void CreatesIds()
        {
            var container = NewInstance();
            var id = Increment(container);
            Assert.AreEqual(id, 1);
        }

        [TestMethod]
        public void IncrementIds()
        {
            var container = NewInstance();
            Assert.AreEqual(Increment(container), 1);
            Assert.AreEqual(Increment(container), 2);
            Assert.AreEqual(Increment(container), 3);
        }

        [TestMethod]
        public void StoresState()
        {
            using (var container1 = NewInstance())
            {
                toTest.NextId(typeof(WithAutoIDs), container1);
                toTest.StoreState(container1);
            }

            toTest = new IdGenerator();
            using (var container2 = NewInstance())
            {
                Assert.AreEqual(Increment(container2), 2);
            }
        }
        [TestMethod]
        public void UpdatesState()
        {
            toTest = new IdGenerator();
            StoreNewObject();
            toTest = new IdGenerator();
            StoreNewObject();

            toTest = new IdGenerator();
            using (var container = NewInstance())
            {
                Assert.AreEqual( 3,Increment(container));
            }
        }

        private void StoreNewObject()
        {
            using (var container= NewInstance())
            {
                toTest.NextId(typeof(WithAutoIDs), container);
                toTest.StoreState(container);
            }
        }

        private int Increment(IObjectContainer container1)
        {
            return toTest.NextId(typeof(WithAutoIDs), container1);
        }


        private IObjectContainer NewInstance()
        {
            var configuration = Db4oEmbedded.NewConfiguration();
            configuration.File.Storage = fileSystem;
            return Db4oEmbedded.OpenFile(configuration, "!In:Memory");
        }
    }
}