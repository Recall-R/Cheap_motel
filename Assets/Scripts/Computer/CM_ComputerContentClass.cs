using UnityEngine;
using TMPro;

public class CM_ComputerContentClass : MonoBehaviour
{
    [SerializeField] private GameObject computerDialog;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text contentText;
    [SerializeField] private TMP_Text topicText;
    [SerializeField] private GameObject newsItemTemplate;
    [SerializeField] private RectTransform newsItemsParent;
    [SerializeField] private float verticalSpacing = 60f;
    [SerializeField] private int NumberOfNewsToDisplay = 1;

    [SerializeField] private CM_NewsCollection newsCollection;

    private void ClearExistingEntries()
    {
        if (newsItemsParent == null)
        {
            return;
        }

        for (int i = newsItemsParent.childCount - 1; i >= 0; i--)
        {
            Destroy(newsItemsParent.GetChild(i).gameObject);
        }
    }

    private void ApplyNewsDataToEntry(GameObject entry, CM_NewsItem newsItem)
    {
        if (entry == null || newsItem == null)
        {
            return;
        }

        TMP_Text[] textComponents = entry.GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text textComponent in textComponents)
        {
            string componentName = textComponent.name.ToLower();

            if (componentName.Contains("title"))
            {   
                textComponent.enabled = true;
                textComponent.text = newsItem.title;
            }
            else if (componentName.Contains("news"))
            {
                textComponent.enabled = true;
                textComponent.text = newsItem.content;
            }
            else if (componentName.Contains("topic"))
            {
                textComponent.enabled = true;
                textComponent.text = newsItem.topic;
            }
        }
    }

    private void ParseNewsData()
    {
        if (newsCollection == null)
        {
            return;
        }

        if (newsItemTemplate != null)
        {
            if (newsItemsParent == null)
            {
                newsItemsParent = computerDialog != null ? computerDialog.GetComponent<RectTransform>() : null;
            }

            if (newsItemsParent != null)
            {
                ClearExistingEntries();

                int totalToDisplay = Mathf.Min(NumberOfNewsToDisplay, newsCollection.newsItems.Length);
                for (int i = 0; i < totalToDisplay; i++)
                {
                    GameObject entry = Instantiate(newsItemTemplate, newsItemsParent, false);
                    entry.SetActive(true);

                    RectTransform entryRect = entry.GetComponent<RectTransform>();
                    if (entryRect != null)
                    {
                        entryRect.anchoredPosition = new Vector2(0f, -i * verticalSpacing);
                    }

                    ApplyNewsDataToEntry(entry, newsCollection.newsItems[i]);
                }
            }
        }
        else if (newsCollection.newsItems != null && newsCollection.newsItems.Length > 0)
        {
            CM_NewsItem firstNews = newsCollection.newsItems[0];
            titleText.text = firstNews.title;
            contentText.text = firstNews.content;
            topicText.text = firstNews.topic;
        }
    }

    private void Start()
    {
        ParseNewsData();
    }
}
