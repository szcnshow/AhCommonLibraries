   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089&    System.Windows.Controls.DataGrid  �� 
TargetType   CommonDataGridStyle �� Property �� Value  CommonGrid_Background  CommonGrid_BorderBrush  CommonGrid_HorLinesBrush  CommonGrid_VerLinesBrush # CommonDataGridColumnHeaderStyle  CommonDataGridRowStyle  CommonDataGridCellStyle " Theme_ContentDatagrid_FontName "	 Theme_ContentDatagrid_FontSize 
 DataGridColumnHeaderHeight  DataGridOperateButtonHeight  ExpanderHeaderHeight=  7System.Windows.Controls.Primitives.DataGridColumnHeader  CommonGridHeader_Foreground �� 
TargetType ��Name  CommonGridHeader_Background   CommonGridHeader_BorderBrush � ColumnDefinitions F�Name �� Stretch J� RenderTransformOrigin	 �� Triggers
 P� Property P� Value �� 
TargetName �� Angle)  #System.Windows.Controls.DataGridRow  CommonGrid_Foreground �� Triggers  CommonGridRow_OddBackground  CommonGridRow_EnvBackground  CommonGridRow_HighLight*  $System.Windows.Controls.DataGridCell  queryResultDataGridStyle  queryResultGrid_Background  queryResultGrid_BorderBrush ! queryResultGrid_HorLinesBrush ! queryResultGrid_VerLinesBrush ( $queryResultDataGridColumnHeaderStyle  queryResultDataGridRowStyle   qeuryResultDataGridCellStyle $  queryResultGridHeader_Foreground $  queryResultGridHeader_Background % !queryResultGridHeader_BorderBrush  queryResultGrid_Foreground $   queryResultGridRow_OddBackground $!  queryResultGridRow_EnvBackground  " queryResultGridRow_HighLight�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      ,s&clr-namespace:System;assembly=mscorlib  5      %  &	        &	
 �    &	 �    &	 
    &	 $    7 
 &	 	    &	 
    &	     &	 �    7 
 &	 �    &	 �    �� 5      "    6   .�5   
   ��  �@  CanUserResizeColumns .  61   �� 5	   
    � ��# �  6'   �� 5
   
    � ��# �  6(   �� 5   
     �@  HorizontalGridLinesBrush# �  65   �� 5   
    �@  VerticalGridLinesBrush# �  63   �� 5   
    �@  AutoGenerateColumns .  60   �� 5   
    �@  
