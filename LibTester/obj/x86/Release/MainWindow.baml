   M S B A M L   `   `   `  ���� K  GAHCommonControls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI6clr-namespace:Ai.Hong.Charts;assembly=AHCommonControlsAi.Hong.Charts  M8clr-namespace:Ai.Hong.Controls;assembly=AHCommonControlsAi.Hong.Controls   	LibTester   LibTester.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Gctrl6clr-namespace:Ai.Hong.Charts;assembly=AHCommonControls     Hbtn8clr-namespace:Ai.Hong.Controls;assembly=AHCommonControls       � Title$  
MainWindow��$	��350��$	��525��.���  � ColumnDefinitions �� $
��Auto ��� $��1* � � RowDefinitions �� $
��Auto ��� $��1* ��� $
��Auto �.+�� $��2�� �� $
��Auto ��� $
��Auto �.+�'   !Ai.Hong.Controls.FileOperatePanel     Orientation$ 
Horizontal=�(   "Ai.Hong.Charts.GraphicOperatePanel  -     Name$ panelOperate��$��1��   Orientation$ 
Horizontal=�$��0��   
ButtonSize$ 24Q�
���   �   MouseOnColor$
 Aqua��   CheckedColor$ Bisque��&    Ai.Hong.Controls.SelectColorList  -   	  Name$	 fileList��$��1��$��0��"   Ai.Hong.Charts.SpectrumChart  -   
  Name$
 graphicCtrl��$��1��$��1��   RevertXAxis .     ChartLanguage$ English=�   selectSensitiy$ 3Q��� $��2��$��1�� �� Orientation$ 
Horizontal=�$��30��$
��10,5q�.+��� -   $��btnLoad��$
��load��$��60���� -   $��	btnRemove��$��remove��$��60��$��20,0,0,0q