   M S B A M L   `   `   `  ���� K  GAHCommonControls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Ai.Hong.ControlsAi.Hong.Controls  3clr-namespace:Ai.Hong.ControlsAi.Hong.Controls  (    "Ai.Hong.Controls.VectorImageButton   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     0localclr-namespace:Ai.Hong.Controls     /commclr-namespace:Ai.Hong.Controls     -     >�Name$  
btnControl��|�. ����  �� 
TargetType" >� �� Triggers P� (   "Ai.Hong.Controls.VectorImageButton P� Property �@ KeepSelected P� Value . . ���  �� Property � �� �� Value ��  �� RelativeSource �� *Self+ �� Path$ SelectedColor���  � �� ��  �� *Self+$ SelectedBorder�	 � 	Resources	 &    Ai.Hong.Controls.VerticalConvert   verCovt(   "Ai.Hong.Controls.HorizontalConvert  horCovt��%   &	        &	          .���� -   $��controlBorder��$��1q�
 �� CornerRadius
 ��  �� ElementName$ 
btnControl��$ CornerRadius����� $ 
btnControl��$ UnselectedColor����� $ 
btnControl��$ UnselectedBorder� �� Padding �� $ 
btnControl��$ ContentPadding����� " �� P�  �@J�IsMouseOver . . ���  � �� �� $ 
btnControl��$ SelectedBorder���  � �� �� $ 
btnControl��$ SelectedColor�._��� -   $��	rootPanel��$��Center=�$��Center=�|�.  �� Orientation �� $ 
btnControl��$ Orientation� �� Mode$ OneWay=�.+�"   Ai.Hong.Controls.VectorImage  -     Name$ imgBtn������ $ 
btnControl��$ ImageMargin�   VectorSource �� $ 
btnControl��$ VectorGeometry�$ OneWay=����� $ 
btnControl��$ 
ImageWidth����� $ 
btnControl��$ ImageHeight�   	DrawColor �� $ 
btnControl��$ 
ImageColor����� $ 
btnControl��$ Orientation� �� 	Converter# [  ���� $ 
btnControl��$ Orientation�# [ ���� "   ��  �� Binding �� $ controlBorder��$ IsMouseOver� �� Value True.a���  �@ 	DrawColor �� $ 
btnControl��$ ImageSelectedColor��� -   $��txtBtn������ $ 
btnControl��$
 Text����� $ 
btnControl��$ 
TextMargin����� $ 
btnControl��$ Orientation�# [  ���� $ 
btnControl��$ Orientation�# [ ���� " �� P�  � �� Z� . ���  � y�$ 	Collapsed=