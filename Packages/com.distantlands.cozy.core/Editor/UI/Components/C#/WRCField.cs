using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using DistantLands.Cozy.Data;

namespace DistantLands.Cozy.EditorScripts
{

    public class WRCField : VisualElement
    {
        private Slider BaseValue => this.Q<Slider>();
        private Label Label => this.Q<Label>();
        private VisualElement CurvesContainer => this.Q<VisualElement>("curves-container");
        private VisualElement ToggleCurves => this.Q<VisualElement>("toggle-curves");
        bool open = false;

        public WRCField(SerializedProperty property)
        {

            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.distantlands.cozy.core/Editor/UI/Components/UXML/weighted-random-chance.uxml"
            );
            asset.CloneTree(this);

            BaseValue.BindProperty(property.FindPropertyRelative("baseChance"));
            Label.text = property.displayName;
            ToggleCurves.Add(new Image() { image = (open ? EditorGUIUtility.IconContent("d_winbtn_win_close_h@2x").image : EditorGUIUtility.IconContent("MoreOptions@2x").image) });
            CurvesContainer.style.display = open ? DisplayStyle.Flex : DisplayStyle.None;
            ToggleCurves.RegisterCallback<ClickEvent>((ClickEvent evt) =>
            {
                open = !open;
                CurvesContainer.style.display = open ? DisplayStyle.Flex : DisplayStyle.None;
                ToggleCurves.Q<Image>().image = (open ? EditorGUIUtility.IconContent("d_winbtn_win_close_h@2x").image : EditorGUIUtility.IconContent("MoreOptions@2x").image);

            });

            ListView effectorsList = new ListView
            {
                showAddRemoveFooter = true,
                showBorder = true,
                showBoundCollectionSize = false,
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight
            };
            effectorsList.BindProperty(property.FindPropertyRelative("chanceEffectors"));
            CurvesContainer.Add(effectorsList);

        }
    }

}