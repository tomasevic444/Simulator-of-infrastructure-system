   M S B A M L   `   `   `  ����I  ENetworkService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:NetworkService.ViewModelsNetworkService.ViewModels  5   	   ;"clr-namespace:NetworkService.ViewsNetworkService.Views  5   	   /clr-namespace:NetworkServiceNetworkService  5   	   W SNotification.Wpf, Version=8.0.0.0, Culture=neutral, PublicKeyToken=1acac493089262b5_Aclr-namespace:Notification.Wpf.Controls;assembly=Notification.WpfNotification.Wpf.Controls 64       NetworkService.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   =	viewModel'clr-namespace:NetworkService.ViewModels     5   	   4views"clr-namespace:NetworkService.Views     5   	   .localclr-namespace:NetworkService     5   	   [notificationsAclr-namespace:Notification.Wpf.Controls;assembly=Notification.Wpf     64   _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors  6�   $	��600��5
   	   $	��350��6     � 
ResizeMode$  NoResize=�5   	    � AllowsTransparency . 
������  � WindowStyle$
 None=�5       � DataContext 5      3   -NetworkService.ViewModels.MainWindowViewModel  5   
   *  $Microsoft.Xaml.Behaviors.Interaction   Triggers 5      +  %Microsoft.Xaml.Behaviors.EventTrigger  5   
      	EventName$ 	MouseDown��6   *  $Microsoft.Xaml.Behaviors.TriggerBase   Actions. 5      2  ,Microsoft.Xaml.Behaviors.InvokeCommandAction     Command 6$   �� *MouseDownCommand+   CommandParameter 6I   �� 	 �� RelativeSource	 �� 
 �� AncestorType"
 �5      .��5      � .+�5   
   ��  �� CornerRadius$ 30��6   $��2q�6$   ��5      �� 5       �� ImageSource$ Images/background.jpg��6    j� Stretch$ UniformToFill=�6A   5      �� 5   
   $ 30��6   $��4q�5       J� Opacity$
 0.95Q�5        �� Padding$
 10 5q�5!      ��5"      �� 5#       �� 
StartPoint$	 0,0&�6&    �� EndPoint$	 1,1&�67   .��5$      �  � Color$ #462AD8��6#    � Offset$ 0Q�63   � 5%      $ #DA34AE��6#   $
 0.75Q�63   � 5&      $ #8A16C1��6#   $ 1Q�63   5'      ��5)      �� 5*      $	 0,1&�6&   $	 1,0&�67   .��5+      � $ #060531��6#   $ 0Q�63   � 5,      $ #1B1448��6#   $ 1Q�63   5-      ._�50      �  � RowDefinitions 51      �� 52      $	��13* �6$   �� 53      $
��100* �6$   �� 54      $	��13* �6$   .+�56      �� $ 20��6   $��2q�57      $
 0.95Q�58      ���#462AD8��5:      �� 5;      $	 0,0&�6.   $	 1,1&�6?   .��5<      � $ #462AD8��6+   $ 0Q�6;   � 5=      $ #DA34AE��6+   $
 0.75Q�6;   � 5>      $ #8A16C1��6+   $ 1Q�6;   5?      ._�5A      � $��10(�6   $��Roboto)�63    �� TextAlignment$ Center=�6Q    � ColumnDefinitions 5B      �� 5C      $
��0.5* �6/   �� 5D      $
��1.5* �6/   �� 5E      $
��1.5* �6/   �� 5F      $
��1.5* �6/   �� 5G      $
��1.5* �6/   �� 5H      $
��0.3* �6/   .+�5L      �� -   5         ��Name$ btnExit��5M   I   $��4��5L   !    � Cursor$
 Hand��61   
������ 
������ $
��Exit��5M      $��Right=�6-   ��5L   s   �� *ChangeViewCommand+.��5N      �� .+�5O   "   �� $��Images/exit.png��6(   $��Uniform=�6A   $��36��6S   $��5q�6^   �� 5P   "   $
��EXIT��6,   
�������$��
Montserrat)�6K   $7��1-8 -7 0 0                                        q�6c   5R      5S      �� 5V      -   5        $ tableBtn��5V   �   
������ 
������ $��Entities��6z   $��3��6�   $��Right=�6�   $��0,0,70,-0.2q�6�   $��71��6�   $
 Hand��6�   ��6T   �� *ChangeViewCommand+.��5W      �� .+�5X   "   �� $��Images/table.png��6(   $��Uniform=�6B   $��26��6T   $��7q�6_   �� 5Y   "   $��TABLE��6,   
�������$��
Montserrat)�6L   5Z      5[      �� 5^      -   5        $ 
displayBtn��5^   "   $
 Hand��66   $��2��6E   
������ 
������ $��Display��6�   ��6�   �� *ChangeViewCommand+.��5_      �� .+�5`   "   �� $��Images/display.png��6(   $��Uniform=�6D   $��26��6V   $��5q�6a   �� 5a   "   $��DISPLAY��6,   
�������$��
Montserrat)�6N   5b      5c      �� 5f      $��3��6"   $
 Hand��62   
������ 
������ $��Graph��5g      ��5f   u   �� *ChangeViewCommand+.��5h      �� .+�5i   "   �� $��Images/graph.png��6(   $��Uniform=�6B   $��26��6T   $��8q�6_   �� 5j   "   $��GRAPH��6,   
�������$��
Montserrat)�6L   5k      5l      5m      5n      �� 5p      $��1��6!   $��Center=�60   $��Center=�5q      ��6.   �� *SelectedContent+0  *Notification.Wpf.Controls.NotificationArea  5s      -   5          Name$ MainNotificationArea��5t      $��1��5u       K� ZIndex$
 1000��5v         MaxItems$ 2��5w         Position$ 	TopCenter=�5x      $	��250��5y      �� 5{      $ 20��6   $��2q�5|      $
 0.95Q�5}      $��2��5~      ���#462AD8��5�      �� 5�      $	 0,0&�6.   $	 1,1&�6?   .��5�      � $ #462AD8��6+   $ 0Q�6;   � 5�      $ #DA34AE��6+   $
 0.75Q�6;   � 5�      $ #8A16C1��6+   $ 1Q�6;   5�      ._�5�      � $��10(�6   $��Roboto)�63   $ Center=�6Q    5�      �� 5�      $
��1.5* �6/   �� 5�      $
��1.5* �6/   .+�5�      �� $��0��6!   
������ 
������ $
 Hand��6�   $��10 2 0 0q�6�   ��6e   �� *UndoCommand+.��5�      �� .+�5�   "   �� $��Images/back.png��6(   $��Uniform=�6A   $��26��6S   $��5q�6^   �� 5�   "   $
��BACK��6,   
�������$��
Montserrat)�6K   5�      5�      �� 5�      -   5        $ homeBtn��5�   �   $��1��6!   
������ 
������ $
��Home��6�   $
 Hand��6�   ��6d   �� *ChangeViewCommand+.��5�      �� .+�5�   "   �� $��Images/home.png��6(   $��Uniform=�6A   $��26��6S   $��5q�6^   �� 5�   "   $
��HOME��6,   
�������$��
Montserrat)�6K   5�      5�      5�      5�      5�      5�      5�      5�      