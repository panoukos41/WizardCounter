using Core;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace WizardCounter;

public sealed class WizardPlayerCollection : KeyedCollection<Uuid, WizardPlayer>
{
    public WizardPlayerCollection()
    {
    }

    [JsonConstructor]
    public WizardPlayerCollection(IEnumerable<WizardPlayer> players)
    {
        foreach (var player in players)
        {
            Add(player);
        }
    }

    protected override Uuid GetKeyForItem(WizardPlayer item)
    {
        return item.Id;
    }
}
