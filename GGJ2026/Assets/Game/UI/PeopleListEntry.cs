using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.UI;

public class PeopleListEntry : MonoBehaviour
{
    private Person data;

    [SerializeField, BoxGroup("Prefabs")]
    private Sprite aliveIcon;

    [SerializeField, BoxGroup("Prefabs")]
    private Sprite unknownIcon;

    [SerializeField]
    private Text text;

    [SerializeField]
    private Image icon;

    [SerializeField]
    private GameObject deadBar;

    public void SetPerson(Person person)
    {
        data = person;
        text.text = data.DisplayName;

        Refresh();
    }

    public void Refresh()
    {
        if (data)
        {
            bool showIcon = false;
            bool showDeadBar = false;
            switch (data.State)
            {
                case Person.PersonState.Unchecked:
                    // Empty on intend
                    break;
                case Person.PersonState.Alive:
                    showIcon = true;
                    icon.sprite = aliveIcon;
                    break;
                case Person.PersonState.Unknown:
                    showIcon = true;
                    icon.sprite = unknownIcon;
                    break;
                case Person.PersonState.Dead:
                    showDeadBar = true;
                    break;
            }
            icon.enabled = showIcon;
            deadBar.SetActive(showDeadBar);
        }
    }
}
