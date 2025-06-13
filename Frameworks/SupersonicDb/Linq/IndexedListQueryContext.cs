using System;
using System.Linq;
using System.Linq.Expressions;

namespace Supersonic.Linq;

internal class IndexedListQueryContext<TItem> where TItem : class
{
    #region Methods
    internal static object Execute(SupersonicList<TItem> supersonicList, Expression expression, bool isEnumerable)
    {
        //The expression must represent a query over the data source
        if (!(expression is MethodCallExpression methodCallExpression)) throw new InvalidProgramException("No query over the data source was specified");

        if (methodCallExpression.Arguments.Count == 1)
        {
            switch (methodCallExpression.Method.Name)
            {
                case "Count": return supersonicList.GetItems().Count();
                case "Any": return supersonicList.GetItems().Any();
                case "First": return supersonicList.GetItems().First();
                case "FirstOrDefault": return supersonicList.GetItems().FirstOrDefault();
                case "Single": return supersonicList.GetItems().Single();
                case "SingleOrDefault": return supersonicList.GetItems().SingleOrDefault();
                default: throw new InvalidProgramException($"Unsuppored query method {methodCallExpression.Method.Name}");
            }
        }
        else if(methodCallExpression.Arguments.Count == 2)
        {
            //Get the expression out
            var predicate = (LambdaExpression)((UnaryExpression)methodCallExpression.Arguments[1]).Operand;

            //Evaluate variables into constatnts
            predicate = (Expression<Func<TItem, bool>>)Evaluator.PartialEval(predicate);

            //Find matching index
            var success = supersonicList.TryFindIndexAndCondition(predicate.Body, out var index, out var condition);

            if (success)
            {
                //Use the index to get results
                switch (methodCallExpression.Method.Name)
                {
                    case "Where": return index.WhereWithIndex(condition);
                    case "Count": return index.CountWithIndex(condition);
                    case "Any": return index.AnyWithIndex(condition);
                    case "All": return index.AllWithIndex(condition);
                    case "First": return index.FirstWithIndex(condition);
                    case "FirstOrDefault": return index.FirstOrDefaultWithIndex(condition);
                    case "Single": return index.SingleWithIndex(condition);
                    case "SingleOrDefault": return index.SingleOrDefaultWithIndex(condition);
                    default: throw new InvalidProgramException($"Unsuppored query method {methodCallExpression.Method.Name}");
                }
            }
            else
            {
                //Use LINQ to Objects to get results
                var compiledPredicate = (Func<TItem, bool>)predicate.Compile();

                switch (methodCallExpression.Method.Name)
                {
                    case "Where": return supersonicList.GetItems().Where(compiledPredicate);
                    case "Count": return supersonicList.GetItems().Count(compiledPredicate);
                    case "Any": return supersonicList.GetItems().Any(compiledPredicate);
                    case "All":  return supersonicList.GetItems().All(compiledPredicate);
                    case "First": return supersonicList.GetItems().First(compiledPredicate);
                    case "FirstOrDefault": return supersonicList.GetItems().FirstOrDefault(compiledPredicate);
                    case "Single": return supersonicList.GetItems().Single(compiledPredicate);
                    case "SingleOrDefault": return supersonicList.GetItems().SingleOrDefault(compiledPredicate);
                    default: throw new InvalidProgramException($"Unsuppored query method {methodCallExpression.Method.Name}");
                }
            }
        }
        else
        {
            throw new InvalidProgramException("This should never happen: methodCallExpression.Arguments.Count > 1");
        }
    }
    #endregion
}