   M S B A M L   `   `   `  ����\  XSobees.Infrastructure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=79232d3f6f965fe8rMclr-namespace:Sobees.Infrastructure.Converters;assembly=Sobees.Infrastructure Sobees.Infrastructure.Converters  5      hHclr-namespace:Sobees.Infrastructure.Cache;assembly=Sobees.InfrastructureSobees.Infrastructure.Cache  5      nKclr-namespace:Sobees.Infrastructure.Controls;assembly=Sobees.InfrastructureSobees.Infrastructure.Controls  5       Sobees.Controls.Twitter<   6Sobees.Controls.Twitter.Templates.DtTweetConversation2   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      ]clsMclr-namespace:Sobees.Infrastructure.Converters;assembly=Sobees.Infrastructure     5      ZcacheHclr-namespace:Sobees.Infrastructure.Cache;assembly=Sobees.Infrastructure     5      fcustomControlsKclr-namespace:Sobees.Infrastructure.Controls;assembly=Sobees.Infrastructure     5        � 	Resources  5       �� MergedDictionaries �� Source>   8Sobees.Infrastructure.Converters.NullVisibilityConverter   nullVisibilityConverter@   :Sobees.Infrastructure.Converters.BoolToVisibilityConverter  boolToVisibilityConverter<   6Sobees.Infrastructure.Converters.RelativeTimeConverter  relativeTimeConverter �� 
TargetType �� Property & "BubbleConversationForegroundFooter �� Value �� Triggers P� Property P� Value�� 5       5	      �� 5
   
   $0 */Sobees.Templates;Component/Templates.xaml?�6   �� 5   
   $+ %/Sobees.Templates;Component/Icon.xaml?�6   %j  &	        &	      &	 &     '�� 9       5      6$     5      6&     5      6"   �� 5      " ��6   .�5   
   ��  � ��# �  5      �� 5   
    � �� 5      Z�  5   
   P� 5       �@��IsMouseOver . . �5      ��  � ��# �  5      �� 5       � ��$ 	Underline{�5      5      5   	   5      .��5!      � 	 � ColumnDefinitions	 5"      �� 5#      �� 5$      $
��Auto �6   .+�5'      ��   ContentControlStyleAvatar#���  6   $��Right=�5(      $��1��5)      $��5,0,0,0q�5*      .��5+      � .+�5,   
   (   "Sobees.Infrastructure.Cache.BImage  
   	ImageKind$
 Small=�5-         BCacheImage 5,      �� *User.ProfileImgUrl+5-      �� 5.   
   ���	#FF222222y�5/      �� *RetweeterUser+ �� 	Converter# [    50   
   $
 Small=�51      $��35��52      $��35��53      $
��Left=�55      $	��Top=�56       50      �� *User.ProfileImgUrl+y�54      �� *RetweeterUser+# [  56        57   
   $
 Small=�58      $��Right=�5:      $��Bottom=�5;      $��25��5<      $��25��5=       57      �� *RetweeterUser.ProfileImgUrl+y�59      �� *RetweeterUser+# [  5=      �� 5>   
   -   5         ��Name$ ccAvatar��5>      $��47��5?      $��47��5@      $y�	Collapsed=�5A      5B   	   5C      �� 5D       ( $BorderStyleTwitterConversationBubble#���  6   F� 5E      $��Right=�6   $��
0,11,-10,0q�5F      $	��Top=�5G         PathStyleTwitterConversation#���  5H       J� RenderTransformOrigin$ 0.5,0.5&�5I      z�5J      V� 5K   
   .~�5L      �� �� 5M      �� 5N      T� 5O      5P      5R      � 5S      .+�5T      ]� $��5q�6    � ContextMenu 5U   
   �� 5V      -   5         ��Name$ cm��5V       �� 	StaysOpen . 
������� J� BitmapEffect 5Y      O� 5Z      5\      .+�5_   
   � $	��Top=�6    � Orientation$ 
Horizontal=�5`      .+�5a      �� -   5         ��Name$ btnUserName��5a        BtnStyleTitleItem#���  5e       % !BubbleConversationForegroundTitle#���  5f      ��5b      �� *User.NickName+��5c      �� *$"DataContext.ShowUserProfileCommand+ �� ElementName$ 	lstTweets����5d      �� *User+5f      F� 5g      -   5         F�Name$ decoToUserName��5g       	 PathStyleToUser#��� 	 5i      #���  5j      y�5h      �� *ToUser+# [  5j      �� 5k      -   5        $ btnToUserName��5k      #���  5p      #���  5q      ��5l      �� *ToUser.NickName+y�5m      �� *ToUser+# [  ��5n      �� *$"DataContext.ShowUserProfileCommand+$ 	lstTweets����5o      �� *ToUser+5q      F� 5r      -   5        $ decoToRetweetName��5r       
 PathStyleRetweet#��� 
 5s      #���  5u      y�5t      �� *RetweeterUser+# [  5u      �� 5v      -   5        $ btnToRetweeterName��5v      #���  5{      #���  5|      ��5w      �� *RetweeterUser.NickName+y�5x      �� *RetweeterUser+# [  ��5y      �� *$"DataContext.ShowUserProfileCommand+$ 	lstTweets����5z      �� *RetweeterUser+5|      5}      8   2Sobees.Infrastructure.Controls.BLinkTweetTextBlock  5~   
   -   5          Name$ txtBlContent��5~   -   $	��Top=�5�   -     TxtblStyleItemContent#���  5�   -    ' #BubbleConversationForegroundContent#���  5�   -   6   0Sobees.Infrastructure.Controls.BTranslateTextBox   BText 5   -   �� *Title+5�   -   �� 5�   
   -	   5        $ btnDate��5�        BtnStyleFooterItem#���  5�      #���  5�      ��5�      �� *	PubDate+# [  �� FallbackValue$ 18 mai 1789����5�      �� *DataContext.GoToTweetCommand+$ 	lstTweets����5�      �� 5�      F� 5�   
   -
   5        $ pathLogoType��5�      $��Bottom=�5�      ��5�      �� *
PostType+5�   	   5�      5�      5�      