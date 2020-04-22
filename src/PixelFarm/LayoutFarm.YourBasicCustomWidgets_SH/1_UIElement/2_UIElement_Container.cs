//Apache2, 2014-present, WinterDev
using System;

namespace LayoutFarm.UI
{
    public static class UIElemExt
    {
        public static void RemoveSelf(this UIElement ui)
        {
            //TODO: must ask parent for remove its child
            RenderElement currentRenderE = ui.CurrentPrimaryRenderElement;
            if (currentRenderE != null &&
                currentRenderE.HasParent)
            {
                currentRenderE.RemoveSelf();
            }

            if (ui.ParentUI is IBoxContainer parentUI)
            {
                parentUI.RemoveChild(ui);
            }
            ui.InvalidateOuterGraphics();
        }
        public static void BringToTopMost(this UIElement ui)
        {

            if (ui.ParentUI is IBoxContainer parentUI)
            {
                //after RemoveSelf_parent is set to null
                //so we backup it before RemoveSelf

                parentUI.RemoveChild(ui);
                parentUI.Add(ui);
                ui.InvalidateGraphics();
            }
        }
        public static void BringToTopOneStep(this UIElement ui)
        {

            if (ui.ParentUI is IBoxContainer parentUI)
            {
                //find next element
                UIElement next = ui.NextUIElement;
                if (next != null)
                {

                    parentUI.RemoveChild(ui);
                    parentUI.AddAfter(next, ui);
                    ui.InvalidateGraphics();
                }
            }
        }
        public static void SendToBackMost(this UIElement ui)
        {
            if (ui.ParentUI is IBoxContainer parentUI)
            {
                //after RemoveSelf_parent is set to null
                //so we backup it before RemoveSelf 
                parentUI.RemoveChild(ui);
                parentUI.AddFirst(ui);
                ui.InvalidateGraphics();
            }
        }
        public static void SendOneStepToBack(this UIElement ui)
        {

            if (ui.ParentUI is IBoxContainer parentUI)
            {
                //find next element
                UIElement prev = ui.PrevUIElement;
                if (prev != null)
                {

                    parentUI.RemoveChild(ui);
                    parentUI.AddBefore(prev, ui);
                }
            }
        }
    }

    public interface IBoxContainer
    {
        void RemoveChild(UIElement ui);
        void ClearChildren();
        void AddFirst(UIElement ui);
        void AddAfter(UIElement afterUI, UIElement ui);
        void AddBefore(UIElement beforeUI, UIElement ui);
        void Add(UIElement ui);
    }



}