   M S B A M L   `   `   `  ���� I  EAHCommonMethod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullW<clr-namespace:Ai.Hong.Common.Convert;assembly=AHCommonMethodAi.Hong.Common.Convert  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   TreeViewItemFocusVisual  �� Property �� Value  	TreeArrow  TreeViewArrowButtonStyle �� 
TargetType �� 
TargetType �� Padding F�Name " treeView_ArrowNormalBackground # treeView_ArrowNormalBorderBrush �� Angle �� CenterY �� CenterX	 �� Triggers
 P� Property P� Value �� 
TargetName $  treeView_ArrowMouseOnBorderBrush # treeView_ArrowMouseOnBackground % !treeView_ArrowSelectedBorderBrush $  treeView_ArrowSelectedBackground 	 TreeViewItemTest �� Mode 
 treeView_Foreground � 	Resources,    &Ai.Hong.Common.Convert.IndentConverter    Indent    
MarginLeft  Key ��Name �� CornerRadius  indentConverter �� 	Converter �� RelativeSource � ColumnDefinitions d�Name �� 	ClickMode d� 	IsChecked �� Path ��Name ��Name ]� 
Conditions �� Property �� Value $  treeView_SelectedBackgroundBrush # treeView_SelectedOutBorderBrush  treeView_SelectedForeground �� 
SourceName % !treeView_MouseMoveBackgroundBrush $  treeView_MouseMoveOutBorderBrush   treeView_MouseMoveForeground  toolbarTreeArrowButtonStyle % !toolbarTree_ArrowNormalBackground & "toolbarTree_ArrowNormalBorderBrush ' #toolbarTree_ArrowMouseOnBorderBrush & "toolbarTree_ArrowMouseOnBackground ( $toolbarTree_ArrowSelectedBorderBrush ' #toolbarTree_ArrowSelectedBackground  toobarTreeItem  toolbarTree_Foreground  �� TargetProperty! �� 
TargetName" R� KeyTime  System.Windows.Visibility#   	Collapsed$ R� Value%   Visible	& W� To' g� Duration( W� EasingFunction/  )System.Windows.Media.Animation.CircleEase7  1System.Windows.Media.Animation.EasingFunctionBase)   
EasingMode* �� From	+ �� To, � LayoutTransform- �� ScaleY. N� EnterActions  ExpandAnimation/ N� ExitActions  UnExpandAnimation ( $toolbarTree_MouseMoveBackgroundBrush ' #toolbarTree_MouseMoveOutBorderBrush #  toolbarTree_MouseMoveForeground ! MouseEnterAnimation '" #toolbarTree_SelectedBackgroundBrush &# "toolbarTree_SelectedOutBorderBrush "$ toolbarTree_SelectedForeground % SampleThemeListBoxItemStyle0 �� Orientation %& !SampleDictPanel_ThemeCanUserInput ' visConvt $(  SampleDictPanel_ThemeBarcodeOnly #) SampleDictPanel_ThemeCodeLength�� F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     Hrescomm<clr-namespace:Ai.Hong.Common.Convert;assembly=AHCommonMethod  ,s&clr-namespace:System;assembly=mscorlib  %�  &	        &	 &     &	 B     7  &		 M    7   7  7  &	 �    7  &	 �	    7   7  7  7  7  7 ! &	% A    7 ' 7 ' �� .���   � �� �� .R��� B� $��M0,0 L0,6 L6,0 zC��� " d�.���   � |� .  ��   � ��$ 16����   � ��$ 16����   � �� �� " d�.R��� 
������ $��16��$ 5,5,5,5q�$��16��._�F� $ 
ExpandPath��8��  #���  #���  z��� $	 135Q�$ 3Q�$ 3Q�	 P� 
 �@J�IsMouseOver . . ��� $ 
ExpandPath��# �    � ���� $ 
ExpandPath��# �    � ��P� 
 �@d�	IsChecked$
 TrueY�. ��� $ 
ExpandPath��  � z� �� $	 180Q�$ 3Q�$ 3Q��� $ 
ExpandPath��# �    � ���� $ 
ExpandPath��# �    � ���� " Q�.���   �@�FocusVisualStyle8   ��   �@Q�
IsExpanded �� *
IsExpanded+$ TwoWay=���   �@Q�
IsSelected �� *
IsSelected+$ TwoWay=���   � ��
 ���� ��   � ��# � 
 ��   � ��$	 0,3q���   � �� �� " Q� ��   $ 19Q�$ 5Q�indentConverter .R��� $
��Auto��.+��� $ 	outBorder��$��1q�$ 2��$��0q�._��� $ 
itemBorder��# z��$ 0��$��0q�#��z��._�� ���� 8   �� *TemplatedParent+ �� $��19 ��� .+�d� $ ArrowButton��$��0��8�� $ Press=� �� $ 
IsExpanded� �� *TemplatedParent+�� $ PART_Header��$��1��$��Header��#��z���� $ 	ItemsHost��	 P� 
 �@Q�
IsExpanded .  . ��� $ 	ItemsHost��  � y�$ 	Collapsed=�P� 
 �@��HasItems .  . ��� $ ArrowButton��  � y�$ Hidden=�]�  ��  � �� .  ��  � ��$
 Auto��./��� $ PART_Header��  � ��$ 75��]�  ��  � �� .  ��  � ��$
 Auto��./��� $ PART_Header��  � ��$ 19��P� 
 �@Q�
IsSelected . . ��� $ 
itemBorder��# �    � ���� $ 	outBorder��# �    � ���� # �    � ��]�  �� $ 
itemBorder�� �@J�IsMouseOver . ��  �@Q�
IsSelected .  ./��� $ 
itemBorder��# �    � ���� $ 	outBorder��# �    � ���� # �    � ���� " d�.���   � |� .  ��   � ��$ 16����   � ��$ 16����   � �� �� " d�.R��� 
������ $��16��$ 5,5,5,5q�$��16��._�F� $ 
ExpandPath��8��  #���  #���  z��� $	 135Q�$ 3Q�$ 3Q�	 P� 
 �@J�IsMouseOver . . ��� $ 
ExpandPath��# �    � ���� $ 
ExpandPath��# �    � ��P� 
 �@d�	IsChecked$
 TrueY�. ��� $ 
ExpandPath��  � z� �� $	 180Q�$ 3Q�$ 3Q��� $ 
ExpandPath��# �    � ���� $ 
ExpandPath��# �    � ���� " Q�.���   �@�FocusVisualStyle8   ��   �@Q�
IsExpanded . ��   � ��
 ���� ��   � ��# �  ��   � {� �� $ threeStateCheck�$ OneWay=���   � ��$ 0,5,10,5q���   � �� �� " Q� ��   $ 19Q�$ 5Q�indentConverter �� ExpandAnimation .��T� $  (UIElement.Visibility)�$! 	ItemsHost��..�i� $" 0��#$ Z# i� $" 0:0:0.1��#$ Z% W� $! 	ItemsHost��$  LayoutTransform.ScaleY�$& 1X�$' 0:0:0.4D�(   $) 	EaseInOut=�W� $! 	ItemsHost��$  Opacity�$& 1X�$' 0:0:0.5D��� UnExpandAnimation .��W� $! 	ItemsHost��$  LayoutTransform.ScaleY�$& 0X�$' 0:0:0.3D�(   $) 	EaseInOut=�W� $! 	ItemsHost��$  Opacity�$& 0X�$' 0:0:0.4D�T� $  (UIElement.Visibility)�$! 	ItemsHost��..�i� $" 0:0:0.4��#$ Z% i� $" 0:0:0.5��#$ Z# �� MouseEnterAnimation .���� $! 
itemBorder��$*  $(Background).(SolidColorBrush.Color)�$* TransparentX�$' 0:0:0.5D��� MouseLeaveAnimation .���� $! 
itemBorder��$*  $(Background).(SolidColorBrush.Color)�$+ TransparentX�$' 0:0:0.5D�.R��� $
��Auto��.+��� $ 	outBorder��$��1q�$ 2��$��0q�._��� $ 
itemBorder��# z��$ 0��$��0q�#��z��._�� ���� 8   �� *TemplatedParent+ �� $��19 ��� .+�d� $ ArrowButton��$��0��8�� $ Press=� �� $ 
IsExpanded� �� *TemplatedParent+�� $ PART_Header��$��1��$��Header��#��z���� $ 	ItemsHost��, �� $- 0Q�	 P� 
 �@Q�
IsExpanded . . �� 8�� / �� 8�� P� 
 �@��HasItems .  . ��� $ ArrowButton��  � y�$ Hidden=�P� 
 � {� .  . ���   � ��
 �����]�  ��  � �� .  ��  � ��$
 Auto��./��� $ PART_Header��  � ��$ 75��]�  ��  � �� .  ��  � ��$
 Auto��./��� $ PART_Header��  � ��$ 19��]�  �� $ 
itemBorder�� �@J�IsMouseOver . ��  �@Q�
IsSelected .  ./��� $ 
itemBorder��# �    � ���� $ 	outBorder��# �    � ���� # �     � ��. �� 8�� P� 
 �@Q�
IsSelected . . ��� $ 
itemBorder��# � "   � ���� $ 	outBorder��# � #   � ���� # � $   � ���� " ��.���   � ��# � 
 ��   � ��$ 5q���   � �� �� " �� ���� visConvt .R��� $ 	outBorder��$��1q�$ 2��$��0q�._��� $ 
itemBorder��# z��$ 0��$��0q�#��z��._��� .+��� $��14(��� $0 
Horizontal=�$��10,5,0,0q�.+��� #��� & y��� *
canInput+8   �� #��� ( y��� *	useCode+8  �� #��� ) $��10,0,0,0q��� $��=��$	��5,0q��� ���� *	codeNum+$ OneWay=�	 P� 
 �@��
IsSelected . . ��� $ 
itemBorder��# �    � ���� $ 	outBorder��# �    � ���� # �    � ��]�  �� $ 
itemBorder�� �@J�IsMouseOver . ��  �@��
IsSelected .  ./��� $ 
itemBorder��# �    � ���� $ 	outBorder��# �    � ���� # �    � ��