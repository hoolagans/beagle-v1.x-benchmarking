using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Supermodel.DataAnnotations.Exceptions;

namespace Supermodel.DataAnnotations.Validations;

public class ValidationResultList : List<ValidationResult>
{
    #region Methods
    public virtual ValidationResult AddValidationResult(ValidationResult vr)
    {
        //We only add non-duplicate errors
        if (this.All(x => !AsyncValidator.AreEqual(x, vr))) Add(vr);
        return vr;
    }
    public virtual ValidationResult AddValidationResult<TModel>(TModel model, string message, params Expression<Func<TModel, object?>>[] expressions)
    {
        var vr = CreateValidationResult(model, message, expressions);
        return AddValidationResult(vr);
    }
    public virtual ValidationResultList AddValidationResultList(ValidationResultList vrl, string prefix = "")
    {
        if (!string.IsNullOrEmpty(prefix)) prefix += ".";
        foreach (var vr in vrl) 
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                var memberNames = vr.MemberNames.ToArray();
                for(var i = 0; i < memberNames.Length; i++) memberNames[i] = $"{prefix}{memberNames[i]}";
                    
                AddValidationResult(new ValidationResult(vr.ErrorMessage, memberNames));
            }
            else
            {
                AddValidationResult(vr);
            }
        }
        return this;
    }
    public virtual List<string> GetAllErrorsFor(string memberName)
    {
        var errors = new List<string>();
        foreach (var vr in this)
        {
            if (vr.MemberNames.Contains(memberName)) errors.Add(vr.ErrorMessage!);
        }
        return errors;
    }
    #endregion

    #region Properties
    public bool IsValid => Count == 0;
    #endregion

    #region Private Helper Methods
    protected virtual ValidationResult CreateValidationResult<TModel>(TModel model, string message, params Expression<Func<TModel, object?>>[] expressions)
    {
        var propertyNames = new List<string>();
        foreach (var expression in expressions)
        {
            propertyNames.Add(GetPropertyName(model, expression));
        }
        return new ValidationResult(message, propertyNames);
    }
    //public static string GetPropertyName<TModel>(TModel model, Expression<Func<TModel, object?>> expression)
    //{
    //    //if the value needs to be boxed
    //    MemberExpression memberExpression;
    //    if (expression.Body.NodeType == ExpressionType.Convert)
    //    {
    //        memberExpression = (MemberExpression)((UnaryExpression)expression.Body).Operand;
    //    }
    //    else
    //    {
    //        if (expression.Body.NodeType != ExpressionType.MemberAccess) throw new ArgumentException("Expression must describe a property", nameof(expression));
    //        memberExpression = (MemberExpression)expression.Body;
    //    }
    //    var propertyName = memberExpression.Member is PropertyInfo ? memberExpression.Member.Name : null;
    //    return GetExpressionName(memberExpression.Expression) + propertyName;
    //}
        
    // ReSharper disable once UnusedParameter.Local
    protected static string GetPropertyName<TModel>(TModel model, Expression<Func<TModel, object?>> expression)
    {
        if (expression.Body.NodeType == ExpressionType.Convert)
        {
            var body = (MemberExpression)((UnaryExpression)expression.Body).Operand;
            var propertyName = body.Member is PropertyInfo ? body.Member.Name : "";
            if (body.Expression.NodeType == ExpressionType.Parameter) return propertyName;
            return $"{GetExpressionName(body.Expression)}.{propertyName}";
        }
        else if (expression.Body.NodeType == ExpressionType.MemberAccess)
        {
            var body = (MemberExpression)expression.Body;
            var propertyName = body.Member is PropertyInfo ? body.Member.Name : "";
            if (body.Expression.NodeType == ExpressionType.Parameter) return propertyName;
            return $"{GetExpressionName(body.Expression)}.{propertyName}";
        }
        else if (expression.Body.NodeType == ExpressionType.Call)
        {
            var body = (MethodCallExpression)expression.Body;
            if (body.Method.Name != "get_Item") throw new ArgumentException("Expression must describe a property or an indexer", nameof(expression));
            return GetExpressionName(body);
        }
        else if (expression.Body.NodeType == ExpressionType.ArrayIndex)
        {
            var body = (BinaryExpression)expression.Body;
            return GetExpressionName(body);
        }
        else
        {
            throw new ArgumentException("Expression must describe a property or an indexer", nameof(expression));
        }
    }
    protected static string GetExpressionName(Expression expression)
    {
        if (expression.NodeType == ExpressionType.Parameter) return "";

        if (expression.NodeType == ExpressionType.MemberAccess)
        {
            var memberExpression = (MemberExpression)expression;
            return $"{GetExpressionName(memberExpression.Expression)}{memberExpression.Member.Name}";
        }

        if (expression.NodeType == ExpressionType.Call)
        {
            var methodCallExpression = (MethodCallExpression)expression;
            var indexExpression = methodCallExpression.Arguments[0];
            var indexExpressionResult = Expression.Lambda(indexExpression).Compile().DynamicInvoke();
            if (methodCallExpression.Object == null) throw new SupermodelException("methodCallExpression.Object == null: this should never happen");
            return $"{GetExpressionName(methodCallExpression.Object)}[{indexExpressionResult}]";
        }

        if (expression.NodeType == ExpressionType.ArrayIndex)
        {
            var binaryExpression = (BinaryExpression)expression;
            var indexExpression = binaryExpression.Right;
            var indexExpressionResult = Expression.Lambda(indexExpression).Compile().DynamicInvoke();
            return $"{GetExpressionName(binaryExpression.Left)}[{indexExpressionResult}]";
        }

        throw new Exception("Invalid Expression '" + expression + "'");
    }
    #endregion
}