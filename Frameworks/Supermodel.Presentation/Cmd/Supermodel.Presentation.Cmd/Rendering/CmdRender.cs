using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Supermodel.DataAnnotations.Attributes;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.DataAnnotations.Expressions;
using Supermodel.Presentation.Cmd.ConsoleOutput;
using Supermodel.Presentation.Cmd.Models;
using Supermodel.Presentation.Cmd.Models.Interfaces;

namespace Supermodel.Presentation.Cmd.Rendering;

public static class CmdRender
{
    #region Helper Class
    public static class Helper
    {
        #region Write with Color
        public static void Write(string str, FBColors? colors)
        {
            colors?.SetColors();
            Console.Write(str);
        }
        public static void WriteLine(string str, FBColors? colors)
        {
            colors?.SetColors();
            Console.WriteLine(str);
        }
        #endregion
            
        #region Parsing Expressions for Routing
        // ReSharper disable once UnusedParameter.Local
        public static string GetPropertyName<TModel, TValue>(TModel model, Expression<Func<TModel, TValue>> expression)
        {
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                var body = (MemberExpression)((UnaryExpression)expression.Body).Operand;
                var propertyName = body.Member is PropertyInfo ? body.Member.Name : "";
                if (body.Expression!.NodeType == ExpressionType.Parameter) return propertyName;
                return $"{GetExpressionName(body.Expression)}.{propertyName}";
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                var body = (MemberExpression)expression.Body;
                var propertyName = body.Member is PropertyInfo ? body.Member.Name : "";
                if (body.Expression!.NodeType == ExpressionType.Parameter) return propertyName;
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
            else if (expression.Body.NodeType == ExpressionType.Parameter)
            {
                var body = (ParameterExpression)expression.Body;
                return GetExpressionName(body);
            }                
            else
            {
                throw new ArgumentException("Expression must describe a property or an indexer", nameof(expression));
            }
        }
        private static string GetExpressionName(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Parameter) return "";

            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpression = (MemberExpression)expression;
                return $"{GetExpressionName(memberExpression.Expression!)}{memberExpression.Member.Name}";
            }

            if (expression.NodeType == ExpressionType.Call)
            {
                var methodCallExpression = (MethodCallExpression)expression;
                var indexExpression = methodCallExpression.Arguments[0];
                var indexExpressionResult = Expression.Lambda(indexExpression).Compile().DynamicInvoke();
                return $"{GetExpressionName(methodCallExpression.Object!)}[{indexExpressionResult}]";
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
    #endregion

    #region Render Label Methods
    public static void ShowLabelFor<TModel, TValue>(TModel model, Expression<Func<TModel, TValue>> propertyExpression, string? label = null, FBColors? colors = null)
    {
        var propertyName = Helper.GetPropertyName(model, propertyExpression);
        ShowLabel(model, propertyName, label, colors);
    }
    public static void ShowLabel<TModel>(TModel model, string expression, string? label = null, FBColors? colors = null)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        label ??= model.GetType().GetDisplayNameForProperty(expression);
        colors?.SetColors();
        Console.Write(label);
    }
    #endregion

    #region Render Validation Methods
    public static void ShowValidationSummary<TModel>(TModel model, FBColors? validationErrorColors, FBColors? invalidFieldLabelColors, FBColors? numbersColors)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
            
        var vrl = CmdContext.ValidationResultList;
        if (vrl.IsValid) return;

