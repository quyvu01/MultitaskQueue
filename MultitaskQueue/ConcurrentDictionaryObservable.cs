using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MultitaskQueue
{
    public class ConcurrentDictionaryObservable<T, K>: ConcurrentDictionary<T, K>
    {
        public event Func<object, ObservableAction, Task> OnChange;
        public new bool TryAdd(T key, K value)
        {
            var isAdded = base.TryAdd(key, value);
            OnChange?.Invoke(this, ObservableAction.Add);
            return isAdded;
        }
        public new bool TryRemove(T key, out K value)
        {
            var isRemoved = base.TryRemove(key, out value);
            OnChange?.Invoke(this, ObservableAction.Remove);
            return isRemoved;
        }
        public new bool TryUpdate(T key, K newValue, K comparationValue)
        {
            var isUpdated = base.TryUpdate(key, newValue, comparationValue);
            OnChange?.Invoke(this, ObservableAction.Update);
            return isUpdated;
        }
    }
}
