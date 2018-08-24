using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CETextBoxControl
{
    public class CEList<T> : IEnumerable<T>, IList<T>
    {
        public const int MAX_COUNT = 10000000;
        private T[] array = new T[0];

        public CEList()
        {
        }

        public T this[int index]
        {
            get
            {
                if (array == null)
                {
                    throw new InvalidOperationException(nameof(array) + " == null");
                }
                if (index < 0 || array.Length <= index)
                {
                    throw new ArgumentException(nameof(index) + " < 0 || " + nameof(array) + ".Length <= " + nameof(index));
                }
                return array[index];
            }

            set
            {
                if (array == null)
                {
                    throw new InvalidOperationException(nameof(array) + " == null");
                }
                if (index < 0 || array.Length <= index)
                {
                    throw new ArgumentNullException(nameof(index) + " < 0 || " + nameof(array) + ".Length <= " + nameof(index));
                }
                array[index] = value;
            }
        }

        public int Count
        {
            get
            {
                if (array == null)
                {
                    throw new InvalidOperationException(nameof(array) + " == null");
                }
                return array.Length;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(T item)
        {
            if (array == null)
            {
                throw new InvalidOperationException(nameof(array) + " == null");
            }
            if (MAX_COUNT <= array.Length)
            {
                throw new ArgumentException(nameof(MAX_COUNT) + " <= " + nameof(array) + ".Length");
            }
            T[] temporary = new T[array.Length + 1];
            array.CopyTo(temporary, 0);
            temporary[temporary.Length - 1] = item;
            array = temporary;
        }

        public void Clear()
        {
            array = new T[0];
        }

        public bool Contains(T item)
        {
            if (array == null)
            {
                throw new InvalidOperationException(nameof(array) + " == null");
            }
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item) + " == null");
            }
            return Array.IndexOf(array, item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (this.array == null)
            {
                throw new InvalidOperationException("this." + nameof(array) + " == null");
            }
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array) + " == null");
            }
            if (arrayIndex < 0 || array.Length <= arrayIndex)
            {
                throw new ArgumentException(nameof(arrayIndex) + " < 0 || " + nameof(array) + ".Length <= " + nameof(arrayIndex));
            }
            this.array.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in array)
            {
                yield return item;
            }
        }

        public int IndexOf(T item)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array) + " == null");
            }
            return Array.IndexOf(array, item);
        }

        public void Insert(int index, T item)
        {
            if (array == null)
            {
                throw new InvalidOperationException(nameof(array) + " == null");
            }
            if (index < 0 || array.Length < index)
            {
                throw new ArgumentException(nameof(index) + " < 0 || " + nameof(array) + ".Length <= " + nameof(index));
            }
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item) + " == null");
            }
            if (MAX_COUNT <= array.Length + 1)
            {
                throw new ArgumentException(nameof(MAX_COUNT) + " <= " + nameof(array) + ".Length");
            }
            T[] temporary = new T[array.Length + 1];
            Array.Copy(array, 0, temporary, 0, index);
            Array.Copy(array, index, temporary, index + 1, array.Length - index);
            temporary[index] = item;
            array = temporary;
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            if (array == null)
            {
                throw new InvalidOperationException(nameof(array) + " == null");
            }
            if (index < 0 || array.Length <= index)
            {
                throw new ArgumentException(nameof(index) + " < 0 || " + nameof(array) + ".Length <= " + nameof(index));
            }
            T[] temporary = new T[array.Length - 1];
            Array.Copy(array, 0, temporary, 0, index);
            Array.Copy(array, index + 1, temporary, index, array.Length - index - 1);
            array = temporary;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return 1;
            yield return 2;
            yield return 3;
            yield return 4;
            //return GetEnumerator();
        }
    }
}