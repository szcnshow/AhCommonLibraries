using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Xml;

namespace BrukerInstrument
{
    /// <summary>
    /// Web.xaml 的交互逻辑
    /// </summary>
    public partial class Web : Window
    {
        /// <summary>
        /// Scanner Block Temperature [°C]
        /// </summary>
        public double Temperature = -1;
        public double laserWave = -1;
        public string IPAddress=string.Empty;
        public string model;
        public Web(string ip,string Model)
        {
            InitializeComponent();
            Width = 0;
            Height = 0;
            IPAddress=ip;
            this.model = Model;
            browser.Navigate(new Uri("http://" + IPAddress + "/config/report.htm"));
        }

        private delegate void RefreshDelgate();
        public void Refresh()
        {
            browser.Navigate(new Uri("http://" + IPAddress + "/config/report.htm"));
        }

        private void browser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (browser.Document != null)
            {
                try
                {
                    mshtml.HTMLDocument dom = (mshtml.HTMLDocument)browser.Document;
                    //mshtml.IHTMLElement temper=null;
                    switch (model.ToUpper())
                    {
                        case "ALPHA"://读取温度 Scanner Block Temperature [°C] Alpha仪器
                            mshtml.IHTMLElement temper = dom.getElementById("SCRTMPCORR");
                            if (temper != null)
                            {
                                if (!double.TryParse(temper.innerHTML, out Temperature))
                                {
                                    Temperature = -1;
                                }
                            }
                            break;
                        case "MATRIX-I"://读取温度 Scanner Block Temperature [°C] Matrix仪器
                            mshtml.IHTMLElementCollection temperM = dom.getElementsByTagName("TD");
                            if (temperM != null)
                            {
                                bool IsFindOut = false;
                                foreach (mshtml.IHTMLElement p in temperM)
                                {
                                    if (IsFindOut)
                                    {
                                        string temp = p.innerHTML;
                                        if (!double.TryParse(p.innerHTML, out Temperature))
                                        {
                                            Temperature = -1;
                                        }
                                        break;
                                    }
                                    if (p.innerHTML.Contains("Scannerblock Temperature"))
                                    {
                                        //已经找到
                                        IsFindOut = true;
                                    }
                                }
                            }
                            break;
                        case "TANGO":
                            mshtml.IHTMLElementCollection temperT = dom.getElementsByTagName("TD");
                            if (temperT != null)
                            {
                                bool IsFindOut = false;
                                foreach (mshtml.IHTMLElement p in temperT)
                                {
                                    if (IsFindOut)
                                    {
                                        string temp = p.innerHTML;
                                        if (!double.TryParse(p.innerHTML, out Temperature))
                                        {
                                            Temperature = -1;
                                        }
                                        break;
                                    }
                                    if (p.innerHTML != null)
                                    {
                                        if (p.innerHTML.Contains("Scanner Block Temperature"))
                                        {
                                            //已经找到
                                            IsFindOut = true;
                                        }
                                    }
                                }
                            }
                            break;
                        default: break;
                    }
                    //读取激光波数
                    mshtml.IHTMLElementCollection textArea = dom.getElementsByTagName("B");
                    foreach (mshtml.IHTMLElement p in textArea)
                    {
                        if (p.parentElement.parentElement.innerHTML != null && p.parentElement.parentElement.innerHTML.Contains("Current NvRAM Data"))//NvRamDataPrev"))
                        {
                            string htmlText = p.parentElement.parentElement.innerHTML;
                            int fx = htmlText.IndexOf("CLWN");
                            int lx = htmlText.LastIndexOf("CLWN");
                            string result = htmlText.Substring(fx, lx - fx);
                            result = result.Replace("CLWN&gt;", string.Empty).Replace("&lt;/", string.Empty);
                            //string temp1="";
                            ////result = Regex.Replace(result, @"[\d+.]*", "");
                            double temp = -1;
                            if (double.TryParse(result, out temp))
                            {
                                laserWave = temp;
                            }
                            else
                            {
                                laserWave = -1;
                            }
                            // break;                        
                            //string tempString = p.innerHTML;// lwn.parentElement.innerHTML;

                            //string[] result = tempString.Replace("<TD id=LWN>LWN\r\n<TD>Laser Wavenumber</TD>\r\n<TD>", string.Empty).Split('\r');
                            ////string[] res=result.s
                            //if (result != null && result.Count() > 0)
                            //{
                            //    if (!double.TryParse(result[0], out laserWave))
                            //    {
                            //        laserWave = -1;
                            //    }
                            //}
                        }
                    }
                    //int i = 0;
                    //foreach(mshtml.IHTMLElement p in lwn.parentElement.children)
                    //{
                    //    if (i == 0)
                    //    {
                    //        i++;
                    //        continue;
                    //    }

                    //    if (!double.TryParse(p.innerHTML, out laserWave))
                    //    {
                    //        laserWave = -1;
                    //    }
                    //}
                    //}                
                    //mshtml.IHTMLElementCollection laser = dom.getElementsByTagName("TEXTAREA");
                    //mshtml.IHTMLElementCollection node = dom.getElementsByTagName("B");
                    //mshtml.IHTMLElement element=null;

                    //foreach (mshtml.IHTMLElement html in node)
                    //{
                    //    if (html.innerHTML == "Previous Start-Up NvRAM Data")
                    //    {
                    //        element=html;
                    //    }
                    //}
                    //foreach(mshtml.IHTMLElement html in laser)
                    //{
                    //    if(html.parentElement.parentElement.parentElement.parentElement.parentElement==element.parentElement.parentElement)
                    //    {
                    //        XmlDocument doc = new XmlDocument();
                    //        doc.LoadXml(html.parentElement.parentElement.innerHTML);    //加载Xml文件  
                    //        XmlElement rootElem = doc.DocumentElement;   //获取根节点  
                    //        XmlNodeList personNodes = rootElem.GetElementsByTagName("TABLE"); //获取person子节点集合  
                    //        foreach (XmlElement node1 in personNodes)
                    //        {
                    //            foreach (XmlElement child in node1.ChildNodes)
                    //            {
                    //                //读取IP地址 
                    //                if (string.Equals(child.Name, "URL"))
                    //                {

                    //                }

                    //            }
                    //        }
                    //    }
                    //}
                    //foreach (mshtml.IHTMLElement html in node)
                    //{
                    //    if (html.innerHTML == "Previous Start-Up NvRAM Data")
                    //    {
                    //        string htmlText = html.parentElement.parentElement.innerHTML;
                    //        int fx = htmlText.IndexOf("CLWN");
                    //        int lx = htmlText.LastIndexOf("CLWN");
                    //        string result = htmlText.Substring(fx, lx - fx);
                    //        result = result.Replace("CLWN&gt;", string.Empty).Replace("&lt;/", string.Empty);
                    //        //string temp1="";
                    //        ////result = Regex.Replace(result, @"[\d+.]*", "");
                    //        double temp = -1;
                    //        if (double.TryParse(result, out temp))
                    //        {
                    //            laserWave = temp;
                    //        }
                    //        else
                    //        {
                    //            laserWave = -1;
                    //        }
                    //        break;
                    //    }
                    //}
                }
                catch { }
                this.Hide();
            }
            else
            {
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new RefreshDelgate(Refresh));
            }
        }

        private void browser_Navigated(object sender, NavigationEventArgs e)
        {

        }
    }
}
