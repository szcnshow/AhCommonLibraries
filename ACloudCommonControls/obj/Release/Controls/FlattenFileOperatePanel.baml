   M S B A M L   `   `   `  ���� K  GAHCommonControls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Ai.Hong.ControlsAi.Hong.Controls  .    (Ai.Hong.Controls.FlattenFileOperatePanel   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     /vImgclr-namespace:Ai.Hong.Controls     -   $��toolbarOperate��  � 	Resources   �� MergedDictionaries �� Source   btnStyle �� 
TargetType(   "Ai.Hong.Controls.VectorImageButton �� Property �� Value �� Path �� RelativeSource �� AncestorType	 �� Mode
 �� ElementName �� Triggers P� Property P� Value��  �� $D >/AHCommonResources;component/Vector/VectorGraphicResource.xaml?�%  &	        �� "  .���  � �� �� $ 
Foreground� �� $ UserControlL�$	 OneWay=���  � �� �� $ ButtonMargin� �� $ UserControlL�$	 OneWay=���  �@ 
ImageWidth �� *
ButtonSize+$
 toolbarOperate����  �@ 
ImageWidth �� *
ButtonSize+$
 toolbarOperate����  �@ SelectedColor �� $ MouseOnColor� �� $ UserControlL�$	 OneWay=���  �@ 
ImageColor �� $ ButtonColor� �� $ UserControlL�$	 OneWay=���  � �� �� $ ButtonMargin� �� $ UserControlL�$	 OneWay=���  �@ 
TextMargin$ 0q���  �@ Orientation$ 
Horizontal=� P�  � {� .  . ���  �@ 
ImageColor �� $ DisabledColor� �� $ UserControlL�$	 OneWay=�.��� .+�`� -    `�Name$ toolBarTray�� `� Orientation �� $
 toolbarOperate��$ Orientation����� $ Width� �� $ UserControlL�$	 OneWay=� `� 
Background �� $ 
Background� �� $ UserControlL�$	 OneWay=�.�c� 
������ .@�  -     Name$ btnOpen��(   "Ai.Hong.Controls.VectorImageButton   Clicked$ Button_Click�� � ToolTip$ 加载��  FileOpenGeometry   VectorGeometry# �    -   $ btnSave��$ Button_Click��$ 保存��  FileSaveGeometry# �    -   $ 	btnSaveAs��$ Button_Click��$ 另存��  FileSaveAsGeometry# �    -   $ 
btnSaveAll��$ Button_Click��$ 保存全部��  FileSaveAllGeometry# �    -   $ btnClose��$ Button_Click��$ 关闭��  FileCloseGeometry# �    -   $ btnCloseAll��$ Button_Click��$ 关闭全部��  FileCloseAllGeometry# �    -	   $ btnUndo��$ Button_Click��$
 Undo��  FileUndoGeometry# �    -
   $ 	btnImport��$ Button_Click��$ 导入��  FileImportGeometry# �    -   $ 	btnExport��$ Button_Click��$ 导出�� 	 FileExportGeometry# � 	 