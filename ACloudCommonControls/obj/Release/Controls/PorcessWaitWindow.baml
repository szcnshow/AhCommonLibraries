   M S B A M L   `   `   `  ���� K  GAHCommonControls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Ai.Hong.ControlsAi.Hong.Controls  (    "Ai.Hong.Controls.PorcessWaitWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     /vImgclr-namespace:Ai.Hong.Controls     -     � WindowStartupLocation$  CenterOwner=� � ShowInTaskbar .   � 
ResizeMode$ CanMinimize=� � WindowStyle$
 None=� � Title$ 
Processing��$	��170��$	��400�� � SizeToContent$ WidthAndHeight=� � 	Resources  �� MergedDictionaries �� Source��  �� $D >/AHCommonResources;component/Vector/VectorGraphicResource.xaml?�%    .��� -   $��rootGrid��	 � RowDefinitions	 �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� -   $��promptTitle��$��Center=�$��10q�
 �� TextAlignment$

 Left=�$��Center=�����  �� Path$ message� �� Mode$ OneWay=�� -   $��processingBar��$��1��$��20��$��10q�
�������$	��250�� � Maximum �� $ maximum�$ OneWay=� � Value �� $ curValue�$ OneWay=�(   "Ai.Hong.Controls.VectorImageButton  -     Name$ 	btnCancel��(   "Ai.Hong.Controls.VectorImageButton   Clicked$ btnCancel_Click��$��2��   CancelWithCircelGeometry   VectorGeometry# �   $��10q�   Text$ Cancel��$	��100��$��40��   ContentPadding$
 10,5q�   SelectedColor
 ��+��