public class RollingQueue<T>
{
    private int capacity;
    private int head;
    private int tail;
    private T[] buffer;

    public RollingQueue(int capacity)
    {
        this.capacity = capacity;
        this.buffer = new T[capacity];
        head = -1;
        tail = 0;
    }

    public void Clear()
    {
        head = -1;
        tail = 0;
    }

    public void Enqueue(T item)
    {
        head++;
        head %= capacity;
        if (buffer.Length == capacity)
        {
            buffer[head] = item;
            tail++;
            tail %= capacity;
        }
        else
        {
            buffer[head] = item;
        }
    }

    private bool CheckCount(int count)
    {
        return count < 0 || head == -1;
    }

    public T GetNewest()
    {
        if (head == -1) return default (T);
        return buffer[head];
    }

    public T GetNewest(int count)
    {
        if (CheckCount(count)) return default(T);
        int size = buffer.Length;
        count %= size;
        return buffer[(size - count + head) % size];
    }

    public T GetOldest()
    {
        if (head == -1) return default (T);
        return buffer[tail];
    }

    public T GetOldest(int count)
    {
        if (CheckCount(count)) return default (T);
        return buffer[(tail + count) % buffer.Length];
    }

    public T[] GetArray()
    {
        return buffer;
    }
};
