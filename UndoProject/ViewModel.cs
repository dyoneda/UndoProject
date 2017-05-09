using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace UndoProject
{
    public class ViewModel : IDisposable
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        void IDisposable.Dispose() => this._disposable.Dispose();

        private BehaviorSubject<bool> _subjects = new BehaviorSubject<bool>(true);

        public ReactiveProperty<string> StringProperty { get; }
        public ReactiveProperty<int> IntProperty { get; }
        public ReactiveProperty<bool> BoolProperty { get; }

        public ReactiveCommand Undo { get; }
        public ReactiveCommand Redo { get; }

        public ViewModel()
        {
            var m = new Model();
            this.StringProperty = CreateReactiveProperty(m, o => o.StringValue);
            this.IntProperty = CreateReactiveProperty(m, o => o.IntValue);
            this.BoolProperty = CreateReactiveProperty(m, o => o.BoolValue);

            this.Undo = this._subjects.Select(_ => UndoStack.Instance.CanUndo()).ToReactiveCommand();
            this.Undo.Subscribe(_ => 
            {
                UndoStack.Instance.Undo();
                this._subjects.OnNext(true);
            }).AddTo(this._disposable);

            this.Redo = this._subjects.Select(_ => UndoStack.Instance.CanRedo()).ToReactiveCommand();
            this.Redo.Subscribe(_ => {
                UndoStack.Instance.Redo();
                this._subjects.OnNext(true);
            }).AddTo(this._disposable);
        }

        private ReactiveProperty<TProp> CreateReactiveProperty<TObj, TProp>(TObj obj, Expression<Func<TObj, TProp>> exp) 
            where TObj : INotifyPropertyChanged
        {
            var latest = default(TProp);
            var rp = obj.ObserveProperty(exp).Do(v => latest = v).ToReactiveProperty().AddTo(this._disposable);
            rp.Where(v => !object.Equals(v, latest)).Subscribe(v =>
            {
                UndoStack.Instance.Push(UndoCommand.CreateCommand(obj, exp, v));
                this._subjects.OnNext(true);
            }).AddTo(this._disposable);
            return rp;
        }
    }
}
