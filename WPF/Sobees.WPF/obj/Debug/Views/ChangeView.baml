   M S B A M L   `   `   `  ����c  _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity  5   �   h dMicrosoft.Expression.Interactions, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35�`clr-namespace:Microsoft.Expression.Interactivity.Core;assembly=Microsoft.Expression.Interactions'Microsoft.Expression.Interactivity.Core 6  
 Sobees   Sobees.Views.ChangeView   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     6<   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     6y   ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     6�   oic`clr-namespace:Microsoft.Expression.Interactivity.Core;assembly=Microsoft.Expression.Interactions     6    � 	Resources  5         	vbListBOx !� Visual �� ElementName 
 dtView  ListBoxItemStyleChangeView �� 
TargetType �� Property �� Value �� RelativeSource �� AncestorType �� 
TargetType	 �Name
 � RowDefinitions ��Name J� SnapsToDevicePixels J� Opacity �� CornerRadius
 � Tag �� Padding �� Triggers P� Property P� Value �� 
TargetName ]� 
Conditions �� Property �� Value��%>  &	        &	 *     &	 �     !� 5       6%   �� $ listBox���� 5      .R�5      �� $	��300��6!   $��UniformToFill=�6-   ��6   �� 6-   5	      �� 5
      " ��60   .�5      ��  � ��
 ���� 6%   �� 5       � �� 65   �� *HorizontalContentAlignment+ �� " ���� 5       � �� 63   �� *VerticalContentAlignment+ �� " ���� 5       � �� 5   	   �� 5   
   " ��6   .R�5      � $
	 grid��6   
 5      �� 5      $
��Auto �6   �� 5      .+�5      �� $ Bd��6    . $ 0Q�6]   ���	#FF3E95CF$ 5��6�   $��2��6�   $��2��6�   �� 5      #��z��6   # z 6a   #��z��6�   $��4q�6�   $��1��6�   �� 5      $��Center=�6   $
��Wrap=�64   #��z 6H   $	��Top=�6e   ���	#FFF9F9F9$��12(�6�   $ 0q�6�   5       5      P� 5       �@J�IsMouseOver . . �5      �� $ Bd��6'    �@J�Opacity$ 1Q�67   5      P� 5       �@��
IsSelected . 6*   ]� 5        5!      �� 5"       �@��
IsSelected . 6.   �� 5#       �@��IsSelectionActive .  6>   5%      P� 5&       � {� .  6)   5(      5*   	   �� 5+       � ��$ 0,0,20,0q�6!   �� 5,       � ��$ 0q�6"   �� 5-       �@�Tag$ 	View name��6   5.      5/      ��50      �� 51       �� EndPoint$ 0.5,1&�6    �� 
StartPoint$ 0.5,0&�6)   .��52      �  � Color$ 	#FF16181A��6    � Offset$
 0.63Q�6$   � 53      $ 	#FF040404��6   $ 0.239Q�6$   54      .��57      � '  !System.Windows.VisualStateManager   VisualStateGroups 58      %  System.Windows.VisualStateGroup  59   	   -   5        VisualStateGroup��   States. 5:   
      System.Windows.VisualState  -   5        vsIntro��   
Storyboard. 5;      �� .��5<      U�  �� TargetProperty$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�6*     �� 
TargetName$  
stackPanel��6�   .\�5=      �� ! O� KeyTime$! 0��6"   " O� Value$	" 1.5Q�6.   �� 5>      $! 0:0:0.4��6"   $" 1Q�64   # �� 	KeySpline$# 0.5,0.5,0.5,1��6>   5?      U� 5@      $V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�6*   $  
stackPanel��6�   .\�5A      �� $! 0��6"   $	" 1.5Q�6.   �� 5B      $! 0:0:0.4��6"   $" 1Q�64   $# 0.5,0.5,0.5,1��6>   5C      U� 5D      $ (UIElement.Opacity)�6*   $  
stackPanel��6Z   .\�5E      �� $! 0��6"   $" 0Q�6.   �� 5F      $! 0:0:0.2��6"   $" 1Q�64   �� 5G      $! 0:0:0.4��6"   $" 1Q�64   5H      5I      5J      5K   
   .   (System.Windows.Interactivity.Interaction$   Triggers$ 5N      /   )System.Windows.Interactivity.EventTrigger  5O   	   %   	EventName$% Loaded��6   .   (System.Windows.Interactivity.TriggerBase&   Actions.& 5P   
   =  7Microsoft.Expression.Interactivity.Core.GoToStateAction  '   	StateName$' vsIntro��6   5Q   
   .+�5T      �� -   5        ( ��Name$( 
stackPanel��5T      ) �� Orientation$) Vertical=�6#   $��Center=�6:   $��Center=�6W   $��
0,0,0,-190q�6r   * J� RenderTransformOrigin$* 0.5,0.5&�6�   $ 0Q�6�   z�5U      V� 5V      .~�5W      �� �� 5X      �� 5Y      T� 5Z      5[      .+�5]      �� -   5        + ��Name$+ listBox��5]      #��[ 5^   =     ListBoxStyleEmpty#���  6�   #���  6�   $	��210��6�   ��6   �� *	ListViews+��6'   Z� , �� SelectedIndex, 6d   �� *SelectedView+��5_      �� 5`      .R�5a      � 5b      5d      �� 5e      -   5        - ��Name$- reflect��5e      #���   6    $	��210��6C   $* 0.5,0.5&�6P   $��0,0,20,0q�6p   . J� OpacityMask. 5f      �� 5g      $ 0.5,1&�6   $ 0.5,0&�6,   .��5h      � $ 	#00000000��6   $ 0.451Q�6'   � 5i      $ 	#36FFFFFF��6   $ 1Q�6'   5j      z�5l      V� 5m      .~�5n      �� / �� ScaleY$/ -1Q�6   �� 5o      �� 5p      T� 5q      5r      5t      5u      �� 5v      $��Center=�6   $
��Wrap=�6+   $��Choose your view:��6?   $	��Top=�6X   ���	#FFF7F7F7$��0,40,0,0q�6�   $��16(�6�   $
��Bold"�6�   5x      5z      