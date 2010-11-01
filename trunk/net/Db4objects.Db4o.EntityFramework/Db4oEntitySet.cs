using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Db4objects.Db4o.Linq;
using System.ComponentModel;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.EntityFramework {
	public class Db4oEntitySet<TSource> : IDb4oEntitySet, System.Data.Objects.IObjectSet<TSource>, IDb4oLinqQueryable<TSource>, IListSource where TSource : class {
		public string Name { get; private set; }
		public Type Type { get; private set; }
		public Db4oEntityContext Context { get; private set; }
		protected readonly IDb4oLinqQueryable<TSource> queryable;

		public Db4oEntitySet(Db4oEntityContext dataContext){
			Context = dataContext;
			queryable = dataContext.ObjectContainer.AsQueryable<TSource>();
			this.Name = typeof(TSource).Name;
			this.Type = typeof(TSource);
		}

		public Db4oEntitySet(Db4oEntityContext dataContext, string name) : this(dataContext) {
			Name = name;
		}

		public override bool Equals(object obj) {
			var otherObj = obj as Db4oEntitySet<TSource>;
			if (obj == null) return false;
			return (otherObj.Type == this.Type) && (otherObj.Context == this.Context);
		}

		#region interface IDb4oLinqQueryable
		public IDb4oLinqQuery GetQuery() {
			return queryable.GetQuery();
		}

		public Type ElementType {
			get { return queryable.ElementType; }
		}

		public System.Linq.Expressions.Expression Expression {
			get { return queryable.Expression; }
		}

		public IQueryProvider Provider {
			get { return queryable.Provider; }
		}

		public System.Collections.IEnumerator GetEnumerator() {
			return queryable.GetEnumerator();
		}

		IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() {
			return queryable.GetEnumerator();
		}
		#endregion

		#region interface IObjectSet
		public void AddObject(TSource entity) {
			Context.ObjectContainer.Store(entity);
			//return entity;
		}

		public void Attach(TSource entity) {
			throw new NotSupportedException();
		}

		public void DeleteObject(TSource entity) {
			Context.ObjectContainer.Delete(entity);
		}

		public void Detach(TSource entity) {
			//Context.ObjectContainer.Ext().Purge(entity);
			throw new NotSupportedException();
		}
		#endregion

		#region interface IListSource
		bool IListSource.ContainsListCollection {
			get { return false; }
		}

		System.Collections.IList IListSource.GetList() {
			return this.ToList<TSource>();
		}
		#endregion
	}
}
