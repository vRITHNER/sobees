   M S B A M L   `   `   `  ����b  ^Sobees.Library.BLocalizeLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=79232d3f6f965fe8nNclr-namespace:Sobees.Library.BLocalizeLib;assembly=Sobees.Library.BLocalizeLibSobees.Library.BLocalizeLib  5   <   \ XSobees.Infrastructure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=79232d3f6f965fe8K   ESobees.Infrastructure.Controls.StatusBoxControls.UcFbStatusBoxButtons   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      ^locNclr-namespace:Sobees.Library.BLocalizeLib;assembly=Sobees.Library.BLocalizeLib     6<   @d2http://schemas.microsoft.com/expression/blend/2008     6�   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     6�     � 	Resources  5       �� MergedDictionaries �� Source   dtListAlbumItem�� 5       5      �� 5   	   $0 */Sobees.Templates;Component/Templates.xaml?�6   %I   &	        �� 5
      .R�5   	   �� ��6   �� *name+5   	   5      .��5      � .+�5      �� -   5         ��Name$ btnFBUploadPhoto��5      |�.  $	��Top=�5      $y�Visible=�5      $��25��5   k   $
��Left=�6w   $��34��6�   ��5      �� *UploadImageCommand+��5      ��  �� ElementName$ 	imgUpload�� � ToolTip 5      )   #Sobees.Library.BLocalizeLib.LocText  �   �Sobees.Library.BLocalizeLib.LocalizeExtension`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   Assembly$# Sobees.Configuration.BGlobals��   Dict$ 	Resources��
   Key$ btnFBUploadPhotoTT��.��5      F�   BrushBtnDefaultForeground#���  6
   	 �� Stretch$
	 Fill=�5      $��15��5      $��11.645��5        GeometryCameraPhoto#���  5      5      ]� 5      $
��Left=�6   $	��Top=�6)   $��0,0,5,0q�6A   .+�5      � $	��Top=�6
   $��0,0,0,5q�6   $
��Left=�60   .+�5      �� -   5        
 ��Name$
 	imgUpload��5      $��70��5!   '   $
��Left=�62   $	��Top=�6M   ��5       �� *	ImageName+y�5!      �� *	IsImgShow+6M   �� 5"      -   5        $ btnDeleteImage��5"      $��Right=�5%      $��Bottom=�5&      $��0,0,1,1q�5'        BtnLittleCommand#���  6   y�5#      �� *	IsImgShow+��5$      �� *DeleteImageCommand+.��5(      F� $
	 Fill=�6   $��9��5*      $��Center=�5+      $��9��5,       J� RenderTransformOrigin$ 0.5,0.499999523162842&�5-      �����^0� @    ц��     Q��� �����@Q $� �����@Q $� B� ��#@Q��� B� ��#@ц��  $� �^0� @ $� ^0� @B� ��#@Q    B� ��#@Q    �����@^0� @�����@	 J� Opacity$	 0.9Q�5/      
���  ����5)      Z� z�51      V� 52   	   .~�53   
   �� �� 54   
   �� 55   
    �� Angle$	 135Q�6   T� 56   
   57   
   59      5:      5;      �� 5<      -   5         ��Name$ cb��5<      #��[  5@      $��Center=�65   $	��Top=�6P   y�5=      �� *	IsImgShow+ �� SelectedItem 5>      �� *SelectedAlbums+��5?      �� *
ListAlbums+5@   P   5A      5B      5C      