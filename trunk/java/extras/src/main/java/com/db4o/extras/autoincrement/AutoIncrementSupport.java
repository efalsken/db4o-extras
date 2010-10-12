package com.db4o.extras.autoincrement;

import com.db4o.EmbeddedObjectContainer;
import com.db4o.ObjectContainer;
import com.db4o.events.*;

import java.lang.reflect.Field;
import java.util.HashMap;
import java.util.Map;

import static com.db4o.extras.autoincrement.ExceptionUtils.reThrow;

/**
 * @author roman.stoffel@gamlor.info
 * @since 10.08.2010
 */
public class AutoIncrementSupport {
    private final IdGenerator generator = new IdGenerator();
    private final Map<Class, FieldAccess> fieldAccessors = new HashMap<Class, FieldAccess>();

    private AutoIncrementSupport() {
    }

    public static void install(final ObjectContainer installOn) {
        ensureLocalContainer(installOn);
        final EventRegistry events = EventRegistryFactory.forObjectContainer(installOn);
        final AutoIncrementSupport support = new AutoIncrementSupport();

        events.creating().addListener(new EventListener4<CancellableObjectEventArgs>() {
            @Override
            public void onEvent(Event4<CancellableObjectEventArgs> eventArguments,
                                CancellableObjectEventArgs objectArgs) {
                support.incrementIdsFor(objectArgs.object(), installOn);
            }
        });
        events.committing().addListener(new EventListener4<CommitEventArgs>() {
            @Override
            public void onEvent(Event4<CommitEventArgs> commitEventArgsEvent4,
                                CommitEventArgs committingArguments) {
                support.storeState(installOn);
            }
        });
    }

    private static void ensureLocalContainer(ObjectContainer installOn) {
        if(!(installOn instanceof EmbeddedObjectContainer)){
            throw new IllegalArgumentException("This functionality currently only works on a embedded container," +
                    " but on in client-server mode");
        }
    }

    private void incrementIdsFor(Object object, ObjectContainer objectContainer) {
        final FieldAccess accessor = accessorFor(object);
        accessor.writeValue(object, generator, objectContainer);
    }

    private void storeState(ObjectContainer container) {
        generator.storeState(container);
    }

    private FieldAccess accessorFor(Object object) {
        Class theClass = object.getClass();
        FieldAccess field = fieldAccessors.get(theClass);
        if (null == field) {
            field = findFieldAccessor(theClass);
            fieldAccessors.put(theClass, field);
        }
        return field;
    }

    private FieldAccess findFieldAccessor(Class theClass) {
        final Field field = findIdField(theClass);
        return FieldAccess.create(field);
    }

    private static Field findIdField(Class theClass) {
        final Field[] fields = theClass.getDeclaredFields();
        for (Field field : fields) {
            final AutoIncrement annotation = field.getAnnotation(AutoIncrement.class);
            if (null != annotation) {
                return field;
            }
        }
        if (!theClass.getSuperclass().equals(Object.class)) {
            return findIdField(theClass.getSuperclass());
        }
        return null;
    }

    private abstract static class FieldAccess {
        public static FieldAccess NO_ID_FIELD_AVAILABLE = new NullFieldAccess();

        public abstract void writeValue(Object onObject, IdGenerator generator, ObjectContainer container);

        public static FieldAccess create(Field field) {
            if (null == field) {
                return NO_ID_FIELD_AVAILABLE;
            } else {
                field.setAccessible(true);
                return new RealFieldAccess(field);
            }
        }

        private static class NullFieldAccess extends FieldAccess {

            public void writeValue(Object obj, IdGenerator generator, ObjectContainer container) {
            }

            ;
        }

        private static class RealFieldAccess extends FieldAccess {
            private final Field field;
            private final Class forType;

            public RealFieldAccess(Field field) {
                this.field = field;
                this.forType = field.getDeclaringClass();
            }

            public void writeValue(Object obj, IdGenerator generator, ObjectContainer container) {
                int id = generator.getNextID(forType, container);
                try {
                    field.setInt(obj, id);
                } catch (Exception e) {
                    reThrow(e);
                }
            }
        }
    }
}
