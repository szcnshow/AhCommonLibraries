   M S B A M L   `   `   `  ����K  GAHCommonControls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI6clr-namespace:Ai.Hong.Charts;assembly=AHCommonControlsAi.Hong.Charts  5   	   M8clr-namespace:Ai.Hong.Controls;assembly=AHCommonControlsAi.Hong.Controls  5   	    	LibTester   LibTester.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Gctrl6clr-namespace:Ai.Hong.Charts;assembly=AHCommonControls     5   	   Hbtn8clr-namespace:Ai.Hong.Controls;assembly=AHCommonControls     5   	     � Title$  
MainWindow��5   	   $	��350��6   $	��525��6)   .��5      �  � ColumnDefinitions 5   
   �� 5	      $
��Auto �6   �� 5
      $��1* �6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��1* �6   �� 5      $
��Auto �6   .+�5   
   � $��2��6    5      �� 5      $
��Auto �6#   �� 5      $
��Auto �6#   .+�5      '   !Ai.Hong.Controls.FileOperatePanel     Orientation$ 
Horizontal=�6#   (   "Ai.Hong.Charts.GraphicOperatePanel  5      -   5          Name$ panelOperate��5   ^   $��1��6'      Orientation$ 
Horizontal=�67   $��0��6Q      
ButtonSize$ 24Q�6t   
���   �   MouseOnColor$
 Aqua��6�      CheckedColor$ Bisque��6�   5      &    Ai.Hong.Controls.SelectColorList  5   
   -   5        	  Name$	 fileList��5      $��1��60   $��0��6=   "   Ai.Hong.Charts.SpectrumChart  5   
   -   5        
  Name$
 graphicCtrl��5   :   $��1��6   $��1��6*      RevertXAxis .     ChartLanguage$ English=�6c      selectSensitiy$ 3Q�6{   �� 5   
   $��2��6   $��1��6"    �� Orientation$ 
Horizontal=�62   $��30��6K   $
��10,5q�6W   .+�5      �� -   5        $��btnLoad��5   $   $
��load��6   $��60��6I   �� 5      -   5        $��	btnRemove��5   &   $��remove��6   $��60��6O   $��20,0,0,0q�6Z   5      5      5       