   M S B A M L   `   `   `  ����I  EAHCommonMethod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullW<clr-namespace:Ai.Hong.Common.Convert;assembly=AHCommonMethodAi.Hong.Common.Convert  5      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   TreeViewItemFocusVisual  �� Property �� Value  	TreeArrow  TreeViewArrowButtonStyle �� 
TargetType �� 
TargetType �� Padding F�Name " treeView_ArrowNormalBackground # treeView_ArrowNormalBorderBrush �� Angle �� CenterY �� CenterX	 �� Triggers
 P� Property P� Value �� 
TargetName $  treeView_ArrowMouseOnBorderBrush # treeView_ArrowMouseOnBackground % !treeView_ArrowSelectedBorderBrush $  treeView_ArrowSelectedBackground 	 TreeViewItemTest �� Mode 
 treeView_Foreground � 	Resources,    &Ai.Hong.Common.Convert.IndentConverter    Indent    
MarginLeft  Key ��Name �� CornerRadius  indentConverter �� 	Converter �� RelativeSource � ColumnDefinitions d�Name �� 	ClickMode d� 	IsChecked �� Path ��Name ��Name ]� 
Conditions �� Property �� Value $  treeView_SelectedBackgroundBrush # treeView_SelectedOutBorderBrush  treeView_SelectedForeground �� 
SourceName % !treeView_MouseMoveBackgroundBrush $  treeView_MouseMoveOutBorderBrush   treeView_MouseMoveForeground  toolbarTreeArrowButtonStyle % !toolbarTree_ArrowNormalBackground & "toolbarTree_ArrowNormalBorderBrush ' #toolbarTree_ArrowMouseOnBorderBrush & "toolbarTree_ArrowMouseOnBackground ( $toolbarTree_ArrowSelectedBorderBrush ' #toolbarTree_ArrowSelectedBackground  toobarTreeItem  toolbarTree_Foreground  �� TargetProperty! �� 
TargetName" R� KeyTime  System.Windows.Visibility#   	Collapsed$ R� Value%   Visible	& W� To' g� Duration( W� EasingFunction/  )System.Windows.Media.Animation.CircleEase7  1System.Windows.Media.Animation.EasingFunctionBase)   
EasingMode* �� From	+ �� To, � LayoutTransform- �� ScaleY. N� EnterActions  ExpandAnimation/ N� ExitActions  UnExpandAnimation ( $toolbarTree_MouseMoveBackgroundBrush ' #toolbarTree_MouseMoveOutBorderBrush #  toolbarTree_MouseMoveForeground ! MouseEnterAnimation '" #toolbarTree_SelectedBackgroundBrush &# "toolbarTree_SelectedOutBorderBrush "$ toolbarTree_SelectedForeground % SampleThemeListBoxItemStyle0 �� Orientation %& !SampleDictPanel_ThemeCanUserInput ' visConvt $(  SampleDictPanel_ThemeBarcodeOnly #) SampleDictPanel_ThemeCodeLength�� 5      F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     5      Hrescomm<clr-namespace:Ai.Hong.Common.Convert;assembly=AHCommonMethod  5      ,s&clr-namespace:System;assembly=mscorlib  5      %R$  &	        &	 n     &	 �     7  &		 3    7   7  7  &	 �    7  &	 z    7   7  7  7  7  7 ! &	% �    7 ' 7 ' �� 5      .�5   
   ��   � �� 5	      �� 5
      .R�5      �� 5      5      5      B� 5      $��M0,0 L0,6 L6,0 zC�6%   �� 5      " d�6-   .�5   
   ��   � |� .  6&   �� 5   
     � ��$ 16��6"   �� 5   
     � ��$ 16��6#   �� 5   
     � �� 5      �� 5      " d�6"   .R�5      �� 
������ $��16��66   $ 5,5,5,5q�6B   $��16��6T   ._�5      F� $ 
ExpandPath��6   8��  #���  6U   #���  5      z�5      �� 5   "   $	 135Q�62   $ 3Q�6>   $ 3Q�6J   5      5      	 5       P� 5!      
 �@J�IsMouseOver . . �5"      �� $ 
ExpandPath��67   # �  6O     � ���� 5#      $ 
ExpandPath��65   # �  6M     � ��5$      P� 5%      
 �@d�	IsChecked$
 TrueY�67   . �5&      �� $ 
ExpandPath��6@     � z� 5'   "   �� 5(   &   $	 180Q�66   $ 3Q�6B   $ 3Q�6N   5*      �� 5+      $ 
ExpandPath��67   # �  6O     � ���� 5,      $ 
ExpandPath��65   # �  6M     � ��5-      5/      51      52      �� 54      " Q�6   .�55   
   ��   �@�FocusVisualStyle8   6-   �� 57   
     �@Q�
IsExpanded 6'   �� *
IsExpanded+$ TwoWay=��� 58   
     �@Q�
IsSelected 6'   �� *
IsSelected+$ TwoWay=��� 59   
     � ��
 ���� 6'   �� 5:   
     � ��# � 
 6'   �� 5;   
     � ��$	 0,3q�6$   �� 5<   
     � �� 5=      �� 5>      " Q�6"    5?      ��   5@      $ 19Q�62   $ 5Q�6>   indentConverter 6N   5A      .R�5B      �� $
��Auto��6!   .+�5C      �� $ 	outBorder��6!   $��1q�64   $ 2��6H   $��0q�6Y   ._�5H      �� $ 
itemBorder��6%   # z��69   $ 0��6]   $��0q�6n   #��z��5I   %   ._�5J   "   � ��6'   �� 8   �� *TemplatedParent+ 5K   &   �� 5L   *   $��19 �6;   �� 5M   *   .+�5O   &   d� $ ArrowButton��6C   $��0��63   8�� $ Press=�5Q   3    5P   3   �� $ 
IsExpanded� �� *TemplatedParent+5Q   3   �� 5R   &   $ PART_Header��6G   $��1��67   $��Header��6\   #��z��5S   7   5T   #   5U      5V      �� 5W      $ 	ItemsHost��6)   5X      	 5Z      P� 5[      
 �@Q�
IsExpanded .  . �5\      �� $ 	ItemsHost��6%     � y�$ 	Collapsed=�6R   5]      P� 5^      
 �@��HasItems .  . �5_      �� $ ArrowButton��6%     � y�$ Hidden=�6T   5`      ]� 5a       5b      �� 5c   "    � �� .  6A   �� 5d   "    � ��$
 Auto��6=   ./�5f      �� $ PART_Header��6%     � ��$ 75��6R   5g      ]� 5h       5i      �� 5j   "    � �� .  6A   �� 5k   "    � ��$
 Auto��6>   ./�5m      �� $ PART_Header��6%     � ��$ 19��6S   5n      P� 5o      
 �@Q�
