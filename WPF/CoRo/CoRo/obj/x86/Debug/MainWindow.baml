   M S B A M L   `   `   `  ����?  ;CoRo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:CoRoCoRo  5   	       CoRo.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   $localclr-namespace:CoRo     5   	     � Title$
  CoRo��5   	   $	��480��6   $	��640��6#    � WindowState$ 	Maximized=�6/   $	��640��6G   $	��480��6V    � Icon$ ./Resources/cr.ico��6f   .��5	      �  � RowDefinitions 5
   
   �� 5      $��70 �6   6*   �� 5      $��* �6   6)   �� 5      $��20 �6   6*   .+�5   
   � $��1��6    � ColumnDefinitions 5      �� 5      -   5         ��Name$ 
treeColumn��5   #   $
��125* �67   $��10��6D   $	��250��6R   �� 5      $
��Auto �6#   �� 5      $
��131* �6#   �� 5      $
��Auto �6#   �� 5      -   5        $ deviceColumn��5   #   $��10 �69   $��10��6D   $	��250��6R   .+�5      �� $��1��6   $��1��6(   $��2��68    �� ResizeDirection$
 Auto=�6B   $��1��6Y   $��Stretch=�6j   
�������6�   �� 5      $��1��6   $��3��6(   $��2��68   $
 Auto=�6B   $��1��6Y   $��Stretch=�6j   
�������6�   R� 5      -   5         R�Name$ treeView��5      $��1��6)   $��0��66   $��Stretch=�6F   $��Stretch=�6d   
�������
�������$��2q�6�   7  1System.Windows.Forms.Integration.WindowsFormsHost  5      -   5          Name$ WFHost��5      $��2��60   $��Stretch=�6@   $��Stretch=�6^   �� 5      -   5        	 ��Name$	 
buttonTree��5      $��1��6)   $��2��66   $��20��6F   $��20��6S   $
��Left=�6^   $��Marlett)�6y   $��16(�6�   $��3��6�   
�������
�������6�   �� 5      $��64��6   $��1��6$   $��2��61   $��Right=�6A   $��Center=�6]   .+�5      �� -   5        $	 buttonRobot��5      $��1��6.   $��2��6;   $��Right=�6K   
�������
�������
 � ToolTip$
 Robot��6�   .��5      �� .+�5      �� $!��./Resources/robot_small.png��6    5       5!      �� 5"      -   5        $	 buttonSensor��5"      $��1��6/   $��2��6<   $��Right=�6L   
�������
�������$
 Sensor��6�   .��5#      �� .+�5$      �� $#��./Resources/scanner_small.png��6    5%      5&      5'      �� 5(      -   5         ��Name$ labelDevice��5(      $��1��6)   $��4��66   .��6F   Device6N   5)      ]� 5*   
   $��70��6   $��0��6   $��0��6,   $
��Left=�6<   .+�5+      �� $��Right=�6    �� EdgeMode$ Aliased=�60    �� BitmapScalingMode$ NearestNeighbor=�6Q   $
��None=�6�   $��./Resources/cr.png��6�   $	��144��6�   5,      ]� 5-   
   $	��270��6   $��0��6    $��2��6-   $��Right=�6=   .+�5.      �� $
��Left=�6   $ Aliased=�6/   $ NearestNeighbor=�6P   $
��None=�6�   $��./Resources/aut.png��6�   �� 5/      $
��Left=�6   $ Aliased=�6/   $ NearestNeighbor=�6P   $
��None=�6�   $��./Resources/nct.png��6�   50      51      52      