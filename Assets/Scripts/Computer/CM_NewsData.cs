using UnityEngine;

[CreateAssetMenu(menuName = "Cheap Motel/News Item")]
public class CM_NewsItem : ScriptableObject
{
    public string title;
    [TextArea] public string content;
    public string topic;
}

[CreateAssetMenu(menuName = "Cheap Motel/News Collection")]
public class CM_NewsCollection : ScriptableObject
{
    public CM_NewsItem[] newsItems;
}
