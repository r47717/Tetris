   M S B A M L   `   `   `  ����A  =Tetris, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:TetrisTetris  5   	       Tetris.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:Tetris     5   	   -   5          � Title$  R47717 Tetris��5   	   $	��500��6   $	��500��6,    � 
ResizeMode$ NoResize=�68   ���	#FFEEEEEE � WindowStartupLocation$ CenterScreen=�6|   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $	��15* �6   �� 5      $
��155* �6   �� 5      $	��60* �6   �� 5      $	��15* �6    � RowDefinitions 5   
   �� 5      $	��30* �6   �� 5      $
��387* �6   �� 5      $	��30* �6   .+�5   
   �� -   5         ��Name$ 
mainCanvas��5      $��1��6%   $
��Left=�65   $	��387��6P   $��1��6]   $	��Top=�6j   $	��291��6�   $��	10,10,0,0q�6�   
�������6�   �� 5   
   -   5         ��Name$ label1��5      $��Next Figure:��6    $��2��67   $
��Left=�6G   $��28��6b   $��	10,10,0,0q�6n   $��1��6�   $	��Top=�6�   $	��101��6�   �� 5   
   -   5        $ previewCanvas��5      $��2��6(   $
��Left=�68   $	��100��6S   $��	10,59,0,0q�6`   $��1��6s   $	��Top=�6�   $	��100��6�   
�������6�   �� 5   
   -   5        $ 	statLabel��5      $��Lines: 0��6#   $��2��66   $
��Left=�6F   $��28��6a   $��
10,181,0,0q�6m   $��1��6�   $	��Top=�6�   $	��101��6�   �� 5   
   
���   �$��1q�6%   $��2��69   $
��Left=�6I   $	��102��6d   $��9,58,0,0q�6q   $��1��6�   $	��Top=�6�   $	��102��6�   o� 5   
   -   5        $��mainMenu��5   !    o� 
IsMainMenu . $��4��61   .@�5      m� $��_File��6   $
��Left=�6&   $��50��6A   .@�5      m� -   5         m�Name$ 	menuStart��5      $��_Start��6.   6>   m� 5      -   5        $ menuStop��5      $��S_top��6-   6<   m� 5      -	   5        $ menuRestart��5      $��_Restart��60   6B   �� 5      m� 5       -
   5        $��_Exit��5       6*   5!      m� 5"      $��_Help��6   $
��Left=�6&   $��50��6A   .@�5#      m� -   5        $��	_About...��5#      6.   5$      5%      5&      5'      