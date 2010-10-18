using System;
using System.Collections.Generic;
using System.Linq;
using Db4objects.Db4o;
using Db4objects.Db4o.Linq;

namespace Db4objects.Db4o.AutoIncrement {
	internal class IdGenerator {
		private PersistedAutoIncrements state = null;

		public int NextId(Type type, IObjectContainer container) {
			var incrementState = EnsureLoadedIncrements(container);
			return incrementState.NextNumber(type);
		}

		public void StoreState(IObjectContainer container) {
            if (null != state)
            {
                if (OnlyNeedUpdate(container))
                {
                    container.Store(state.CurrentHighestIds); 
                } else
                {
                    container.Store(state);  
                }
			}
		}

	    private bool OnlyNeedUpdate(IObjectContainer container)
	    {
	        return container.Ext().IsStored(state);
	    }

	    private PersistedAutoIncrements EnsureLoadedIncrements(IObjectContainer container) {
			return state ?? (state = loadOrCreateState(container));
		}

		private static PersistedAutoIncrements loadOrCreateState(IObjectContainer container) {
			var existingState = container.Query<PersistedAutoIncrements>().SingleOrDefault();
			return existingState ?? new PersistedAutoIncrements();
		}

		private class PersistedAutoIncrements {
			private readonly IDictionary<Type, int> currentHighestIds = new Dictionary<Type, int>();

			public int NextNumber(Type type) {
				var number = 0;
				if (!currentHighestIds.TryGetValue(type, out number)) {
					number = 0;
				}
				number += 1;
				currentHighestIds[type] = number;
				return number;
			}

		    public IDictionary<Type, int> CurrentHighestIds
		    {
		        get { return currentHighestIds; }
		    }
		}
	}
}