IsReadOnly . 6'   �� 5   
    � ��$ 0q�6,   �� 5   
    �@  ColumnHeaderStyle# �  6.   �� 5   
    �@  RowStyle# �  6%   �� 5   
    �@  	CellStyle# �  6&   �� 5   
    �@��AlternationCount$ 2��6-   �� 5   
    �@  HeadersVisibility$ Column=�6.   �� 5   
    � ��# �  6'   �� 5   
    � ��# � 	 6%   5      X�5      28Q�66   X�5      30Q�67   X�5      26Q�60   �� 5      "   6   .�5   
   ��  �@J�SnapsToDevicePixels . 60   �� 5    
    � ��$ 0��6%   �� 5!   
    � ��8   6&   �� 5"   
    � ��# �  6'   �� 5#   
    � ��$ 12(�6%   �� 5$   
    �@�Cursor$
 Hand��6#   �� 5%   
    � �� 5&      �� 5'      "  6"   .R�5)      �� $ BackgroundBorder��6   $��0,0,1,0q�67   #���  6Q   #���  5*      ._�5+      �  5,      �� 5-   "   $
��Auto �63   �� 5.   "   $
��Auto �63   .+�50      F� $ 	SortArrow��6#   $y�	Collapsed=�66   "����        �@B     � � @B 	$
 Fill=�6f   $��0��51   %   $��8��65   $��6��6?   
�������$	��3,0q�6W   $��Center=�52   %   $ 0.5,0.5&�6@   �� 54      $��1��6/   $��Center=�6?   $��Center=�6Z   $	��3,0q�6w   55      56      	 57      P� 58      
 �@ SortDirection$ 	AscendingX�6;   . �59      �� $ 	SortArrow��6%    � y�$ Visible=�6R   �� 5:      $ 	SortArrow��6%    � z� 5;   "   �� 5<   &   $	 180Q�66   5>      5?      P� 5@      
 �@ SortDirection$ 
DescendingX�6;   . �5A      �� $ 	SortArrow��6%    � y�$ Visible=�6R   5B      5D      5F      �� 5G   
    � ��$ 25��6#   5H      �� 5L      "   6   .�5M   
   ��  � ��
 ���� 6'   �� 5N   
    � ��$ 25��6#   �� 5O   
    � ��# �  6'    5P   
   P� 5R      
 �@ AlternationIndex$ 0��62   . �5S      ��  � ��# �  6/   5T      P� 5U      
 �@ AlternationIndex$ 1��62   . �5V      ��  � ��# �  6/   5W      P� 5Y      
 �@J�IsMouseOver . . �5Z      ��  � ��
 �����6/   5[      P� 5]      
 �@ 
IsSelected . . �5^      ��  � ��# �  6/   5_      5a      �� 5d      "   6   .�5e   
   ��  � �� 5f      �� 5g      "  6"   .R�5h      �� $��Center=�6'   5k      5m       5n   
   P� 5o      
 �@ 
IsSelected . . �5p      ��  � ��
 �   �6/   5q      5s      �� 5x      "    6   .�5z   
   ��  �@  CanUserResizeColumns .  61   �� 5{   
    � ��# �  6'   �� 5|   
    � ��# �  6(   �� 5}   
     �@  HorizontalGridLinesBrush# �  65   �� 5~   
    �@  VerticalGridLinesBrush# �  63   �� 5   
    �@  AutoGenerateColumns .  60   �� 5�   
    �@  
IsReadOnly . 6'   �� 5�   
    � ��$ 0q�6,   �� 5�   
    �@  ColumnHeaderStyle# �  6.   �� 5�   
    �@  RowStyle# �  6%   �� 5�   
    �@  	CellStyle# �  6&   �� 5�   
    �@��AlternationCount$ 2��6-   �� 5�   
    �@  HeadersVisibility$ Column=�6.   5�      �� 5�      "   6   .�5�   
   ��  �@J�SnapsToDevicePixels . 60   �� 5�   
    � ��$ 0��6%   �� 5�   
    � ��8   6&   �� 5�   
    � ��# �  6'   �� 5�   
    � ��$ 12(�6%   �� 5�   
    �@�Cursor$
 Hand��6#   �� 5�   
    � �� 5�      �� 5�      "  6"   .R�5�      �� $ BackgroundBorder��6   $��0,0,1,0q�67   #���  6Q   #���  5�      ._�5�      �  5�      �� 5�   "   $
��Auto �63   �� 5�   "   $
��Auto �63   .+�5�      F� $ 	SortArrow��6#   $y�	Collapsed=�66   "����        �@B     � � @B 	$
 Fill=�6f   $��0��5�   %   $��8��65   $��6��6?   
�������$	��3,0q�6W   $��Center=�5�   %   $ 0.5,0.5&�6@   �� 5�      $��1��6/   $��Center=�6?   $��Center=�6Z   $	��3,0q�6w   5�      5�      	 5�      P� 5�      
 �@ SortDirection$ 	AscendingX�6;   . �5�      �� $ 	SortArrow��6%    � y�$ Visible=�6R   �� 5�      $ 	SortArrow��6%    � z� 5�   "   �� 5�   &   $	 180Q�66   5�      5�      P� 5�      
 �@ SortDirection$ 
DescendingX�6;   . �5�      �� $ 	SortArrow��6%    � y�$ Visible=�6R   5�      5�      5�      �� 5�   
    � ��$ 25��6#   5�      �� 5�      "   6   .�5�   
   ��  � ��
 ���� 6'   �� 5�   
    � ��$ 25��6#   �� 5�   
    � ��# �  6'    5�   
   P� 5�      
 �@ AlternationIndex$ 0��62   . �5�      ��  � ��# �   6/   5�      P� 5�      
 �@ AlternationIndex$ 1��62   . �5�      ��  � ��# � ! 6/   5�      P� 5�      
 �@J�IsMouseOver . . �5�      ��  � ��
 �����6/   5�      P� 5�      
 �@ 
IsSelected . . �5�      ��  � ��# � " 6/   5�      5�      �� 5�      "   6   .�5�   
   ��  � �� 5�      �� 5�      "  6"   .R�5�      �� $��Center=�6'   5�      5�       5�   
   P� 5�      
 �@ 
IsSelected . . �5�      ��  � ��
 �   �6/   5�      5�      5�      