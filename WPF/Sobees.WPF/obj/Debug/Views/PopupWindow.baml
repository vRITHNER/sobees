   M S B A M L   `   `   `  ����
  Sobees    Sobees.Views.PopupWindow   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5        � 	Resources  5
         	PopupOpen �� TargetProperty �� 
TargetName O� KeyTime O� Value��%�  &	        �� 5      .��5      U� $ (UIElement.Opacity)�6&   $ 	mainPopup��5   &   .\�5   
   �� $ 0��6   $	 0.1Q�5      �� 5   
   $ 0:0:0.2��6   $ 0.205Q�5      �� 5   
   $ 0:0:0.4��6   $	 0.4Q�5      �� 5   
   $ 0:0:0.6��6   $
 0.54Q�5      �� 5   
   $ 0:0:0.8��6   $ 0.685Q�5      �� 5   
   $ 0:0:1��6   $ 1Q�5      5   	   5      5      ��5      �� 5       �� Triggers 5      �� 5    
    �� Binding 6   ��  �� Path$ 
Visibility� �� Value Visible5!      	 N� EnterActions	 5"      �� 5#      .��5$      �� .��5%      W� $ Opacity�6"   
 W� From$	
 0.0X�5&   "   	 W� To$	 1.0X�5'   "    g� Duration$ 0:0:0.2D�5(   "   5)      5*      5,      5.      .��50      � 
�������.+�51      ]� $	��Top=�6   .+�52      �� -   5         ��Name$ Title��52      $��Center=�53      $��21.333(�54      $
��Bold"�55      	message6-   56      ]� 57      $��Center=�6   $��10,10,10,10q�58      .+�59      �� $��Center=�6   $��Center=�60   .+�5:      �� -   5        $ MessageContentTw��5:      $��Center=�5;   	   $��16(�5<   	   $��Normal"�5=   	    �� TextAlignment$ Center=�5>   	   $
��Wrap=�5?   	   $	��Top=�5@   0   $��0,0,0,30q�6H   ��6	   �� *PopupMessageContentTw+6H   �� 5A      -   5        $ MessageContentFb��5A      $��Center=�5B      $��16(�5C      $��Normal"�5D      $ Center=�5E      $
��Wrap=�5F      $��0,0,0,30q�5G   9   $	��Top=�6K   ��6   �� *PopupMessageContentFb+6K   5H   	   5I      ]� 5J      $��Bottom=�6   .+�5K      �� -   5         ��Name$ btnOk��5O      $��Ok��5K      $��23��5L      $��50��5M      $��0,0,0,10q�5N      $	��Top=�5Q      ��5P      �� *PopupCloseCommand+5Q      5R      5T      5U      