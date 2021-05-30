using System;
using System.Collections.Generic;
using System.Text;

namespace MultitaskQueue
{
    public enum State
    {
        NotStarted,
        Running,
    }
    public enum ObservableAction
    {
        Add,
        Update,
        Remove
    }
}
