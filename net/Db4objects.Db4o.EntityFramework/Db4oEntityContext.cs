using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Reflection;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Config;

namespace Db4objects.Db4o.EntityFramework {
	public class Db4oEntityContext : DynamicObject, IDisposable {
		private bool _createdObjectContainer = false;
		public IObjectContainer ObjectContainer { get; private set; }
		protected List<IDb4oEntitySet> _RegisteredTypes = new List<IDb4oEntitySet>();

		#region Constructor
		public Db4oEntityContext(string dbFileName) {
			CreateConnection(dbFileName);
			OnContextCreated();
		}

		public Db4oEntityContext(IObjectContainer db) {
			Requires.NotNull("db", db);
			Requires.IsFalse(db.Ext().IsClosed());
			ObjectContainer = db;
			OnContextCreated();
		}

		public Db4oEntityContext() {
			CreateConnection(this.GetType().Name + ".db4o");
			OnContextCreated();
		}

		protected void CreateConnection(string dbFileName = null) {
			var config = OnConfigurationCreating() ?? Db4oEmbedded.NewConfiguration();
			OnConfigurationCreated(config);
			if (config == null) throw new InvalidOperationException("Could not create db4o Configuration.");

			var db = OnConnectionCreating(config);
			if (db == null) {
				if (config is IEmbeddedConfiguration) {
					db = Db4oEmbedded.OpenFile((IEmbeddedConfiguration)config, dbFileName);
					_createdObjectContainer = true;
				}
			}

			if (db == null)
				throw new InvalidOperationException("Could not create db4o ObjectContainer. (if you provided your own config, did you remember to overload OnConnectionCreating?)");
			
			this.ObjectContainer = db;
			
			OnConnectionCreated(db);
		}
		#endregion

		#region IDisposable
		public void Dispose() {
			GC.SuppressFinalize(this);
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (_createdObjectContainer && this.ObjectContainer != null)
					ObjectContainer.Close();
				this.ObjectContainer = null;
			}
		}
		#endregion

		#region DynamicObject overrides
		public override IEnumerable<string> GetDynamicMemberNames() {
			//List<Type> allTypes = new List<Type>();
			//foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
			//    allTypes.AddRange(asm.GetExportedTypes());

			//List<Type> storedTypes = new List<Type>();
			//foreach (IStoredClass sc in ObjectContainer.Ext().StoredClasses()) {
			//    var sctype = Type.GetType(sc.GetName(), false);
			//    if (sctype == null) continue;
			//    storedTypes.Add(sctype);
			//}
			////allTypes.Intersect(storedTypes)
			//return (from Type t in storedTypes select t.Name).Distinct();

			return from objectSet in _RegisteredTypes select objectSet.Name;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result) {
			result = null;
			var typeAlias = binder.Name;
			var results = from objectSet in _RegisteredTypes
						  where objectSet.Name == typeAlias
						  select objectSet;
			var ret = results.SingleOrDefault();
			if (ret == null) return false;
			//var modelType = ret.Type;
			//var retType = typeof(IQueryable<>);
			//retType = retType.MakeGenericType(modelType);
			//if (retType == null) return false;
			//binder.ReturnType = retType;
			result = ret;
			return true;
		}
		#endregion

		#region CRUD
		public void AcceptAllChanges() {
			ObjectContainer.Commit();
		}

		public void AddObject<T>(T entity) {
			if (_RegisteredTypes.Any(x => x.Type == typeof(T)))
				ObjectContainer.Store(entity);
			else throw new ArgumentException("Unknown type: " + typeof(T).Name);
		}

		public void DeleteObject<T>(T entity) {
			if (_RegisteredTypes.Any(x => x.Type == typeof(T)))
				ObjectContainer.Delete(entity);
			else throw new ArgumentException("Unknown type: " + typeof(T).Name);
		}

		public void AttachTo<T>(T entity) {
			throw new NotSupportedException();
		}

		public void Detatch<T>(T entity) {
			throw new NotSupportedException();
		}

		public void Refresh<T>(T entity) {
			ObjectContainer.Ext().Refresh(entity, ObjectContainer.Ext().Configure().ActivationDepth());
		}
		
