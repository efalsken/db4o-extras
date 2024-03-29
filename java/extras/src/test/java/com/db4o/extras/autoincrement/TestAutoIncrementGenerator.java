package com.db4o.extras.autoincrement;

import com.db4o.ObjectContainer;
import org.testng.annotations.BeforeMethod;
import org.testng.annotations.Test;

import static org.testng.Assert.assertEquals;

/**
 * @author roman.stoffel@gamlor.info
 * @since 10.08.2010
 */
public class TestAutoIncrementGenerator {
    private IdGenerator toTest;
    private InMemoryDB databases;

    @BeforeMethod
    public void setup(){
        this.databases = new InMemoryDB();
        this.toTest = new IdGenerator();
    }

    @Test
    public void createsIds(){
        ObjectContainer container = databases.dbInstance();
        final int id = increment(container);
        assertEquals(id,1);
    }

    @Test
    public void incrementIds(){
        ObjectContainer container = databases.dbInstance();
        assertEquals(increment(container),1);
        assertEquals(increment(container),2);
        assertEquals(increment(container),3);
    }

    @Test
    public void storesState(){
        storeNewObject();

        toTest = new IdGenerator();
        ObjectContainer container2 = databases.dbInstance();
        assertEquals(increment(container2),2);
    }

    @Test
    public void updatesState()
    {
        toTest = new IdGenerator();
        storeNewObject();
        toTest = new IdGenerator();
        storeNewObject();

        toTest = new IdGenerator();
        ObjectContainer container2 = databases.dbInstance();
        assertEquals(increment(container2),3);

    }

    private void storeNewObject() {
        ObjectContainer container1 = databases.dbInstance();
        toTest.getNextID(WithAutoIDs.class, container1);
        toTest.storeState(container1);
        container1.close();
    }

    private int increment(ObjectContainer container1) {
        return toTest.getNextID(WithAutoIDs.class, container1);
    }

}
