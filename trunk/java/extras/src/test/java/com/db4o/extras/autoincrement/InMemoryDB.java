package com.db4o.extras.autoincrement;

import com.db4o.Db4oEmbedded;
import com.db4o.EmbeddedObjectContainer;
import com.db4o.ObjectContainer;
import com.db4o.config.EmbeddedConfiguration;
import com.db4o.io.MemoryStorage;


/**
 * @author roman.stoffel@gamlor.info
 * @since 14.07.2010
 */
class InMemoryDB {
    final MemoryStorage storage = new MemoryStorage();

    public ObjectContainer dbInstance() {
        EmbeddedConfiguration configuration = Db4oEmbedded.newConfiguration();
        configuration.file().storage(storage);
        final EmbeddedObjectContainer instance = Db4oEmbedded.openFile(configuration, "!No:File");
        AutoIncrementSupport.install(instance);
        return instance;
    }

    public static ObjectContainer newDB() {
        return new InMemoryDB().dbInstance();
    }
}
