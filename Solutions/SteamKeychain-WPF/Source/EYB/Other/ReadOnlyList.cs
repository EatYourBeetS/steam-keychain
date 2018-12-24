using System;
using System.Collections;
using System.Collections.Generic;

namespace EYB
{
    public sealed class ReadOnlyList<T> : IEnumerable<T>
    {
        private List<T> _list;

        public int Count { get { return _list.Count; } }
        public T this[int index] { get { return _list[index]; } set { _list[index] = value; } }

        public ReadOnlyList(Func<List<T>> getList)
        {
            _list = getList();
        }

        public ReadOnlyList(ref List<T> list)
        {
            _list = list;
        }

        public ReadOnlyList(IEnumerable<T> items)
        {
            _list = new List<T>(items);
        }

        public ReadOnlyList()
        {
            _list = new List<T>();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}