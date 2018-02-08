using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EntityFramework.Utilities
{
	public class EFUQueryProvider<T> : ExpressionVisitor, System.Linq.IQueryProvider
	{
		internal IQueryable Source;

		public EFUQueryProvider(IQueryable source)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
		}

		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			if (expression == null) throw new ArgumentNullException(nameof(expression));

			return new EFUQueryable<TElement>(Source, expression) as IQueryable<TElement>;
		}

		public IQueryable CreateQuery(Expression expression)
		{
			if (expression == null) throw new ArgumentNullException(nameof(expression));
			var elementType = expression.Type.GetGenericArguments().First();
			var result = (IQueryable)Activator.CreateInstance(typeof(EFUQueryable<>).MakeGenericType(elementType),
				new object[] { Source, expression });
			return result;
		}

		public TResult Execute<TResult>(Expression expression)
		{
			if (expression == null) throw new ArgumentNullException(nameof(expression));
			var result = Execute(expression);
			return (TResult)result;
		}

		public object Execute(Expression expression)
		{
			if (expression == null) throw new ArgumentNullException(nameof(expression));

			var efuQuery = GetIncludeContainer(expression);
			var translated = Visit(expression);
			var result = Source.Provider.Execute(translated);

			var first = efuQuery.Includes.First();
			first.SingleItemLoader(result);

			return result;
		}

		internal IEnumerable ExecuteEnumerable(Expression expression)
		{
			if (expression == null) throw new ArgumentNullException(nameof(expression));

			var modifiers = GetModifiersForQuery(expression);

			var efuQuery = GetIncludeContainer(expression);
			var translated = Visit(expression);
			var translatedQuery = Source.Provider.CreateQuery(translated);
			var list = new List<object>();
			foreach (var item in translatedQuery)
			{
				list.Add(item);
			}

			var first = efuQuery.Includes.First();
			first.Loader(modifiers, list);

			return list;
		}

		private static List<MethodCallExpression> GetModifiersForQuery(Expression expression)
		{
			var modifiers = new List<MethodCallExpression>();
			var temp = expression;
			while (temp is MethodCallExpression)
			{
				var func = temp as MethodCallExpression;
				if (func.Method.Name != "IncludeEFU" && func.Method.Name != "Include")
				{
					modifiers.Add(func);
				}
				temp = func.Arguments[0];
			}
			modifiers.Reverse(); //We parse in reverse order so undo that
			return modifiers;
		}

		private IIncludeContainer<T> GetIncludeContainer(Expression expression)
		{
			var temp = expression;
			while (temp is MethodCallExpression)
			{
				temp = (temp as MethodCallExpression).Arguments[0];
			}

			return (temp as ConstantExpression)?.Value as IIncludeContainer<T>;
		}

		#region Visitors
		protected override Expression VisitConstant(ConstantExpression c)
		{
			// fix up the Expression tree to work with EF again
			if (c.Type == typeof(EFUQueryable<T>))
			{
				return Source.Expression;
			}
			return base.VisitConstant(c);
		}
		#endregion
	}
}