        for (var i = 0; i < vrl.Count; i++)
        {
            var vr = vrl[i];

            var first = true;
            foreach (var memberName in vr.MemberNames)
            {
                string fieldName;
                try
                {
                    fieldName = model.GetType().GetDisplayNameForProperty(memberName);
                }
                catch (Exception)
                {
                    fieldName = memberName;
                }

                numbersColors?.SetColors();
                Console.Write($"{i + 1}) ");

                if (first) 
                {
                    invalidFieldLabelColors?.SetColors();
                    Console.Write(fieldName);
                    first = false;
                }
                else
                {
                    validationErrorColors?.SetColors();
                    Console.Write(", ");
                    invalidFieldLabelColors?.SetColors();
                    Console.Write(fieldName);

                }
            }
            invalidFieldLabelColors?.SetColors();
            Console.Write(": ");
            validationErrorColors?.SetColors();
            Console.Write(vr.ErrorMessage);
            numbersColors?.SetColors();
            Console.WriteLine(";");
        }
    }
    public static void ShowValidationMessageFor<TModel, TValue>(TModel model, Expression<Func<TModel, TValue>> propertyExpression, FBColors? colors = null)
    {
        var propertyName = Helper.GetPropertyName(model, propertyExpression);
        ShowValidationMessage(model, propertyName, colors);
    }
    public static void ShowValidationMessage<TModel>(TModel model, string expression, FBColors? colors = null)
    {
        var vrl = CmdContext.ValidationResultList;
            
        if (vrl.IsValid) return;

        if (model == null) throw new ArgumentNullException(nameof(model));

        var name = expression;

        var errors = vrl.GetAllErrorsFor(name);
        if (!errors.Any()) return;

        colors?.SetColors();
        Console.Write(errors.First());
    }
    #endregion

    #region Render Editor Methods
    #nullable disable
    public static TModel EditForModel<TModel>(TModel model, FBColors? colors = null, FBColors? invalidValueColors = null, FBColors? promptColors = null)
    {
        return (TModel)Edit(model, "", colors, invalidValueColors, promptColors);
    }
    public static TValue EditFor<TModel, TValue>(TModel model, Expression<Func<TModel, TValue>> propertyExpression, FBColors? colors = null, FBColors? invalidValueColors = null, FBColors? promptColors = null)
    {
        var propertyName = Helper.GetPropertyName(model, propertyExpression);
        return (TValue)Edit(model, propertyName, colors, invalidValueColors, promptColors);
    }
    #nullable enable
    public static object? Edit<TModel>(TModel model, string expression, FBColors? colors = null, FBColors? invalidValueColors = null, FBColors? promptColors = null)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));

        colors?.SetColors();

        var (_, propertyType, propertyValue) = model.GetPropertyInfoPropertyTypeAndValueByFullName(expression);
        if (typeof(ICmdEditor).IsAssignableFrom(propertyType))
        {
            propertyValue ??= Activator.CreateInstance(propertyType);

            if (propertyValue is ICmdEditor editor) return editor.Edit();
            else throw new SupermodelException("This should never happen: propertyValue is not ICmdEditor");
        }

        //ICmdOutput
        if (typeof(ICmdOutput).IsAssignableFrom(propertyType))
        {
            var iCmdOutput = (ICmdOutput)(propertyValue ?? StringWithColor.Empty);
            iCmdOutput.WriteToConsole();
            return propertyValue;
        }

        //strings
        if (typeof(string).IsAssignableFrom(propertyType)) return ConsoleExt.EditString(propertyValue?.ToString() ?? "");

        //integer types
        if (typeof(int).IsAssignableFrom(propertyType) || typeof(int?).IsAssignableFrom(propertyType)) return ConsoleExt.EditInteger((int?)propertyValue);
        if (typeof(uint).IsAssignableFrom(propertyType) || typeof(uint?).IsAssignableFrom(propertyType)) return ConsoleExt.EditInteger((uint?)propertyValue);
        if (typeof(long).IsAssignableFrom(propertyType) || typeof(long?).IsAssignableFrom(propertyType)) return ConsoleExt.EditInteger((long?)propertyValue);
        if (typeof(ulong).IsAssignableFrom(propertyType) || typeof(ulong?).IsAssignableFrom(propertyType)) return ConsoleExt.EditInteger((ulong?)propertyValue);
        if (typeof(short).IsAssignableFrom(propertyType) || typeof(short?).IsAssignableFrom(propertyType)) return ConsoleExt.EditInteger((short?)propertyValue);
        if (typeof(ushort).IsAssignableFrom(propertyType) || typeof(ushort?).IsAssignableFrom(propertyType)) return ConsoleExt.EditInteger((ushort?)propertyValue);
        if (typeof(byte).IsAssignableFrom(propertyType) || typeof(byte?).IsAssignableFrom(propertyType)) return ConsoleExt.EditInteger((byte?)propertyValue);
        if (typeof(sbyte).IsAssignableFrom(propertyType) || typeof(sbyte?).IsAssignableFrom(propertyType)) return ConsoleExt.EditInteger((sbyte?)propertyValue);

        //floating point types
        if (typeof(double).IsAssignableFrom(propertyType) || typeof(double?).IsAssignableFrom(propertyType)) return ConsoleExt.EditFloat((double?)propertyValue);
        if (typeof(float).IsAssignableFrom(propertyType) || typeof(float?).IsAssignableFrom(propertyType)) return ConsoleExt.EditFloat((float?)propertyValue);
        if (typeof(decimal).IsAssignableFrom(propertyType) || typeof(decimal?).IsAssignableFrom(propertyType)) return ConsoleExt.EditFloat((decimal?)propertyValue);

        //booleans
        if (typeof(bool).IsAssignableFrom(propertyType) || typeof(bool?).IsAssignableFrom(propertyType)) return ConsoleExt.EditBool((bool?)propertyValue);

        //DateTime
        if (typeof(DateTime).IsAssignableFrom(propertyType) || typeof(DateTime?).IsAssignableFrom(propertyType)) return ConsoleExt.EditDate((DateTime?)propertyValue, StringWithColor.Empty);

        //enums
        if (typeof(Enum).IsAssignableFrom(propertyType) || Nullable.GetUnderlyingType(propertyType)?.IsEnum == true)
        {
            Helper.Write(propertyValue?.ToString() ?? "", colors);
            return propertyValue;
        }

        //Guid
        if (typeof(Guid).IsAssignableFrom(propertyType))
        {
            Helper.Write(propertyValue?.ToString() ?? "", colors);
            return propertyValue;
        }

        //catch-all for primitive types
        if (propertyType.IsPrimitive) 
        {
            Helper.Write(propertyValue?.ToString() ?? "", colors);
            return propertyValue;
        }

        //catch-all for complex types - do nothing
        return propertyValue;
    }
    #endregion

    #region Render Display Methods
    public static void DisplayForModel<TModel>(TModel model, FBColors? colors = null)
    {
        Display(model, "", colors);
    }
    public static void DisplayFor<TModel, TValue>(TModel model, Expression<Func<TModel, TValue>> propertyExpression, FBColors? colors = null)
    {
        var propertyName = Helper.GetPropertyName(model, propertyExpression);
        Display(model, propertyName, colors);
    }
    public static void Display<TModel>(TModel model, string expression, FBColors? colors = null)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));

        colors?.SetColors();

        var (_, propertyType, propertyValue) = model.GetPropertyInfoPropertyTypeAndValueByFullName(expression);
        if (typeof(ICmdDisplayer).IsAssignableFrom(propertyType))
        {
            propertyValue ??= Activator.CreateInstance(propertyType);

            if (propertyValue is ICmdDisplayer displayer) displayer.Display();
            else throw new SupermodelException("This should never happen: propertyValue is not ICmdDisplayer");
        }

        //ICmdOutput
        else if(typeof(ICmdOutput).IsAssignableFrom(propertyType))
        {
            var iCmdOutput = (ICmdOutput)(propertyValue ?? StringWithColor.Empty);
            iCmdOutput.WriteToConsole();
        }

        //strings
        else if (typeof(string).IsAssignableFrom(propertyType))
        {
            Helper.Write(propertyValue?.ToString() ?? "", colors);
        }
            
        //integer types
        else if (typeof(int).IsAssignableFrom(propertyType) ||
                 typeof(int?).IsAssignableFrom(propertyType) ||
                 typeof(uint).IsAssignableFrom(propertyType) ||
                 typeof(uint?).IsAssignableFrom(propertyType) ||
                 typeof(long).IsAssignableFrom(propertyType) ||
                 typeof(long?).IsAssignableFrom(propertyType) ||
                 typeof(ulong).IsAssignableFrom(propertyType) ||
                 typeof(ulong?).IsAssignableFrom(propertyType) ||
                 typeof(short).IsAssignableFrom(propertyType) ||
                 typeof(short?).IsAssignableFrom(propertyType) ||
                 typeof(ushort).IsAssignableFrom(propertyType) ||
                 typeof(ushort?).IsAssignableFrom(propertyType) ||
                 typeof(byte).IsAssignableFrom(propertyType) ||
                 typeof(byte?).IsAssignableFrom(propertyType) ||
                 typeof(sbyte).IsAssignableFrom(propertyType) ||
                 typeof(sbyte?).IsAssignableFrom(propertyType))
        {
            Helper.Write(propertyValue?.ToString() ?? "", colors);
        }
            
        //floating point types
        else if (typeof(double).IsAssignableFrom(propertyType) ||
                 typeof(double?).IsAssignableFrom(propertyType) ||
                 typeof(float).IsAssignableFrom(propertyType) ||
                 typeof(float?).IsAssignableFrom(propertyType) ||
                 typeof(decimal).IsAssignableFrom(propertyType) ||
                 typeof(decimal?).IsAssignableFrom(propertyType))
        {
            Helper.Write(propertyValue?.ToString() ?? "", colors);
        }

        //booleans
        else if (typeof(bool).IsAssignableFrom(propertyType) ||
                 typeof(bool?).IsAssignableFrom(propertyType))
        {
            var boolPropertyValue = (bool?)propertyValue;
            Helper.Write(boolPropertyValue == true ? "Yes" : "No" , colors);
        }

        //DateTime
        else if (typeof(DateTime).IsAssignableFrom(propertyType) ||
                 typeof(DateTime?).IsAssignableFrom(propertyType))
        {
            Helper.Write(propertyValue?.ToString() ?? "", colors);
        }

        //enums
        else if (typeof(Enum).IsAssignableFrom(propertyType) ||
                 Nullable.GetUnderlyingType(propertyType)?.IsEnum == true)
        {
            Helper.Write(propertyValue?.ToString() ?? "", colors);
        }

        //Guid
        else if (typeof(Guid).IsAssignableFrom(propertyType))
        {
            Helper.Write(propertyValue?.ToString() ?? "", colors);
        }

        //catch-all for primitive types
        else if (propertyType.IsPrimitive)
        {
            Helper.Write(propertyValue?.ToString() ?? "", colors);
        }

        //catch-all for complex types
        else
        {
            //do nothing
        }
    }
    #endregion
}