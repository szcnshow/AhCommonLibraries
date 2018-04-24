using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using OxyPlot;
using OxyPlot.Series;

namespace Ai.Hong.Controls.Chart
{
    /// <summary>
    /// Normal OxyPlot behavior is to show the tracker when the bound mouse button is pressed,
    /// and hide it again when the button is released. With this behavior set, the tracker will stay open
    /// until the user clicks the plot outside it (or the plot is modified).
    /// usage:<PlotView behaviors:ShowTrackerAndLeaveOpenBehavior.BindToMouseDown="Left" />
    /// </summary>
    public static class ShowTrackerAndLeaveOpenBehavior
    {
        public static readonly DependencyProperty BindToMouseDownProperty = DependencyProperty.RegisterAttached(
            "BindToMouseDown", typeof(OxyMouseButton), typeof(ShowTrackerAndLeaveOpenBehavior),
            new PropertyMetadata(default(OxyMouseButton), OnBindToMouseButtonChanged));

        [AttachedPropertyBrowsableForType(typeof(IPlotView))]
        public static void SetBindToMouseDown(DependencyObject element, OxyMouseButton value)
        {
            element.SetValue(BindToMouseDownProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(IPlotView))]
        public static OxyMouseButton GetBindToMouseDown(DependencyObject element)
        {
            return (OxyMouseButton)element.GetValue(BindToMouseDownProperty);
        }

        private static void OnBindToMouseButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is IPlotView))
                throw new InvalidOperationException("Can only be applied to {nameof(IPlotView)}");

            var plot = d as IPlotView;
            if (plot.ActualModel == null)
                throw new InvalidOperationException("Plot has no model");

            var controller = plot.ActualController;
            if (controller == null)
                throw new InvalidOperationException("Plot has no controller");

            if (e.OldValue is OxyMouseButton)
            {
                var oldButton = (OxyMouseButton)e.OldValue;
                if (oldButton != OxyMouseButton.None)
                    controller.UnbindMouseDown(oldButton);
            }

            var newButton = GetBindToMouseDown(d);
            if (newButton == OxyMouseButton.None)
                return;

            controller.UnbindMouseDown(newButton);
            controller.BindMouseDown(newButton, new DelegatePlotCommand<OxyMouseDownEventArgs>(
                AddStayOpenTrackerManipulator));
        }

        private static void AddStayOpenTrackerManipulator(IPlotView view, IController controller,
            OxyMouseDownEventArgs e)
        {
            controller.AddMouseManipulator(view, new StayOpenTrackerManipulator(view), e);
        }

        private class StayOpenTrackerManipulator : TrackerManipulator
        {
            private readonly PlotModel _plotModel;
            private bool _isTrackerOpen;

            public StayOpenTrackerManipulator(IPlotView plot)
                : base(plot)
            {
                _plotModel = plot != null ? plot.ActualModel : null;
                if (_plotModel == null)
                    throw new ArgumentException("Plot has no model", "plot");

                Snap = true;
                PointsOnly = false;
            }

            public override void Started(OxyMouseEventArgs e)
            {
                _plotModel.TrackerChanged += HandleTrackerChanged;
                base.Started(e);
            }

            public override void Completed(OxyMouseEventArgs e)
            {
                if (!_isTrackerOpen)
                {
                    ReallyCompleted(e);
                }
                else
                {
                    // Completed() is called as soon as the mouse button is released.
                    // We won't call the base Completed() here since that would hide the tracker.
                    // Instead, defer the call until one of the hooked events occurs.
                    // The caller will still remove us from the list of active manipulators as soon as we return,
                    // but that's good; otherwise the tracker would continue to move around as the mouse does.
                    new DeferredCompletedCall(_plotModel, () => ReallyCompleted(e)).HookUp();
                }
            }

            private void ReallyCompleted(OxyMouseEventArgs e)
            {
                base.Completed(e);

                // Must unhook or this object will live as long as the model (instead of as long as the manipulation)
                _plotModel.TrackerChanged -= HandleTrackerChanged;
            }

            private void HandleTrackerChanged(object sender, TrackerEventArgs e)
            {
                _isTrackerOpen = e.HitResult != null;
            }

            /// <summary>
            /// Monitors events that should trigger manipulator completion and calls an injected function when they fire
            /// </summary>
            private class DeferredCompletedCall
            {
                private readonly PlotModel _plotModel;
                private readonly Action _completed;

                public DeferredCompletedCall(PlotModel plotModel, Action completed)
                {
                    if (plotModel != null)
                        _plotModel = plotModel;
                    else
                        throw new ArgumentNullException("plotModel");

                    if (completed != null)
                        _completed = completed;
                    else
                        throw new ArgumentNullException("completed");
                }

                /// <summary>
                /// Start monitoring events. Their observer lists will keep us alive until <see cref="Unhook"/> is called.
                /// </summary>
                public void HookUp()
                {
                    Unhook();

                    _plotModel.MouseDown += HandleMouseDown;
                    _plotModel.Updated += HandleUpdated;
                    _plotModel.MouseLeave += HandleMouseLeave;
                }

                /// <summary>
                /// Stop watching events. If they were the only things keeping us alive, we'll turn into garbage.
                /// </summary>
                private void Unhook()
                {
                    _plotModel.MouseDown -= HandleMouseDown;
                    _plotModel.Updated -= HandleUpdated;
                    _plotModel.MouseLeave -= HandleMouseLeave;
                }

                private void CallCompletedAndUnhookEvents()
                {
                    _completed();
                    Unhook();
                }

                private void HandleUpdated(object sender, EventArgs e)
                {
                    CallCompletedAndUnhookEvents();
                }

                private void HandleMouseLeave(object sender, OxyMouseEventArgs e)
                {
                    CallCompletedAndUnhookEvents();
                }

                private void HandleMouseDown(object sender, OxyMouseDownEventArgs e)
                {
                    CallCompletedAndUnhookEvents();

                    // Since we're not setting e.Handled to true here, this click will have its regular effect in
                    // addition to closing the tracker; e.g. it could open the tracker again at the new position.
                    // Modify this code if that's not what you want.
                }
            }
        }
    }
}
