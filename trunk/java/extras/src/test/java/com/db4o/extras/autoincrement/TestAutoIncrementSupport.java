package com.db4o.extras.autoincrement;

import com.db4o.Db4oEmbedded;
import com.db4o.EmbeddedObjectContainer;
import com.db4o.ObjectContainer;
import com.db4o.config.EmbeddedConfiguration;
import com.db4o.io.MemoryStorage;
import org.testng.annotations.BeforeMethod;
import org.testng.annotations.Test;

import static org.testng.Assert.assertEquals;

/**
 * @author roman.stoffel@gamlor.info
 * @since 10.08.2010
 */
public class TestAutoIncrementSupport {
    private MemoryStorage storage;

    @BeforeMethod
    public void setup() {
        this.storage = new MemoryStorage();
    }

    @Test
    public void createsIds() {
        ObjectContainer container = newInstance();
        WithAutoIDs toTest = storeNewId(container);
        assertEquals(toTest.getGeneratedIds(), 1);
        assertEquals(toTest.getOthterInt(), 0);
    }

    @Test
    public void incrementsIds() {
        ObjectContainer container = newInstance();
        assertEquals(storeNewId(container).getGeneratedIds(), 1);
        assertEquals(storeNewId(container).getGeneratedIds(), 2);
        assertEquals(storeNewId(container).getGeneratedIds(), 3);
    }

    @Test
    public void storingExistingObjectDoesNotIncrementId() {
        ObjectContainer container = newInstance();
        WithAutoIDs object = storeNewId(container);
        container.store(object);
        container.store(object);
        assertEquals(object.getGeneratedIds(), 1);
    }

    @Test
    public void persistIdState() {
        ObjectContainer container1 = newInstance();
        storeNewId(container1);
        storeNewId(container1);
        container1.close();
        ObjectContainer container2 = newInstance();
        assertEquals(storeNewId(container2).getGeneratedIds(), 3);

    }

    @Test
    public void keepIncrementingAfterRollback() {
        ObjectContainer container = newInstance();
        storeNewId(container);
        container.commit();
        storeNewId(container);
        container.rollback();
        container.close();

        ObjectContainer newContainer = newInstance();
        assertEquals(storeNewId(newContainer).getGeneratedIds(), 3);
    }

    @Test
    public void dontLooseIdsOnTermination() {
        ObjectContainer container = newInstance();
        storeNewId(container);
        storeNewId(container);
        container.rollback();
        assertEquals(storeNewId(container).getGeneratedIds(), 3);
    }

    @Test
    public void incrementInheritedIDs() {
        ObjectContainer container = newInstance();
        InheritedId toTest = new InheritedId();
        container.store(toTest);
        assertEquals(toTest.getGeneratedIds(), 1);
    }

    @Test
    public void dontTouchOtherObjects() {
        ObjectContainer container = newInstance();
        WithoutAutoId toTest = new WithoutAutoId();
        container.store(toTest);
        assertEquals(toTest.getOthterInt(), 0);
    }

    @Test
    public void noDoublicatesEntriesOverTime() {
        ObjectContainer container = newInstance();
        storeEntry(container.ext().openSession());
        storeEntry(container.ext().openSession());
        container.close();
        ObjectContainer container2 = newInstance();
        int id = storeNewId(container2.ext().openSession()).getGeneratedIds();
        assertEquals(id,3);
    }

    @Test(expectedExceptions = IllegalArgumentException.class)
    public void throwsIfSessionContainer() {
        ObjectContainer container = newInstance().ext().openSession();
        AutoIncrementSupport.install(container);
    }

    private void storeEntry(ObjectContainer container) {
        WithAutoIDs toTest = new WithAutoIDs();
        container.store(toTest);
        container.close();
    }

    private WithAutoIDs storeNewId(ObjectContainer container) {
        WithAutoIDs toTest = new WithAutoIDs();
        container.store(toTest);
        return toTest;
    }


    private ObjectContainer newInstance() {
        EmbeddedConfiguration configuration = Db4oEmbedded.newConfiguration();
        configuration.file().storage(storage);
        final EmbeddedObjectContainer container = Db4oEmbedded.openFile(configuration, "!No:File");
        AutoIncrementSupport.install(container);
        return container;
    }
}
