   M S B A M L   `   `   `  ����b  ^Sobees.Library.BLocalizeLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=79232d3f6f965fe8nNclr-namespace:Sobees.Library.BLocalizeLib;assembly=Sobees.Library.BLocalizeLibSobees.Library.BLocalizeLib  5      \ XSobees.Infrastructure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=79232d3f6f965fe8rMclr-namespace:Sobees.Infrastructure.Converters;assembly=Sobees.Infrastructure Sobees.Infrastructure.Converters 5      ! Sobees.Controls.TwitterSearch2   ,Sobees.Controls.TwitterSearch.Views.Settings   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      ^locNclr-namespace:Sobees.Library.BLocalizeLib;assembly=Sobees.Library.BLocalizeLib     5      ]ClsMclr-namespace:Sobees.Infrastructure.Converters;assembly=Sobees.Infrastructure     5      @d2http://schemas.microsoft.com/expression/blend/2008     6h   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     6�     � 	Resources  5       �� MergedDictionaries �� Source@  :Sobees.Infrastructure.Converters.BoolToVisibilityConverter   boolToVisibilityConverter�� 5       5	      �� 5
      $7 1/Sobees.Templates;Component/TemplateSettings.xaml?�6   �� 5      $/ )/Sobees.Templates;Component/Geometry.xaml?�6   %   &	          5      6$   5      .��5      ]� .+�5      �� $	��Top=�6     GroupBoxStyleSettings#���  6"   ��5      �� 5   	   $��Center=�6    �� Orientation$ 
Horizontal=�6/   .+�5   
   F�  �� Stretch$
 Fill=�5   
   $��0��5   
   $��16��5   
   $��16��5   
   $
��Left=�5   
   $��0,0,5,0q�5   
   ���	#FF595959  GeometryAngenage#���  5      $��Center=�61   �� 5   
     TxtblStyleSettingsTitle#���  5   
   ��6<   )   #Sobees.Library.BLocalizeLib.LocText  �   �Sobees.Library.BLocalizeLib.LocalizeExtension`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   Assembly$# Sobees.Configuration.BGlobals��   Dict$ 	Resources��
   Key$ tabItemGeneral��5   
   .��5!      ]� $��0,5,0,0q�6   .+�5"      �� -   5         ��Name$ ckbUseTwitterSearch��5"      $��0q�5%      $	��Top=�6   	 d� 	IsChecked	 5#      �� *IsUseTwitterSearch+��5$        $# Sobees.Configuration.BGlobals��$ 	Resources��$ ckbUseTwitterSearch��5%      �� 5&      -   5        
 ��Name$
 spTwitterSearch��5&      $ 
Horizontal=�6*   $��0,0,0,5q�6�   $	��Top=�6�   y�6C   �� *IsUseTwitterSearch+ �� 	Converter# [  .+�5'      �� $��Center=�6   ��6,     $# Sobees.Configuration.BGlobals��$ 	Resources��$ txtblRefreshTimeTS���� 5(      -   5         ��Name$ slRefreshTimeTS��5(       � Minimum$ 1Q�5)       � Maximum$ 30Q�5*       � SmallChange$ 1Q�5+       �� IsSnapToTickEnabled . $
��Auto��5.        SliderStyleSettingsTime#���  6   $	��5,0q�6F    � Value 5-      �� *SlRefreshTimeTS+ �� Mode$ TwoWay=�5.   F   �� 5/      $��Center=�6r   ��6     $# Sobees.Configuration.BGlobals��$ 	Resources��$ txtblRefreshMin��6r   50      �� 5B   
   -   5        $
 spFriendFeed��5B      $ 
Horizontal=�6+   $	��Top=�5C   c   y�6   �� *IsUseFriendFeed+# [  6c   5H      5I      �� 5J      $	��Top=�6   #���  6"   ��5K      �� 5L   	   $��Center=�6   $ 
Horizontal=�6/   .+�5M   
   F� $
 Fill=�5N   
   $��0��5O   
   $��14��5P   
   $��18��5Q   
   $
��Left=�5R   
   $��0,0,5,0q�5S   
   ���	#FF595959  GeometryRefresh#���  5U      �� 5V   
   $��Center=�5W   
   #���  5X   
   ��5V        $# Sobees.Configuration.BGlobals��$ 	Resources��$ 
btnRefresh��5X   
   5Y   
   .��5[      � $��0,5,0,0q�6
    � RowDefinitions 5\      �� 5]      $
��Auto �6   �� 5^      $
��Auto �6    � ColumnDefinitions 5`      �� 5a      $	��140 �6   �� 5b      $
��Auto �6   �� 5c      $
��Auto �6   .+�5e      �� $��Center=�5f      $
��Wrap=�5g      ��5e        $# Sobees.Configuration.BGlobals��$ 	Resources��$ txtblNumberResult��5g      �� 5h      -   5        $ slRpp��5h      $ 10Q�5i      $	 100Q�5j      $ 1Q�5k       . $
��Auto��5n       � LargeChange$ 10Q�5o      #���  6   $	��3,4q�6I   $��1��6V    5m      �� *Rpp+$ TwoWay=�5o   V   �� 5p      $��2��5q      $��Center=�6   ��5p        $# Sobees.Configuration.BGlobals��$ 	Resources��$ txtblResuls��5q      �� 5r      $��Center=�5s      $
��Wrap=�5t      $��1��6   ��5r        $# Sobees.Configuration.BGlobals��$ 	Resources��$ txtblNumberResultKeeped��5t      �� 5u      -   5        $ slMaxRpp��5u      $	 100Q�5v      $	 800Q�5w      $ 10Q�5x       . $
��Auto��5{      $ 10Q�5|      #���  6   $	��3,4q�6I   $��1��6V   $��1��6c    5z      �� *
MaxTweet+$ TwoWay=�5|   c   �� 5}      $��1��5~      $��2��6   $��Center=�6$   ��5}        $# Sobees.Configuration.BGlobals��$ 	Resources��$ txtblResuls��5~   $   5      5�      5�      5�      