IsSelected . . �5p      �� $ 
itemBorder��6%   # �  6S     � ���� 5q      $ 	outBorder��6%   # �  6S     � ���� 5r      # �  6;     � ��5s      ]� 5t       5u      �� 5v   "   $ 
itemBorder��6,    �@J�IsMouseOver . 6[   �� 5w   "    �@Q�
IsSelected .  6B   ./�5y      �� $ 
itemBorder��6%   # �  6S     � ���� 5z      $ 	outBorder��6%   # �  6S     � ���� 5{      # �  6;     � ��5|      5~      5�      5�      �� 5�      " d�60   .�5�   
   ��   � |� .  6&   �� 5�   
     � ��$ 16��6"   �� 5�   
     � ��$ 16��6#   �� 5�   
     � �� 5�      �� 5�      " d�6"   .R�5�      �� 
������ $��16��66   $ 5,5,5,5q�6B   $��16��6T   ._�5�      F� $ 
ExpandPath��6   8��  #���  6U   #���  5�      z�5�      �� 5�   "   $	 135Q�62   $ 3Q�6>   $ 3Q�6J   5�      5�      	 5�      P� 5�      
 �@J�IsMouseOver . . �5�      �� $ 
ExpandPath��67   # �  6O     � ���� 5�      $ 
ExpandPath��65   # �  6M     � ��5�      P� 5�      
 �@d�	IsChecked$
 TrueY�67   . �5�      �� $ 
ExpandPath��6@     � z� 5�   "   �� 5�   &   $	 180Q�66   $ 3Q�6B   $ 3Q�6N   5�      �� 5�      $ 
ExpandPath��67   # �  6O     � ���� 5�      $ 
ExpandPath��65   # �  6M     � ��5�      5�      5�      5�      �� 5�      " Q�6   .�5�   
   ��   �@�FocusVisualStyle8   6-   �� 5�   
     �@Q�
IsExpanded . 66   �� 5�   
     � ��
 ���� 6'   �� 5�   
     � ��# �  6'   �� 5�   
     � {� 6&   �� $ threeStateCheck�$ OneWay=��� 5�   
     � ��$ 0,5,10,5q�6$   �� 5�   
     � �� 5�      �� 5�      " Q�6"    5�      ��   5�      $ 19Q�62   $ 5Q�6>   indentConverter 6N   �� 5�      ExpandAnimation .��5�      T� $  (UIElement.Visibility)�6<   $! 	ItemsHost��6o   ..�5�   "   i� $" 0��69   #$ Z# 6E   i� 5�   "   $" 0:0:0.1��69   #$ Z% 6K   5�      W� 5�      $! 	ItemsHost��6.   $  LayoutTransform.ScaleY�6P   $& 1X�6�   $' 0:0:0.4D�6�   ( 5�   "     5�   &   $) 	EaseInOut=�61   5�      W� 5�      $! 	ItemsHost��6.   $  Opacity�6P   $& 1X�6t   $' 0:0:0.5D�6{   5�      �� 5�      UnExpandAnimation .��5�      W� $! 	ItemsHost��6.   $  LayoutTransform.ScaleY�6P   $& 0X�6�   $' 0:0:0.3D�6�   ( 5�   "     5�   &   $) 	EaseInOut=�61   5�      W� 5�      $! 	ItemsHost��6.   $  Opacity�6P   $& 0X�6t   $' 0:0:0.4D�6{   T� 5�      $  (UIElement.Visibility)�6<   $! 	ItemsHost��6o   ..�5�   "   i� $" 0:0:0.4��69   #$ Z% 6K   i� 5�   "   $" 0:0:0.5��69   #$ Z# 6K   5�      5�      �� 5�      MouseEnterAnimation .��5�      �� $! 
itemBorder��6-   $*  $(Background).(SolidColorBrush.Color)�6P   $* TransparentX�6�   $' 0:0:0.5D�6�   5�      �� 5�      MouseLeaveAnimation .��5�      �� $! 
itemBorder��6-   $*  $(Background).(SolidColorBrush.Color)�6P   $+ TransparentX�6�   $' 0:0:0.5D�6�   5�      5�      .R�5�      �� $
��Auto��6!   .+�5�      �� $ 	outBorder��6!   $��1q�64   $ 2��6H   $��0q�6Y   ._�5�      �� $ 
itemBorder��6%   # z��69   $ 0��6]   $��0q�6n   #��z��5�   %   ._�5�   &   � ��6+   �� 8   �� *TemplatedParent+ 5�   *   �� 5�   .   $��19 �6?   �� 5�   .   .+�5�   *   d� $ ArrowButton��6G   $��0��67   8�� $ Press=�5�   7    5�   7   �� $ 
IsExpanded� �� *TemplatedParent+5�   7   �� 5�   *   $ PART_Header��6K   $��1��6;   $��Header��6`   #��z��5�   ;   5�   '   5�      5�      �� 5�      $ 	ItemsHost��6)   , 5�      �� 5�   "   $- 0Q�61   5�      5�      	 5�      P� 5�      
 �@Q�
IsExpanded . . 5�      �� 5�   "   8�� 62   / 5�      �� 5�   "   8�� 62   5�      P� 5�      
 �@��HasItems .  . �5�      �� $ ArrowButton��6%     � y�$ Hidden=�6T   5�      P� 5�      
 � {� .  . �5�      ��   � ��
 �����6;   5�      ]� 5       5     �� 5  "    � �� .  6A   �� 5  "    � ��$
 Auto��6=   ./�5     �� $ PART_Header��6%     � ��$ 75��6R   5     ]� 5      5     �� 5	  "    � �� .  6A   �� 5
  "    � ��$
 Auto��6>   ./�5     �� $ PART_Header��6%     � ��$ 19��6S   5     ]� 5      5     �� 5  "   $ 
itemBorder��6,    �@J�IsMouseOver . 6[   �� 5  "    �@Q�
IsSelected .  6B   ./�5     �� $ 
itemBorder��6%   # �  6S     � ���� 5     $ 	outBorder��6%   # �  6S     � ���� 5     # �   6;     � ��. 5     �� 5  "   8�� 62   5     P� 5     
 �@Q�
IsSelected . . �5     �� $ 
itemBorder��6%   # � " 6S     � ���� 5     $ 	outBorder��6%   # � # 6S     � ���� 5      # � $ 6;     � ��5!     5#     5%     5&     �� 5)     " ��6   .�5*  
   ��   � ��# � 
 6'   �� 5+  
     � ��$ 5q�6$   �� 5,  
     � �� 5-     �� 5.     " ��6"    5/     ���� 50     visConvt 67   51     .R�52     �� $ 	outBorder��6   $��1q�60   $ 2��6D   $��0q�6U   ._�53     �� $ 
itemBorder��6!   # z��65   $ 0��6Y   $��0q�6j   #��z��54  %   ._�55     �� .+�56  "   �� $��14(�63   �� 57  "   $0 
Horizontal=�6-   $��10,5,0,0q�6F   .+�59  &   �� #��� & 60   y�5:  0   �� *
canInput+8   �� 5<  &   #��� ( 60   y�5=  0   �� *	useCode+8  �� 5?  &   #��� ) 60   $��10,0,0,0q�6i   �� 5@  &   $��=��60   $	��5,0q�69   �� 5A  &   ��60   �� *	codeNum+$ OneWay=�5B  #   5C     5D     5E     	 5F     P� 5G     
 �@��
IsSelected . . �5H     �� $ 
itemBorder��6%   # �  6S     � ���� 5I     $ 	outBorder��6%   # �  6S     � ���� 5J     # �  6;     � ��5K     ]� 5L      5M     �� 5N  "   $ 
itemBorder��6,    �@J�IsMouseOver . 6[   �� 5O  "    �@��
IsSelected .  6B   ./�5Q     �� $ 
itemBorder��6%   # �  6S     � ���� 5R     $ 	outBorder��6%   # �  6S     � ���� 5S     # �  6;     � ��5T     5V     5X     5Y     5]     