   M S B A M L   `   `   `  ����b  ^Sobees.Library.BLocalizeLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=79232d3f6f965fe8nNclr-namespace:Sobees.Library.BLocalizeLib;assembly=Sobees.Library.BLocalizeLibSobees.Library.BLocalizeLib  5      \ XSobees.Infrastructure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=79232d3f6f965fe8nKclr-namespace:Sobees.Infrastructure.Commands;assembly=Sobees.InfrastructureSobees.Infrastructure.Commands 5      ! Sobees.Controls.TwitterSearch5   /Sobees.Controls.TwitterSearch.Controls.UcAvatar   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      ^locNclr-namespace:Sobees.Library.BLocalizeLib;assembly=Sobees.Library.BLocalizeLib     5      `CommandsKclr-namespace:Sobees.Infrastructure.Commands;assembly=Sobees.Infrastructure     5      -   5          >�Name$  ucAvatar��5      
 � Tag 5      �� *DataContext+ �� ElementName$ 	LstTweets�� � 	Resources 5       �� MergedDictionaries �� Source�� 5	       5
      �� 5      $; 5/Sobees.Templates;Component/TemplateButtonAvatar.xaml?�6   %    5      .��5      �  � RowDefinitions 5      �� 5      �� 5       � ColumnDefinitions 5      �� 5      �� 5      .+�5      �� -   5         ��Name$ 
btnReplies��5         BtnCommandTwitter#���   5      ��5      �� *Tag.ReplyCommand+$ ucAvatar����5      �� 	 � ToolTip	 5      )   #Sobees.Library.BLocalizeLib.LocText  �   �Sobees.Library.BLocalizeLib.LocalizeExtension`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
   Assembly$#
 Sobees.Configuration.BGlobals��   Dict$ 	Resources��
   Key$ btnReply��.��5      F�   PathStyleRepliesInAvatar#���  6   5      �� 5       -   5        $ 
btnReTweet��5       #���   5!      $��8��5$      ��5"      �� *Tag.ReTweetCommand+$ ucAvatar����5#      �� 	 5%        $#
 Sobees.Configuration.BGlobals��$ 	Resources��$ 
btnRetweet��.��5&      F�   PathStyleRetweetInAvatar#���  6   5'      �� 5H      $��1��6   $��2��5I      4  .Sobees.Infrastructure.Commands.BCommandManager   CommandEventName$ SelectionChanged��5K         Command 5J      �� *Tag.MenuActionCommand+$ ucAvatar��   CommandParameter 5L      �� .@�5M      �� -   5         ��Name$ 
ReplyToAll��5M      $ 
ReplyToAll��5N      	 5O        $#
 Sobees.Configuration.BGlobals��$ 	Resources��$ txtReplyToAll����5P        $#
 Sobees.Configuration.BGlobals��$ 	Resources��$ txtReplyToAll��5Q   	   �� 5R      -   5        $ Favorit��5R      $ Favorit��5S      	 5T        $#
 Sobees.Configuration.BGlobals��$ 	Resources��$ 
btnFavorit����5U        $#
 Sobees.Configuration.BGlobals��$ 	Resources��$ 
btnFavorit��5V   	   �� 5]      -   5        $ Follow��5]      $ Follow��5^      	 5_        $#
 Sobees.Configuration.BGlobals��$ 	Resources��$ txtbkFollow����5`        $#
 Sobees.Configuration.BGlobals��$ 	Resources��$ txtbkFollow��5b   	   �� 5i      -   5        $ 	Addtolist��5i      $ 	AddToList��5j      	 5k        $#
 Sobees.Configuration.BGlobals��$ 	Resources��$ txtblAddToList����5l        $#
 Sobees.Configuration.BGlobals��$ 	Resources��$ txtblAddToList��5n   	   5u      5w      5x      