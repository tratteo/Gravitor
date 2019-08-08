public class SaveObject
{
    private object data;

    public SaveObject(object data)
    {
        this.data = data;
    }

    public T GetData<T>()
    {
        return (T)data;
    }
}