		#region Saving Changes
		/// <summary>
		/// Persists all changes to the database.
		/// </summary>
		public void SaveChanges() {
			OnSavingChanges();
			ObjectContainer.Commit();
		}

		public event EventHandler SavingChanges;
		private void OnSavingChanges() {
			if (this.SavingChanges != null)
				this.SavingChanges(this, EventArgs.Empty);
		}

		public IQueryable<TEntity> GetEnities<TEntity>() where TEntity : class {
			var ret = (from objectSet in _RegisteredTypes
					   where objectSet.Type == typeof(TEntity)
					   select objectSet).SingleOrDefault();
			if (ret == null || !(ret is IQueryable<TEntity>)) return new TEntity[0].AsQueryable();
			else return (IQueryable<TEntity>)ret;
		}
		#endregion
		#endregion

		#region Events
		public event EventHandler ContextCreated;
		protected virtual void OnContextCreated() {
			if (ContextCreated != null)
				ContextCreated(this, EventArgs.Empty);
		}

		public event EventHandler<ConfigurationEventArgs> ConfigurationCreating;
		protected virtual ICommonConfigurationProvider OnConfigurationCreating(ICommonConfigurationProvider config = null) {
			if (ConfigurationCreating != null) {
				var args = new ConfigurationEventArgs() { Configuration = config };
				ConfigurationCreating(this, args);
				return args.Configuration;
			}
			return config;
		}

		public event EventHandler<ConfigurationEventArgs> ConfigurationCreated;
		protected void OnConfigurationCreated(ICommonConfigurationProvider config) {
			if (ConfigurationCreated != null)
				ConfigurationCreated(this, new ConfigurationEventArgs() { Configuration = config });
		}

		public event EventHandler<ConnectionCreatingEventArgs> ConnectionCreating;
		protected virtual IObjectContainer OnConnectionCreating(ICommonConfigurationProvider config) {
			if(ConnectionCreating != null){
				var args = new ConnectionCreatingEventArgs(config);
				ConnectionCreating(this, args);
				return args.ObjectContainer ;
			}
			return null;
		}

		public event EventHandler<ConnectionEventArgs> ConnectionCreated;
		protected virtual void OnConnectionCreated(IObjectContainer objectContainer) {
			if (ConnectionCreated != null)
				ConnectionCreated(this, new ConnectionEventArgs() { ObjectContainer = objectContainer });
		}
		#endregion

		protected Db4oEntitySet<TEntity> GetEntitySet<TEntity>(string typeAlias) where TEntity : class {
			var result = (from os in _RegisteredTypes
						  where os.Type == typeof(TEntity) && os.Name == typeAlias
						  select os).SingleOrDefault();

			if (result == null) {
				result = RegisterTypeAlias(typeAlias, typeof(TEntity));
			}
			
			if(result is Db4oEntitySet<TEntity>)
				return (Db4oEntitySet<TEntity>)result;
			throw new ArgumentException(String.Format("Cannot find an ObjectSet mapping for type '{0}' and name '{1}'.", typeof(TEntity), typeAlias));
		}

		public IDb4oEntitySet RegisterTypeAlias(string typeAlias, Type type) {
			if(_RegisteredTypes.Any(x => x.Name == typeAlias))
				throw new ArgumentException("Alias is already in use.", "typeAlias");
			//hack: this is probably a good idea, but prevents derived instances from creating strongly-typed access members.
			//if (this.GetType().GetMembers().Any(m => m.Name == typeAlias))
			//    throw new ArgumentException("Illegal alias name.", "typeAlias");

			var osetType = typeof(Db4oEntitySet<>).MakeGenericType(type);
			var oset = Activator.CreateInstance(osetType, this, typeAlias);

			this._RegisteredTypes.Add((IDb4oEntitySet)oset);
			return (IDb4oEntitySet)oset;
		}

		public override bool Equals(object obj) {
			var otherObj = obj as Db4oEntityContext;
			if (otherObj == null) return false;
			return (otherObj.ObjectContainer == this.ObjectContainer);
		}

		public override int GetHashCode() {
			return ObjectContainer.GetHashCode();
		}
	}
}
