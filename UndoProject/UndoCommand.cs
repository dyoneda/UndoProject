using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UndoProject
{
    public class UndoCommand
    {
        public UndoCommand(Action undo, Action redo)
        {
            this.UndoAction = undo;
            this.RedoAction = redo;
        }

        public Action UndoAction { get; }
        public Action RedoAction { get; }

        public static UndoCommand CreateCommand<TObj, TProp>(TObj obj, Expression<Func<TObj, TProp>> exp, TProp newValue)
        {
            var propName = ((MemberExpression)exp.Body).Member.Name;
            var getter = GetGetter<TObj, TProp>(propName);
            var setter = GetSetter<TObj, TProp>(propName);
            var oldValue = getter(obj);
            return new UndoCommand(() => setter(obj, oldValue), () => setter(obj, newValue));
        }

        private static Func<TObj, TProp> GetGetter<TObj, TProp>(string propName)
            => (Func<TObj, TProp>)Delegate.CreateDelegate(typeof(Func<TObj, TProp>),
                    typeof(TObj).GetProperty(propName).GetGetMethod());

        private static Action<TObj, TProp> GetSetter<TObj, TProp>(string propName)
            => (Action<TObj, TProp>)Delegate.CreateDelegate(typeof(Action<TObj, TProp>),
                    typeof(TObj).GetProperty(propName).GetSetMethod());
    }
}
