using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ai.Hong.Charts
{
    /// <summary>
    /// 数据点形状
    /// </summary>
    public enum PointSharp
    {
        /// <summary>
        /// 棱锥体
        /// </summary>
        Pyramid = 0,
        /// <summary>
        /// 正方体
        /// </summary>
        Cube = 1,
        /// <summary>
        ///球体 
        /// </summary>
        Sphere = 2,
        /// <summary>
        /// 正方形
        /// </summary>
        Square = 10,
        /// <summary>
        /// 圆形
        /// </summary>
        Circle = 11,
        /// <summary>
        /// 菱形
        /// </summary>
        Diamond = 12,
        /// <summary>
        /// 三角形
        /// </summary>
        Triangle = 13,
        /// <summary>
        /// 十字形
        /// </summary>
        Cross = 14,
        /// <summary>
        /// 四星形
        /// </summary>
        Star4 = 15,
        /// <summary>
        /// 五星形
        /// </summary>
        Star5 = 16,
        /// <summary>
        /// 六星形
        /// </summary>
        Star6 = 17,
        /// <summary>
        /// 十星形
        /// </summary>
        Star10 = 18
    }

    /// <summary>
    /// 数据点的结构
    /// </summary>
    public class PointData
    {
        /// <summary>
        /// 数据点名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 中心点X坐标
        /// </summary>
        public double centerX { get; set; }
        /// <summary>
        /// 中心点Y坐标
        /// </summary>
        public double centerY { get; set; }
        /// <summary>
        /// 中心点Z坐标
        /// </summary>
        public double centerZ { get; set; }
        /// <summary>
        /// 点的大小（边长，半径）
        /// </summary>
        public double size { get; set; }
        /// <summary>
        /// 点的形状
        /// </summary>
        public PointSharp pointSharp { get; set; }
        /// <summary>
        /// 点的颜色
        /// </summary>
        public System.Windows.Media.SolidColorBrush color { get; set; }

        /// <summary>
        /// Winform颜色
        /// </summary>
        public System.Drawing.Color winformColor
        {
            get
            {
                if (color == null)
                    return System.Drawing.Color.FromArgb(0x000000);
                else
                    return System.Drawing.Color.FromArgb(color.Color.A, color.Color.R, color.Color.G, color.Color.B);
            }
        }

        /// <summary>
        /// 是否填充
        /// </summary>
        public bool solid { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool isSelected { get; set; }
        /// <summary>
        /// key
        /// </summary>
        public Guid key { get; set; }

        /// <summary>
        /// 创建图像点数据
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="size">大小</param>
        /// <param name="color">颜色</param>
        /// <param name="solid">是否填充</param>
        /// <param name="shape">形状</param>
        public PointData(string name, double x, double y, double z, double size, System.Windows.Media.SolidColorBrush color, bool solid = false, PointSharp shape = PointSharp.Pyramid)
        {
            this.name = name;
            this.centerX = x;
            this.centerY = y;
            this.centerZ = z;
            this.size = size;
            this.color = color;
            this.solid = solid;
            this.pointSharp = shape;
            this.key = Guid.NewGuid();
        }

        /// <summary>
        /// 构建2D或者3D图形数据点
        /// </summary>
        /// <param name="xAxis">X轴</param>
        /// <param name="yAxis">Y轴</param>
        /// <param name="size">大小</param>
        /// <param name="color">颜色</param>
        /// <param name="names">名称</param>
        /// <param name="zAxis">Z轴</param>
        /// <param name="solid">是否填充</param>
        /// <param name="shape">图形形状</param>
        /// <returns></returns>
        public static List<PointData> CreatePointDatas(double[] xAxis, double[] yAxis, double size=1, 
            System.Windows.Media.SolidColorBrush color=null, 
            string[] names = null, double[] zAxis=null, bool solid = false, PointSharp shape = PointSharp.Pyramid)
        {
            if (xAxis == null || yAxis == null ||yAxis.Length != xAxis.Length)
                return null;

            if (color == null)
                color = System.Windows.Media.Brushes.Black;

            if (zAxis != null && zAxis.Length != xAxis.Length)
                return null;

            //如果有提供名称，名称数量必须与坐标数量相同
            if (names != null && names.Length != xAxis.Length)
                return null;

            var retDatas = new List<PointData>();
            for (int i = 0; i < xAxis.Length; i++)
            {
                string name = names == null ? null : names[i];
                double zvalue = zAxis == null ? 1 : zAxis[i];

                retDatas.Add(new PointData(name, xAxis[i], yAxis[i], zvalue, size, color,solid, shape));
            }

            return retDatas;
        }
    }

}
