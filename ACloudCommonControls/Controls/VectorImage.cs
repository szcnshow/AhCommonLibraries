using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ai.Hong.Controls
{
    /// <summary>
    /// 矢量图形
    /// </summary>
    public class VectorImage:Image
    {

        /// <summary>
        /// 显示颜色
        /// </summary>
        public SolidColorBrush DrawColor
        {
            get { return (SolidColorBrush)GetValue(DrawColorProperty); }
            set { SetValue(DrawColorProperty, value); }
        }

        /// <summary>
        /// 显示颜色
        /// </summary>
        public static readonly DependencyProperty DrawColorProperty =
            DependencyProperty.Register("DrawColor", typeof(SolidColorBrush), typeof(VectorImage), new UIPropertyMetadata(Brushes.Black));

        DrawingGroup _vectorSource = null;

        /// <summary>
        /// 矢量图形（DrawingGroup）
        /// </summary>
        public DrawingGroup VectorSource
        {
            get { return (DrawingGroup)GetValue(VectorSourceProperty); }
            set
            {
                if (value == null)
                    _vectorSource = null;
                else
                    _vectorSource = value.CloneCurrentValue();

                SetValue(VectorSourceProperty, _vectorSource);
            }
        }

        /// <summary>
        /// 矢量图形（DrawingGroup）
        /// </summary>
        public static readonly DependencyProperty VectorSourceProperty =
            DependencyProperty.Register("VectorSource", typeof(DrawingGroup), typeof(VectorImage), new UIPropertyMetadata(null));

        /// <summary>
        /// 控件初始化
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            //InitTemplate();
        }

        /// <summary>
        /// 初始化显示模板
        /// </summary>
        private void InitTemplate()
        {

        }

        /// <summary>
        /// 设置一个DrawingGroup的颜色
        /// </summary>
        /// <param name="parent">DrawingGroup</param>
        /// <param name="brush">颜色</param>
        private void UpdateChildrenColor(DrawingGroup parent, SolidColorBrush brush)
        {
            if (parent == null)
                return;

            try
            {
                //逐个子图形设置颜色
                foreach (var item in parent.Children)
                {
                    if (item is GeometryDrawing)
                    {
                        var draw = item as GeometryDrawing;
                        if (draw != null)
                        {
                            if (draw.Brush != null && !(draw.Brush is SolidColorBrush))     //如果有其它方式的Brush，不用设置颜色
                                continue;
                            draw.Brush = brush == null ? Brushes.Black : brush;
                        }
                    }
                    else if (item is DrawingGroup)   //可能还有子路径
                    {
                        UpdateChildrenColor(item as DrawingGroup, brush);
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// 设置图形颜色
        /// </summary>
        private void SetImageBrush(DrawingImage drawImage, SolidColorBrush brush)
        {
            if (drawImage == null || drawImage.Drawing == null || !(drawImage.Drawing is DrawingGroup))
                return;

            DrawingGroup drawgroup = drawImage.Drawing as DrawingGroup;
            UpdateChildrenColor(drawgroup, brush);
        }

        static bool isChanging = false;

        /// <summary>
        /// 属性变化消息
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (isChanging)
                return;

            //颜色变化消息
            if(e.Property.Name == "DrawColor")
            {
                isChanging = true;
                if(Source != null && (Source is DrawingImage))
                {
                    SetImageBrush(Source as DrawingImage, e.NewValue as SolidColorBrush);
                }
                isChanging = false;
            }
            else if(e.Property.Name == "VectorSource")  //内容变化消息
            {
                isChanging = true;
                DrawingGroup drawgroup = e.NewValue as DrawingGroup;
                if(drawgroup != null && drawgroup.Children.Count > 0)
                {
                    Source = new DrawingImage(drawgroup);
                    SetImageBrush(Source as DrawingImage, DrawColor);
                }
                else
                {
                    Source = null;
                }
                isChanging = false;
            }
            else
                base.OnPropertyChanged(e);
        }
    }
}
