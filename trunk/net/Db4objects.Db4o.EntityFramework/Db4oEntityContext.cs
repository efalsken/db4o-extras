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

namespace Db4objects.Db4o.EntityFramework {
	public class Db4oEntityContext : DynamicObject, IDisposable {
		
		private readonly bool _createdConnection = false;
		public IObjectContainer ObjectContainer { get; private set; }
		protected List<IDb4oEntitySet> _RegisteredTypes = new List<IDb4oEntitySet>();

		#region Constructor
		public Db4oEntityContext(string dbFileName, Db4o.Config.IEmbeddedConfiguration config = null) 
			: this(CreateConnection(dbFileName, config)){
			_createdConnection = true;
		}

		public Db4oEntityContext(IObjectContainer db) {
			Requires.NotNull("db", db);
			if (db.Ext().IsClosed()) throw new ArgumentException("ObjectContainer must still be open.", "db");
			ObjectContainer = db;
			//TODO OnContextCreated
		}

		private static IObjectContainer CreateConnection(string dbFileName, Db4o.Config.IEmbeddedConfiguration config = null) {
			if (config == null) {
				//TODO OnConfigurationCreating
				config = Db4oEmbedded.NewConfiguration();
			}
			//TODO OnConfigurationCreated
			//TODO OnConnectionCreating
			var db = Db4oEmbedded.OpenFile(config, dbFileName);
			//TODO OnConnectionCreated
			return db;
		}
		#endregion

		#region IDisposable
		public void Dispose() {
			GC.SuppressFinalize(this);
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (_createdConnection && this.ObjectContainer != null)
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

		/// <summary>
		/// Persists all changes to the database.
		/// </summary>
		public void SaveChanges() {
			OnSavingChanges();
			ObjectContainer.Commit();
		}

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

		public event EventHandler SavingChanges;

		protected Db4oEntitySet<TEntity> GetObjectSet<TEntity>(string typeAlias) where TEntity : class {
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

		//protected struct NamedType {
		//    public readonly string Name;
		//    public readonly Type Type;
			
		//    public NamedType(Db4oObjectSet<object> objectSet){
		//        this.Type = objectSet.Type;
		//        this.Name = objectSet.Name;
		//    }
		//}

		public IDb4oEntitySet RegisterTypeAlias(string typeAlias, Type type) {
			if(_RegisteredTypes.Any(x => x.Name == typeAlias))
				throw new ArgumentException("Alias is already in use.", "typeAlias");
			//if (this.GetType().GetMembers().Any(m => m.Name == typeAlias))
			//    throw new ArgumentException("Illegal alias name.", "typeAlias");

			var osetType = typeof(Db4oEntitySet<>).MakeGenericType(type);
			var oset = Activator.CreateInstance(typeof(Db4oEntitySet<>).MakeGenericType(type), this, typeAlias);

			this._RegisteredTypes.Add((IDb4oEntitySet)oset);
			return (IDb4oEntitySet)oset;
		}

		public override bool Equals(object obj) {
			var otherObj = obj as Db4oEntityContext;
			if (otherObj == null) return false;
			return (otherObj.ObjectContainer == this.ObjectContainer);
		}
	}
}
