using TriInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Knifest.DebugTools
{
    [DeclareTabGroup(Tabs)]
    [DeclareBoxGroup(Tabs_Events, Title = "Events")]
    public abstract class BaseDebugField : MonoBehaviour
    {
        protected const string Tabs = "Tabs";
        protected const string Tab_User = "User";
        protected const string Tab_Dev = "Dev";
        protected const string Tabs_Events = Tabs + "/Events";

        [field: Group(Tabs), Tab(Tab_User), SerializeField]
        protected string Label { get; private set; }

        [FormerlySerializedAs("_labelUI")] [Group(Tabs), Tab(Tab_Dev)] [SerializeField]
        protected TMPro.TMP_Text labelUI;
    }
}