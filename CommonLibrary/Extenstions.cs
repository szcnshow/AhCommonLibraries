using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;

namespace Ai.Hong.Common.Extenstion
{
    /// <summary>
    /// 灰度图像
    /// Class used to have an image that is able to be gray when the control is not enabled.
    /// Based on the version by Thomas LEBRUN (http://blogs.developpeur.org/tom)
    /// </summary>
    public class AutoGreyableImage : Image
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoGreyableImage"/> class.
        /// </summary>
        static AutoGreyableImage()
        {
            // Override the metadata of the IsEnabled and Source property.
            IsEnabledProperty.OverrideMetadata(typeof(AutoGreyableImage), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnAutoGreyScaleImageIsEnabledPropertyChanged)));
            SourceProperty.OverrideMetadata(typeof(AutoGreyableImage), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnAutoGreyScaleImageSourcePropertyChanged)));
        }

        /// <summary>
        /// Return grey image from source
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        protected static AutoGreyableImage GetImageWithSource(DependencyObject source)
        {
            var image = source as AutoGreyableImage;
            if (image == null)
                return null;

            if (image.Source == null)
                return null;

            return image;
        }

        /// <summary>
        /// Called when [auto grey scale image source property changed].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected static void OnAutoGreyScaleImageSourcePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            AutoGreyableImage image = GetImageWithSource(source);
            if (image != null)
                ApplyGreyScaleImage(image, image.IsEnabled);
        }

        /// <summary>
        /// Called when [auto grey scale image is enabled property changed].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected static void OnAutoGreyScaleImageIsEnabledPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            AutoGreyableImage image = GetImageWithSource(source);
            if (image != null)
            {
                var isEnabled = System.Convert.ToBoolean(args.NewValue);
                ApplyGreyScaleImage(image, isEnabled);
            }
        }

        /// <summary>
        /// Apply grey to image
        /// </summary>
        /// <param name="autoGreyScaleImg">The image source</param>
        /// <param name="isEnabled">True=color image, False=grey image</param>
        protected static void ApplyGreyScaleImage(AutoGreyableImage autoGreyScaleImg, Boolean isEnabled)
        {
            try
            {
                if (!isEnabled)
                {
                    BitmapSource bitmapImage = null;

                    if (autoGreyScaleImg.Source is FormatConvertedBitmap)
                    {
                        // Already grey !
                        return;
                    }
                    else if (autoGreyScaleImg.Source is BitmapSource)
                    {
                        bitmapImage = (BitmapSource)autoGreyScaleImg.Source;
                    }
                    else // trying string 
                    {
                        bitmapImage = new BitmapImage(new Uri(autoGreyScaleImg.Source.ToString()));
                    }
                    FormatConvertedBitmap conv = new FormatConvertedBitmap(bitmapImage, PixelFormats.Gray32Float, null, 0);
                    autoGreyScaleImg.Source = conv;

                    // Create Opacity Mask for greyscale image as FormatConvertedBitmap does not keep transparency info
                    autoGreyScaleImg.OpacityMask = new ImageBrush(((FormatConvertedBitmap)autoGreyScaleImg.Source).Source); //equivalent to new ImageBrush(bitmapImage)
                }
                else
                {
                    if (autoGreyScaleImg.Source is FormatConvertedBitmap)
                    {
                        autoGreyScaleImg.Source = ((FormatConvertedBitmap)autoGreyScaleImg.Source).Source;
                    }
                    else if (autoGreyScaleImg.Source is BitmapSource)
                    {
                        // Should be full color already.
                        return;
                    }

                    // Reset the Opcity Mask
                    autoGreyScaleImg.OpacityMask = null;
                }
            }
            catch (Exception)
            {
                // nothin'
            }

        }

    }

    /// <summary>
    /// TreeView样式所用到的类
    /// </summary>
    public static class TreeViewItemExtensions
    {
        /// <summary>
        /// Get the max depth of this tree view
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int GetDepth(this TreeViewItem item)
        {
            int depth = 0;
            while ((item = item.GetAncestor<TreeViewItem>()) != null)
            {
                depth++;
            }
            return depth;
        }

        /// <summary>
        /// Get the ancestor of a element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T GetAncestor<T>(this DependencyObject source)
            where T : DependencyObject
        {
            do
            {
                source = VisualTreeHelper.GetParent(source);
            } while (source != null && !(source is T));
            return source as T;
        }
    }

    /// <summary>
    /// 可编辑的ComboBox扩展
    /// </summary>
    public static class EditableComboBoxExtensions
    {
        /// <summary>
        /// Get combox's allowable text lenght
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetMaxLength(DependencyObject obj)
        {
            return (int)obj.GetValue(MaxLengthProperty);
        }

        /// <summary>
        /// Set the combox's text length
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetMaxLength(DependencyObject obj, int value)
        {
            obj.SetValue(MaxLengthProperty, value);
        }
        
        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxLength.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.RegisterAttached("MaxLength", typeof(int), typeof(EditableComboBoxExtensions), new UIPropertyMetadata(OnMaxLenghtChanged));

        private static void OnMaxLenghtChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var comboBox = obj as ComboBox;
            if (comboBox == null) return;

            comboBox.Loaded +=
                (s, e) =>
                {
                    //var textBox = comboBox.FindChild(typeof(TextBox), "PART_EditableTextBox");
                    var textBox = comboBox.FindName("PART_EditableTextBox") as TextBox;
                    if (textBox == null) return;

                    textBox.SetValue(TextBox.MaxLengthProperty, args.NewValue);
                };
        }
    }

    /// <summary>
    /// WPF DataGrid控件扩展方法 
    /// </summary>
    public static class DataGridPlus
    {
        /// <summary>
        /// 获取DataGrid控件单元格
        /// </summary>
        /// <param name="dataGrid">DataGrid控件</param>
        /// <param name="rowIndex">单元格所在的行号</param>
        /// <param name="columnIndex">单元格所在的列号</param>
        /// <returns>指定的单元格</returns>
        public static DataGridCell GetCell(this DataGrid dataGrid, int rowIndex, int columnIndex)
        {
            DataGridRow rowContainer = dataGrid.GetRow(rowIndex);
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter == null)
                {
                    dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[columnIndex]);
                    presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                }
                if (presenter == null)
                    return null;

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
                if (cell == null)
                {
                    dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[columnIndex]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
                }
                return cell;
            }
            return null;
        }

        /// <summary>
        /// 获取DataGrid的行
        /// </summary>
        /// <param name="dataGrid">DataGrid控件</param>
        /// <param name="rowIndex">DataGrid行号</param>
        /// <returns>指定的行号</returns>
        public static DataGridRow GetRow(this DataGrid dataGrid, int rowIndex)
        {
            DataGridRow rowContainer = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            if (rowContainer == null)
            {
                dataGrid.UpdateLayout();
                dataGrid.ScrollIntoView(dataGrid.Items[rowIndex]);
                rowContainer = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            }
            return rowContainer;
        }

        /// <summary>
        /// 获取父可视对象中第一个指定类型的子可视对象
        /// </summary>
        /// <typeparam name="T">可视对象类型</typeparam>
        /// <param name="parent">父可视对象</param>
        /// <returns>第一个指定类型的子可视对象</returns>
        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
    }



}
