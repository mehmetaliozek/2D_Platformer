using System.Collections.Generic;
using System.Linq;

public class CoroutineTracker
{
    private List<bool> finished = new List<bool>();

    public int Register()
    {
        finished.Add(false);
        return finished.Count - 1;
    }

    public void Complete(int index)
    {
        if (index >= 0 && index < finished.Count)
            finished[index] = true;
    }

    public bool AllCompleted()
    {
        return finished.All(value => value);
    }
}
