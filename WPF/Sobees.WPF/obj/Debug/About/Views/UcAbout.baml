   M S B A M L   `   `   `  ����b  ^Sobees.Library.BLocalizeLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=79232d3f6f965fe8nNclr-namespace:Sobees.Library.BLocalizeLib;assembly=Sobees.Library.BLocalizeLibSobees.Library.BLocalizeLib  5      
 Sobees    Sobees.About.Views.UcAbout   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      ^locNclr-namespace:Sobees.Library.BLocalizeLib;assembly=Sobees.Library.BLocalizeLib     5        � DataContext  5      �� *AboutViewModel+   Locator �� Source# [   � 	Resources 5       �� MergedDictionaries �� Source�� 5       5      �� 5   
   $0 */Sobees.Templates;Component/Templates.xaml?�6   %    5      .��5      � -   5         �Name$ grMain��5   	   .+�5      �� -   5         ��Name$ Bg��5        ContentControlStyleBgPaper#���  5      �� 5      -   5         ��Name$ btnClose��5      $��Right=�5      $��Bottom=�5      $��	0,0,10,10q�5      y�5      �� *CloseButtonVisibility+��5      �� *CloseCommand+��5      )   #Sobees.Library.BLocalizeLib.LocText  �   �Sobees.Library.BLocalizeLib.LocalizeExtension`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   Assembly$# Sobees.Configuration.BGlobals��	   Dict$	 	Resources��

   Key$
 btnOk��5      ]� 5      $	��Top=�6   $	��300��5      $��0,30,0,0q�5      .+�5       �� $	��137��6   $��34.065��5!      $A��;/Resources\Images\logo_sobees_transparent_small_no_text.png��5"      $
��Fill=�5#      $
��Left=�5$      $��-7,0,0,5q�5%      $	��Top=�5&      �� 5(      $	��Top=�5)        TxtblStyleItemContent#���  5*      ��5(        $# Sobees.Configuration.BGlobals��$	 	Resources��$
 txtblVersionShort��5*      �� 5+       �� Orientation$ 
Horizontal=�6   $	��Top=�5,      .+�5-   
   �� $��0,0,3,0q�5.      #���  5/      ��5-        $# Sobees.Configuration.BGlobals��$	 	Resources��$
 txtblVersionBeta��5/      �� 50   
   -   5         ��Name$ txtblVersion��50      #���  52      $��0,0,5,0q�53      ��51      �� *VersionNumber+53      54   	   �� 55      #���  56      $��0,0,0,10q�57      $	��Top=�58      ��55        $# Sobees.Configuration.BGlobals��$	 	Resources��$
 txtblCopyright��58      �� 59      #���  6   $	��Top=�5;      ��5:        $# Sobees.Configuration.BGlobals��$	 	Resources��$
 txtblCopyrightWarning��5;      �� 5<      $��http://www.sobees.com/sla��5>      $��0,10,0,0q�5?      $	��Top=�5@        BtnStyleFooterItem#���  5A      $��12(�5B      ��5<        $# Sobees.Configuration.BGlobals��$	 	Resources��$
 hypbtnlicence����5=      �� *GoToWebCommand+5B      �� 5C      $ 
Horizontal=�6   $��0,10,0,0q�5D      $y�	Collapsed=�5E      .+�5W   
   �� -   5        $ hypbtnFollowFB��5W      $
��Left=�5X      $	��Top=�5Y      $��0,0,10,0q�5Z       � Cursor$
 Hand��5[      $��0q�5\      .��5]      �� $	��Top=�6   $	��144��5^      $��44��5_      $
��Left=�5`      5a      �� 5r   
   -   5        $ hypbtnFollowTW��5r      $
��Left=�5s      $	��Top=�5t      $
 Hand��5u      $	��114��5v      .��5w      ��   ibTwitterIcon#��[ 6   $	��Top=�5x      $��40��5y      $��40��5z      $
��Left=�5{      5|      5}   	   �� 5~      $��0,10,0,0q�6   $ 
Horizontal=�5      $	��Top=�5�      .+�5�   
   �� $
��Left=�6   $$��http://www.facebook.com/sobees��5�      $	��Top=�5�      $��0,0,5,0q�5�        BtnFollowFacebook#���  5�      ��5�      �� *GoToWebCommand+ � ToolTip 5�        $# Sobees.Configuration.BGlobals��$	 	Resources��$
 hypbtnFollowFB��.��5�      ��   ibFollowFacebook#��[ 6   $	��Top=�5�      $	��144��5�      $��44��5�      $
��Left=�5�      5�      �� 5�   
   $
��Left=�6   $��http://twitter.com/sobees��5�      $	��Top=�5�      $	��114��5�      #���  5�      ��5�      �� *GoToWebCommand+
 � Tag 5�        $# Sobees.Configuration.BGlobals��$	 	Resources��$
 hypbtnFollowTW��.��5�      �� #��[ 6   $	��Top=�5�      $��40��5�      $��40��5�      $
��Left=�5�      $��2q�5�      5�      5�   	   5�      5�      5�      