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
    public class ImageCheckBox : CheckBox
    {
        /// <summary>
        /// 选中图形
        /// </summary>
        public static readonly DependencyProperty CheckedImageProperty =
            DependencyProperty.Register("CheckedImage", typeof(ImageSource), typeof(ImageCheckBox), new UIPropertyMetadata(null));
        /// <summary>
        /// 选中图形
        /// </summary>
        public ImageSource CheckedImage
        {
            get { return (ImageSource)GetValue(CheckedImageProperty); }
            set { SetValue(CheckedImageProperty, value); }
        }

        /// <summary>
        /// 未选中图形
        /// </summary>
        public static readonly DependencyProperty UncheckedImageProperty =
            DependencyProperty.Register("UncheckedImage", typeof(ImageSource), typeof(ImageCheckBox), new UIPropertyMetadata(null));
        /// <summary>
        /// 未选中图形
        /// </summary>
        public ImageSource UncheckedImage
        {
            get { return (ImageSource)GetValue(UncheckedImageProperty); }
            set { SetValue(UncheckedImageProperty, value); }
        }

        /// <summary>
        /// 显示的文本
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ImageCheckBox), new UIPropertyMetadata(null));
        /// <summary>
        /// 显示的文本
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// 控件初始化
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
            InitTemplate();
            base.OnInitialized(e);
        }

        /// <summary>
        /// 初始化Template
        /// </summary>
        protected void InitTemplate()
        {
            Style style = new Style(typeof(CheckBox));
            DataTemplate template = new DataTemplate(typeof(CheckBox));
            style.Setters.Add(new Setter(CheckBox.TemplateProperty, template));

            Grid contentGrid = new Grid();
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            contentGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Auto);
            contentGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);

            FrameworkElementFactory grid = new FrameworkElementFactory(typeof(Grid));
            template.VisualTree = grid;

            FrameworkElementFactory column = new FrameworkElementFactory(typeof(ColumnDefinition));
            column.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Auto));
            grid.AppendChild(column);

            column = new FrameworkElementFactory(typeof(ColumnDefinition));
            column.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Star));
            grid.AppendChild(column);

            //两个图形
            FrameworkElementFactory image = new FrameworkElementFactory(typeof(Image), "checkedImageControl");
            //绑定图形
            Binding bind = new Binding("CheckedImage");
            bind.Mode = BindingMode.OneWay;
            image.SetBinding(Image.SourceProperty, bind);

            //绑定可视
            bind = new Binding("IsChecked");
            bind.Mode = BindingMode.OneWay;
            bind.Converter = new Common.TrueIsVisibilityConvert();
            image.SetBinding(Image.VisibilityProperty, bind);

            grid.AppendChild(image);

            image = new FrameworkElementFactory(typeof(Image), "unCheckedImageControl");
            //绑定图形
            bind = new Binding("unCheckedImage");
            bind.Mode = BindingMode.OneWay;
            image.SetBinding(Image.SourceProperty, bind);

            //绑定可视
            bind = new Binding("IsChecked");
            bind.Mode = BindingMode.OneWay;
            bind.Converter = new Common.TrueIsVisibilityConvert();
            image.SetBinding(Image.VisibilityProperty, bind);

            grid.AppendChild(image);

            //文本
            FrameworkElementFactory txt = new FrameworkElementFactory(typeof(TextBlock), "textControl");
            bind = new Binding("Text");
            bind.Mode = BindingMode.OneWay;
            txt.SetBinding(TextBlock.TextProperty, bind);
            txt.SetValue(Grid.ColumnProperty, 1);
            grid.AppendChild(txt);

            this.ContentTemplate = template;


        }

        /// <summary>
        /// 初始化显示模板
        /// </summary>
        private void InitContent()
        {
            //if (IsChecked == true)
            //    Content = CheckedImage;
            //else
            //    Content = UncheckedImage;
        }

        /// <summary>
        /// 属性变化消息
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "CheckedImage" ||  e.Property.Name == "UncheckedImage" ||
                e.Property.Name == "IsChecked")  //内容变化消息
            {
                InitContent();
            }
            else
                base.OnPropertyChanged(e);
        }
    }
}
