using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UndoProject
{
    public class UndoStack
    {
        private readonly Stack<UndoCommand> _undoStack = new Stack<UndoCommand>();
        private readonly Stack<UndoCommand> _redoStack = new Stack<UndoCommand>();

        public static UndoStack Instance { get; } = new UndoStack();

        public void Push(UndoCommand command)
        {
            this._redoStack.Clear();
            this._undoStack.Push(command);
            command.RedoAction();
        }

        public bool CanUndo() => this._undoStack.Any();
        public bool CanRedo() => this._redoStack.Any();

        public void Undo()
        {
            if (!this.CanUndo()) return;
            var command = this._undoStack.Pop();
            command.UndoAction();
            this._redoStack.Push(command);
        }

        public void Redo()
        {
            if (!this.CanRedo()) return;
            var command = this._redoStack.Pop();
            command.RedoAction();
            this._undoStack.Push(command);
        }
    }
}
