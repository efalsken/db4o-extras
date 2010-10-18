package com.db4o.extras.autoincrement;

import com.db4o.ObjectContainer;
import com.db4o.ObjectSet;

import java.util.HashMap;
import java.util.Map;

/**
 * @author roman.stoffel@gamlor.info
 * @since 10.08.2010
 */
class IdGenerator {
    private PersistedAutoIncrements state = null;

    synchronized int getNextID(Class forClass, ObjectContainer container) {
        PersistedAutoIncrements incrementState = ensureLoadedIncrements(container);
        return incrementState.nextNumber(forClass);
    }

    synchronized void storeState(ObjectContainer container) {
        if (null != state) {
            if (onlyNeedUpdate(container)) {
                container.store(state.currentHighestIds);
            } else {
                container.store(state);
            }
        }
    }

    private boolean onlyNeedUpdate(ObjectContainer container) {
        return container.ext().isStored(state);
    }

    private PersistedAutoIncrements ensureLoadedIncrements(ObjectContainer container) {
        if (null == state) {
            state = loadOrCreateState(container);
        }
        return state;

    }

    private PersistedAutoIncrements loadOrCreateState(ObjectContainer container) {
        ObjectSet<PersistedAutoIncrements> existingState = container.query(PersistedAutoIncrements.class);
        if (0 == existingState.size()) {
            return new PersistedAutoIncrements();
        } else if (1 == existingState.size()) {
            return existingState.get(0);
        } else {
            throw new IllegalStateException("Cannot have more than one state stored in database");
        }
    }

    private static class PersistedAutoIncrements {
        private final Map<Class, Integer> currentHighestIds = new HashMap<Class, Integer>();

        public int nextNumber(Class forClass) {
            Integer number = currentHighestIds.get(forClass);
            if (null == number) {
                number = 0;
            }
            number += 1;
            currentHighestIds.put(forClass, number);
            return number;
        }
    }

}
