   M S B A M L   `   `   `  ����
  Sobees    Sobees.Views.MainViewTab   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5        � 	Resources  5         ListBoxItemViewTab �� 
TargetType �� Property �� Value �� 
TargetType � 	Resources  Key ��Name  Level2ItemBg J� SnapsToDevicePixels  Level2ItemBorder  Level2ItemBgOver J� Opacity  Level2ItemBorderOver  Level2ItemBgSelected  Level2itemBorderSelected	 ��Name
 �� Triggers P� Property P� Value �� 
TargetName  	dtService ��Name J� OpacityMask �� EndPoint �� 
StartPoint � MappingMode � Color � Offset ��Name 	 TxtblStyleItemTitle��%	  &	        &	 �    �� 5      " ��5      .�5      ��  � ��$
 Left=�5      �� 5	       � ��$	 Top=�5
      �� 5       � ��
 ���� 5      �� 5       � ��$ 0,0,0,0q�5      �� 5       � ��$ 0,0,1,1q�5      �� 5       �@�OverridesDefaultStyle . 5      �� 5       � �� 5      �� 5      " ��6    5      ���� 5   	   itemSelected 6   �� 5   	   itemNoSelected 6   5   	   .R�5      � #��z��6   .+�5   	   �� $ bg��6   #���  5      $	��0,1q�5       . #���  5      �� 5    	   $ bgOver��6   #���  5!      $ 0Q�5"      #���  5#       . $	��0,1q�5%      �� 5&   	   $ 
bgSelected��6   #���  5'      $ 0Q�5(      $	��0,1q�5)      #���  5*       . 5+      �� 5,   	   $	 contentPresenter��6   #��z��5-      #��z��5.      5/   	   
 50      P� 51   	    �@J�IsMouseOver . . �53   
   �� $ bgOver��54       �@J�Opacity$
 0.26Q�55      56   
   P� 57   	    �@��
IsSelected . . �59   
   �� $ 
bgSelected��5:       �@J�Opacity$ 1Q�5;      5<   
   5>      5@      5B      �� 5C      .R�5E      � $��30��6
   $	��100��5F      .+�5G      �� $ 
imgService��6   $
��Left=�5I   	   $��0q�5J   	   $��Uniform=�5K   	   $	��100��5L   	   $y�	Collapsed=�5M   	   ��5H   	   �� *	ImageUser+ 5N      �� 5O      $ 19.4,0&�6   $ -0.1,0&�5P      $ Absolute=�5Q      .��5R   	   � $ Black��6   $ 0.742Q�5S      � 5T   	   $ 1Q�6   5U   	   5W      �� 5X      $ imgService2��6   $��20��5Y   	   $
��Left=�5[   	   $��20��5\   	   $��5,0,0,0q�5]   	   ��5Z   	   �� *BServiceWorkspace.Img+5]   	   �� 5^      $ txtBlService��6   $��NoWrap=�5`      $��10(�5a      #��� 	 5b      $��Center=�5c      $
��Left=�5d      $��30,0,0,0q�5e      $��CharacterEllipsis=�5f      ��5_      �� *DisplayName+5f      5g      5i      5j      .��5k      ]� ��. .+�5l      �� $	��Top=�5m      #��[ 5n       
 ItemsPanelTemplateWrap#��� 
 5p      $��Disabled=�5q        ListBoxStyleEmpty#���  5r      #���   5s      ��5l      �� *BServiceWorkspaces1+ �� SelectedIndex 5o      �� *SelectedServiceIndex+5s      �� 5t      ��6   �� *SelectedService+��5u      �� *" SelectedService.DataTemplateView+5v      5w      