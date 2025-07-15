using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
#if NetVersion35
using System.Linq;
using System.Linq.Expressions;
#endif

namespace Azuro.Common
{
    public delegate void PropertySet( object instance, object value );
    public delegate object PropertyGet( object instance );
    public delegate void CallMethodNoResult( object instance, params object[] parameters );
    public delegate object CallMethodWithResult( object instance, params object[] parameters );

    public enum LateBoundType { Reflection, ExpressionTrees };

    public class LateBound
    {
        private readonly LateBoundType m_lateBoundType;

        public LateBound()
            : this( LateBoundType.ExpressionTrees )
        {
        }

        public LateBound( LateBoundType lateBoundType )
        {
#if NetVersion35            
            m_lateBoundType = lateBoundType;
#else
            m_lateBoundType = LateBoundType.Reflection;
#endif
        }

        public CallMethodNoResult CreateMethodCallNoResult( MethodInfo mi )
        {
#if NetVersion35
            if( m_lateBoundType == LateBoundType.ExpressionTrees )
            {
                return CreateMethodCallNoResultExpressionTree( mi );
            }
#endif
            return CreateMethodCallNoResultReflection( mi );
        }

        public CallMethodWithResult CreateMethodCallWithResult( MethodInfo mi )
        {
#if NetVersion35
            if( m_lateBoundType == LateBoundType.ExpressionTrees )
            {
                return CreateMethodCallWithResultExpressionTree( mi );
            }
#endif
            return CreateMethodCallWithResultReflection( mi );
        }

        public PropertyGet CreatePropertyGetter( string propertyName, Type instanceType )
        {
#if NetVersion35
            if( m_lateBoundType == LateBoundType.ExpressionTrees )
            {
                return CreatePropertyGetterExpressionTree( propertyName, instanceType );
            }
#endif
            return CreatePropertyGetterReflection( propertyName, instanceType );
        }

        public PropertySet CreatePropertySetter( string propertyName, Type instanceType )
        {
#if NetVersion35
            if( m_lateBoundType == LateBoundType.ExpressionTrees )
            {
                return CreatePropertySetterExpressionTree( propertyName, instanceType );
            }
#endif
            return CreatePropertySetterReflection( propertyName, instanceType );
        }


#if NetVersion35
        public CallMethodWithResult CreateMethodCallWithResultExpressionTree( MethodInfo method )
        {
            var instanceParameter = Expression.Parameter( typeof( object ), "target" );
            var argsParameter = Expression.Parameter( typeof( object[] ), "arguments" );

            var call = Expression.Call(
                Expression.Convert( instanceParameter, method.DeclaringType ),
                method,
                CreateParameterExpressionExpressionTree( method, argsParameter ) );

            var lambda = Expression.Lambda<CallMethodWithResult>(
                Expression.Convert( call, typeof( object ) ),
                instanceParameter,
                argsParameter );

            return lambda.Compile();
        }

        public CallMethodNoResult CreateMethodCallNoResultExpressionTree( MethodInfo method )
        {
            var instanceParameter = Expression.Parameter( typeof( object ), "target" );
            var argsParameter = Expression.Parameter( typeof( object[] ), "arguments" );

            var call = Expression.Call(
                Expression.Convert( instanceParameter, method.DeclaringType ),
                method,
                CreateParameterExpressionExpressionTree( method, argsParameter ) );

            var lambda = Expression.Lambda<CallMethodNoResult>(
                call,
                instanceParameter,
                argsParameter );

            return lambda.Compile();
        }


        private Expression[] CreateParameterExpressionExpressionTree( MethodInfo method, ParameterExpression argsParameter )
        {
            return method.GetParameters().Select(
                ( param, index ) => Expression.Convert(
                    Expression.ArrayIndex( argsParameter, Expression.Constant( index ) ),
                    param.ParameterType ) ).ToArray();
        }

        public PropertySet CreatePropertySetterExpressionTree( string propertyName, Type instanceType )
        {
            var setterPropInfo = instanceType.GetProperty( propertyName );
            var setterMethodInfo = setterPropInfo.GetSetMethod();

            var instanceParam = Expression.Parameter( typeof( object ), "instance" );
            var instanceParamTyped = Expression.Convert( instanceParam, instanceType );

            var valueParam = Expression.Parameter( typeof( object ), propertyName );
            var valueParamTyped = Expression.Convert( valueParam, setterPropInfo.PropertyType );

            var setterCall = Expression.Call( instanceParamTyped, setterMethodInfo, valueParamTyped );
            var setPropExpr = Expression.Lambda<PropertySet>( setterCall, instanceParam, valueParam );

            return setPropExpr.Compile();
        }

        public PropertyGet CreatePropertyGetterExpressionTree( string propertyName, Type instanceType )
        {
            var instanceParam = Expression.Parameter( typeof( object ), "instance" );
            var instanceParamTyped = Expression.Convert( instanceParam, instanceType );

            var memberAccess = Expression.MakeMemberAccess( instanceParamTyped, instanceType.GetProperty( propertyName ) );
            var memberAccessTyped = Expression.Convert( memberAccess, typeof( object ) );

            var lambda = Expression.Lambda<PropertyGet>( memberAccessTyped, instanceParam );

            return lambda.Compile();
        }
#endif

        public CallMethodNoResult CreateMethodCallNoResultReflection( MethodInfo mi )
        {
            CallMethodNoResult call = ( instance, prms ) => mi.Invoke( instance, prms );
            return call;
        }

        public CallMethodWithResult CreateMethodCallWithResultReflection( MethodInfo mi )
        {
                return mi.Invoke;
        }

        public PropertyGet CreatePropertyGetterReflection( string propertyName, Type instanceType )
        {
                var getterPropInfo = instanceType.GetProperty( propertyName );
                PropertyGet get = instance => getterPropInfo.GetValue( instance, null );
                return get;
        }

        public PropertySet CreatePropertySetterReflection( string propertyName, Type instanceType )
        {
                var setterPropInfo = instanceType.GetProperty( propertyName );
                PropertySet set = ( instance, value ) => setterPropInfo.SetValue( instance, value, null );
                return set;
        }
    }
}
