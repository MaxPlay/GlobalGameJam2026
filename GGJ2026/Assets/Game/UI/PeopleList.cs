using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PeopleList : MonoBehaviour
{
    private readonly List<PeopleListEntry> entries = new();

    public bool IsVisible { get; set; }

    [SerializeField]
    private PeopleListEntry entryPrefab;

    [SerializeField]
    private RectTransform container;

    public void Populate(IReadOnlyList<Person> people)
    {
        while (entries.Count < people.Count)
        {
            PeopleListEntry entry = Instantiate(entryPrefab, container);
            entries.Add(entry);
        }

        while (entries.Count > people.Count)
        {
            PeopleListEntry entry = entries[^1];
            entries.RemoveAt(entries.Count - 1);
            Destroy(entry);
        }

        int index = 0;
        foreach (Person person in people.OrderBy(p => p.DisplayName))
        {
            entries[index].SetPerson(person);
            index++;
        }
    }

    public void Refresh()
    {
        foreach (PeopleListEntry entry in entries)
        {
            entry.Refresh();
        }
    }
}
