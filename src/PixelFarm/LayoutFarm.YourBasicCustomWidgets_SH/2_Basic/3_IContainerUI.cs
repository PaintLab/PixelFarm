//Apache2, 2014-present, WinterDev
using System;

namespace LayoutFarm.UI
{
    public static class UIElemExtensions
    {
        public static void RemoveSelf(this UIElement ui)
        {
            //TODO: must ask parent for remove its child
            if (ui.ParentUI is IContainerUI parentUI)
            {
                parentUI.RemoveChild(ui);
            }
        }
        public static void BringToTopMost(this UIElement ui)
        {

            if (ui.ParentUI is IContainerUI parentUI)
            {
                //after RemoveSelf_parent is set to null
                //so we backup it before RemoveSelf
                parentUI.BringChildToFrontMost(ui);
            }
        }
        public static void BringToTopOneStep(this UIElement ui)
        {
            //ask parent for this operation
            if (ui.ParentUI is IContainerUI parentUI)
            {
                //find next element
                parentUI.BringChildToFront(ui, 1);
            }
        }
        public static void SendToBackMost(this UIElement ui)
        {
            if (ui.ParentUI is IContainerUI parentUI)
            {
                //after RemoveSelf_parent is set to null
                //so we backup it before RemoveSelf 
                parentUI.SendChildToBackMost(ui);
            }
        }
        public static void SendOneStepToBack(this UIElement ui)
        {

            if (ui.ParentUI is IContainerUI parentUI)
            {
                //find next element
                parentUI.SendChildToBack(ui, 1);
            }
        }
    }

    public interface IContainerUI
    {
        void RemoveChild(UIElement ui);
        void ClearChildren();
        void AddFirst(UIElement ui);
        void AddAfter(UIElement afterUI, UIElement ui);
        void AddBefore(UIElement beforeUI, UIElement ui);
        void Add(UIElement ui);

        bool BringChildToFront(UIElement ui, int steps);
        bool BringChildToFrontMost(UIElement ui);
        bool SendChildToBack(UIElement ui, int steps);
        bool SendChildToBackMost(UIElement ui);
    }

}